using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    // Top-down enemy controller with simple coroutine-based FSM:
    // Idle -> Patrol -> Attack -> ReturnToOrigin
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownEnemyController : MonoBehaviour
    {
        private static readonly int MOVE_X = Animator.StringToHash("MoveX");
        private static readonly int MOVE_Y = Animator.StringToHash("MoveY");
        private static readonly int IS_MOVE = Animator.StringToHash("IsMove");
        private static readonly int ATTACK = Animator.StringToHash("Attack");

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float baseUpdateTerm = 0.1f;

        [Header("Timing")]
        [SerializeField] private float idleWaitTime = 2f;
        [SerializeField] private float maxChaseDuration = 7f;
        [SerializeField] private float blockedChaseDuration = 5f;

        [Header("Detection")]
        [SerializeField] private float traceRange = 8f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField, Range(0f, 360f)] private float fieldOfView = 120f; // degrees

        [Header("Obstacle Avoidance")]
        [SerializeField] private LayerMask obstacleMask;
        [SerializeField] private float avoidanceDistance = 0.6f;
        [SerializeField] private float avoidanceRadius = 0.25f;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;

        [Header("References")]
        [field: SerializeField] public Transform Target { get; private set; }

        private Animator Animator { get; set; }
        private Rigidbody2D rBody;
        private Vector2 currentVelocity;

        private Vector3 originPosition;
        private int patrolIndex = 0;
        private Vector3 lastKnownTargetPosition;

        private float chaseTimer;
        private bool isBlockedChase;

        // Facing direction used for FOV checks and animator
        private Vector2 facing = Vector2.down;

        // Coroutine state
        protected IEnumerator nextStateCoroutine = null;

        private void Awake()
        {
            Animator = GetComponentInChildren<Animator>();
            rBody = GetComponent<Rigidbody2D>();
            rBody.bodyType = RigidbodyType2D.Kinematic;
            rBody.freezeRotation = true;

            originPosition = transform.position;

            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                patrolIndex = 0;
            }
        }

        private void OnEnable()
        {
            nextStateCoroutine = IdleStateCoroutine();
            StartCoroutine(FiniteStateMachineCoroutine());
        }

        private void FixedUpdate()
        {
            if (currentVelocity.sqrMagnitude > 0.0001f)
            {
                rBody.MovePosition(rBody.position + currentVelocity * Time.fixedDeltaTime);
            }
        }

        private IEnumerator FiniteStateMachineCoroutine()
        {
            while (gameObject.activeInHierarchy)
            {
                yield return StartCoroutine(nextStateCoroutine);
            }
        }

        private IEnumerator IdleStateCoroutine()
        {
            float waited = 0f;

            WaitForSeconds term = new WaitForSeconds(baseUpdateTerm);

            while (true)
            {
                // If player seen -> Attack
                if (IsTargetVisible())
                {
                    nextStateCoroutine = AttackStateCoroutine();
                    yield break;
                }

                // After idle time, go patrol if points exist
                waited += baseUpdateTerm;
                if (patrolPoints != null && patrolPoints.Length > 0 && waited >= idleWaitTime)
                {
                    nextStateCoroutine = PatrolStateCoroutine();
                    yield break;
                }

                yield return term;
            }
        }

        private IEnumerator PatrolStateCoroutine()
        {
            const float STOPPING_DISTANCE = 0.1f;

            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                nextStateCoroutine = IdleStateCoroutine();
                yield break;
            }

            while (true)
            {
                // If target appears, interrupt
                if (IsTargetVisible())
                {
                    nextStateCoroutine = AttackStateCoroutine();
                    yield break;
                }

                Vector3 goal = patrolPoints[patrolIndex].position;
                goal.z = transform.position.z;

                if (Vector2.Distance(transform.position, goal) <= STOPPING_DISTANCE)
                {
                    // advance patrol and go idle
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                    SetMovementAnim(Vector2.zero);
                    nextStateCoroutine = IdleStateCoroutine();
                    yield break;
                }

                MoveTowards(goal);
                yield return null;
            }
        }

        private IEnumerator AttackStateCoroutine()
        {
            chaseTimer = 0f;
            isBlockedChase = false;
            lastKnownTargetPosition = Target != null ? Target.position : transform.position;

            while (true)
            {
                if (Target == null)
                {
                    nextStateCoroutine = ReturnToOriginCoroutine();
                    yield break;
                }

                Vector2 toTarget = Target.position - transform.position;
                float dist = toTarget.magnitude;
                bool withinTrace = dist <= traceRange;
                bool inFov = IsTargetInFOV(toTarget);
                bool obstructed = withinTrace && inFov && IsObstacleBetween(transform.position, Target.position);
                bool visible = withinTrace && inFov && !obstructed;

                if (!withinTrace)
                {
                    nextStateCoroutine = ReturnToOriginCoroutine();
                    yield break;
                }

                if (visible)
                {
                    lastKnownTargetPosition = Target.position;
                    if (isBlockedChase)
                    {
                        isBlockedChase = false;
                        chaseTimer = 0f;
                    }

                    chaseTimer += Time.deltaTime;
                    if (chaseTimer > maxChaseDuration)
                    {
                        nextStateCoroutine = ReturnToOriginCoroutine();
                        yield break;
                    }
                }
                else if (obstructed)
                {
                    if (!isBlockedChase)
                    {
                        isBlockedChase = true;
                        chaseTimer = 0f;
                    }

                    chaseTimer += Time.deltaTime;
                    if (chaseTimer > blockedChaseDuration)
                    {
                        nextStateCoroutine = ReturnToOriginCoroutine();
                        yield break;
                    }
                }
                else
                {
                    nextStateCoroutine = ReturnToOriginCoroutine();
                    yield break;
                }

                Vector3 targetPos = lastKnownTargetPosition;
                targetPos.z = transform.position.z;

                if (visible && dist <= attackRange)
                {
                    currentVelocity = Vector2.zero;
                    SetMovementAnim(Vector2.zero);
                    Animator.SetTrigger(ATTACK);
                    ProcessAttack();
                    yield return new WaitForSeconds(1.0f);
                }
                else
                {
                    MoveTowards(targetPos, visible);
                }

                yield return null;
            }
        }

        private IEnumerator ReturnToOriginCoroutine()
        {
            const float STOPPING_DISTANCE = 0.1f;

            while (true)
            {
                // If player reappears
                if (IsTargetVisible())
                {
                    nextStateCoroutine = AttackStateCoroutine();
                    yield break;
                }

                if (Vector2.Distance(transform.position, originPosition) <= STOPPING_DISTANCE)
                {
                    SetMovementAnim(Vector2.zero);
                    nextStateCoroutine = IdleStateCoroutine();
                    yield break;
                }

                MoveTowards(originPosition);
                yield return null;
            }
        }

        protected virtual void ProcessAttack()
        {
            // Override in derived classes to apply damage/effects.
        }

        private void MoveTowards(Vector3 goal, bool isTargetVisible = false)
        {
            Vector2 dir = goal - transform.position;

            if (dir.sqrMagnitude <= 0.0001f)
            {
                currentVelocity = Vector2.zero;
                SetMovementAnim(Vector2.zero);
                return;
            }

            Vector2 desiredMoveDir = dir.normalized;
            Vector2 moveDir = GetAvoidedDirection(desiredMoveDir);

            if (moveDir.sqrMagnitude <= 0.0001f)
            {
                currentVelocity = Vector2.zero;
                SetMovementAnim(Vector2.zero);
                return;
            }

            facing = desiredMoveDir;
            currentVelocity = moveDir * moveSpeed;
            SetMovementAnim(desiredMoveDir);
        }

        private Vector2 GetAvoidedDirection(Vector2 desiredDirection)
        {
            if (IsPathClear(desiredDirection))
                return desiredDirection;

            Vector2 leftCheck = new Vector2(-desiredDirection.y, desiredDirection.x);
            Vector2 rightCheck = new Vector2(desiredDirection.y, -desiredDirection.x);

            if (IsPathClear(leftCheck))
                return leftCheck;
            if (IsPathClear(rightCheck))
                return rightCheck;

            Vector2 leftDiagonal = (desiredDirection + leftCheck).normalized;
            Vector2 rightDiagonal = (desiredDirection + rightCheck).normalized;

            if (IsPathClear(leftDiagonal))
                return leftDiagonal;
            if (IsPathClear(rightDiagonal))
                return rightDiagonal;

            return Vector2.zero;
        }

        private bool IsPathClear(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f)
                return false;

            RaycastHit2D hit = Physics2D.CircleCast(rBody.position, avoidanceRadius, direction, avoidanceDistance, obstacleMask);
            return hit.collider == null;
        }

        private void SetMovementAnim(Vector2 move)
        {
            Animator.SetFloat(MOVE_X, move.x);
            Animator.SetFloat(MOVE_Y, move.y);
            Animator.SetBool(IS_MOVE, move.sqrMagnitude > 0.001f);
        }

        private bool IsTargetVisible()
        {
            if (Target == null) return false;
            Vector2 toTarget = Target.position - transform.position;
            float distance = toTarget.magnitude;

            if (distance > traceRange) return false;
            if (!IsTargetInFOV(toTarget)) return false;
            if (IsObstacleBetween(transform.position, Target.position)) return false;

            return true;
        }

        private bool IsTargetObstructed()
        {
            if (Target == null) return false;
            Vector2 toTarget = Target.position - transform.position;
            if (toTarget.magnitude > traceRange) return false;
            if (!IsTargetInFOV(toTarget)) return false;
            return IsObstacleBetween(transform.position, Target.position);
        }

        private bool IsTargetInFOV(Vector2 toTarget)
        {
            if (toTarget.sqrMagnitude < 0.0001f) return true;

            Vector2 dir = toTarget.normalized;
            float angle = Vector2.Angle(facing, dir);
            return angle <= (fieldOfView * 0.5f);
        }

        private bool IsObstacleBetween(Vector3 origin, Vector3 targetPosition)
        {
            Vector2 direction = (targetPosition - origin).normalized;
            float distance = Vector2.Distance(origin, targetPosition);
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleMask);
            return hit.collider != null && hit.collider.gameObject != Target.gameObject;
        }
    }
}
