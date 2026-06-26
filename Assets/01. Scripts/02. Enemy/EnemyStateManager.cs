using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyStateManager : MonoBehaviour
    {
        [Header("Settings")]
        public float Speed = 3.0f;
        public Transform PatrolPosition;
        public Vector2 SpawnPosition;

        [Header("Components")]
        public Rigidbody2D Rb;
        public Animator Anim;
        public Transform Player;

        [Header("Box Transforms")]
        public Transform SightBox;      // 회전 제어용
        public Transform AttackHitbox;   // 회전 제어용
        public Transform BackHurtBox;    // 회전 제어용

        [Header("Box Colliders")]
        public Collider2D SightBoxCollider;     // 플레이어 감지용
        public Collider2D AttackHitboxCollider;  // 공격 사거리 판정용
        public Collider2D BackHurtBoxCollider;   // 뒤통수 피격 판정용

        [Header("Chase Settings")]
        public float StopDistance = 0.5f;   // 플레이어와 유지할 최소 거리
        public float FlipThreshold = 0.1f;  // 방향 반전을 허용할 최소 이동량


        public LayerMask ObstacleLayer;
        public bool IsGoingToPatrolPoint = true;

        private IEnemyState _currentState;
        private Coroutine _stateCoroutine;
        

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Anim = GetComponentInChildren<Animator>();
            SpawnPosition = transform.position; // 시작 위치 기억 [5]
        }

        private void Start()
        {
            ChangeState(new EnemyIdleState()); // 초기 상태 설정 [6]
        }

        public void ChangeState(IEnemyState newState)
        {
            if (_currentState != null) _currentState.Exit(this); // 상태 종료 처리 [7]

            if (_stateCoroutine != null) StopCoroutine(_stateCoroutine); // 기존 루프 중단 [8]

            _currentState = newState;
            _currentState.Enter(this); // 새로운 상태 진입 [7]
            _stateCoroutine = StartCoroutine(_currentState.Execute(this)); // 상태 루프 시작 [9]
        }

        // 상태들이 공통으로 사용할 이동 함수 (기능 병합) [10]
        public void Move(Vector2 direction)
        {
            Rb.linearVelocity = direction * Speed;
            Anim.SetBool("IsMoving", true);
            Anim.SetFloat("MoveX", direction.x);
            Anim.SetFloat("MoveY", direction.y);
        }

        public void Stop()
        {
            Rb.linearVelocity = Vector2.zero;
            Anim.SetBool("IsMoving", false);
        }

        public void SetAttackAnimation(bool isAttack)
        {
            Anim.SetBool("IsAttacking", isAttack);
        }


        // 적의 플레이어 발견 로직.

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") == true)
            {
                Player = collision.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") == true)
            {
                Player = null;
            }
        }

        // 시야 장애물 체크
        public bool CheckPlayerInSight()
        {
            if (Player == null) return false;

            Vector2 direction = (Player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, Player.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, ObstacleLayer);

            if (hit.collider == null) return true; // 장애물에 가려지지 않음 [2]
            return false;
        }

        // 플레이어가 공격 범위(AttackHitbox) 안에 있는지 체크
        public bool CheckPlayerInAttackRange()
        {
            if (Player == null) return false;

            // IsTouching을 사용하여 플레이어의 콜라이더와 겹치는지 확인
            Collider2D playerCollider = Player.GetComponent<Collider2D>();
            if (playerCollider == null) return false;

            bool isTouching = AttackHitboxCollider.IsTouching(playerCollider);

            if (isTouching == true) return true;
            return false;
        }

        public void UpdateBoxRotations(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01f) return;

            float absX = Mathf.Abs(direction.x);
            float absY = Mathf.Abs(direction.y);

            if (absX > absY) // 좌우 이동이 클 때
            {
                if (direction.x > 0) SetRotations(0f, 90f, 0f);    // 우
                else SetRotations(180f, 270f, 180f);               // 좌
            }
            else // 상하 이동이 클 때
            {
                if (direction.y > 0) SetRotations(90f, 180f, 90f);  // 위
                else SetRotations(270f, 0f, 270f);                   // 아래
            }
        }

        private void SetRotations(float attackRot, float hurtRot, float sightRot)
        {
            AttackHitbox.localEulerAngles = new Vector3(0, 0, attackRot);
            BackHurtBox.localEulerAngles = new Vector3(0, 0, hurtRot);
            SightBox.localEulerAngles = new Vector3(0, 0, sightRot);
        }
    }

}



