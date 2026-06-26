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

        [Header("Rotation Settings")]
        public float RotationSpeed = 10f; // 회전의 부드러운 정도

        


        public LayerMask ObstacleLayer;
        public bool IsGoingToPatrolPoint = true;

        private IEnemyState _currentState;
        private Coroutine _stateCoroutine;
        private float _targetAttackRot;
        private float _targetHurtRot;
        private float _targetSightRot;

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

        public void UpdateBoxRotations()
        {
            float moveX = Anim.GetFloat("MoveX");
            float moveY = Anim.GetFloat("MoveY");

            // 정지 상태일 때는 마지막 목표 각도를 유지합니다
            if (Mathf.Abs(moveX) < 0.01f && Mathf.Abs(moveY) < 0.01f)
            {
                 ApplyRotations();
                return;
            }

            float absX = Mathf.Abs(moveX);
            float absY = Mathf.Abs(moveY);

            // X, Y 중 큰 값을 기준으로 목표 각도를 0~360도 양수값으로 설정합니다 [2]
            if (absX > absY)
            {
                if (moveX > 0) SetTargetAngles(90f, 90f, 0f);    // 오른쪽
                else SetTargetAngles(270f, 270f, 180f);          // 왼쪽
            }
            else
            {
                if (moveY > 0) SetTargetAngles(180f, 180f, 90f); // 위
                else SetTargetAngles(0f, 0f, 270f);              // 아래
            }

             ApplyRotations();
        }

        private void SetTargetAngles(float attack, float hurt, float sight)
        {
            _targetAttackRot = attack;
            _targetHurtRot = hurt;
            _targetSightRot = sight;
        }

        private void ApplyRotations()
        {
            // 1. 공격 히트박스와 뒤통수 허트박스는 즉시 회전하여 정확한 방향을 가리킵니다. 
            AttackHitbox.localEulerAngles = new Vector3(0, 0, _targetAttackRot);
            BackHurtBox.localEulerAngles = new Vector3(0, 0, _targetHurtRot);

            // 2. 시야 박스만 보간을 사용하여 부드럽게 회전합니다. 
            float currentSight = SightBox.localEulerAngles.z;
            float nextSight = Mathf.LerpAngle(currentSight, _targetSightRot, Time.deltaTime * RotationSpeed);
            
            SightBox.localEulerAngles = new Vector3(0, 0, nextSight);
        }
       
        public void OnBackHit(Collider2D collision)
        {
            // 플레이어의 공격 히트박스 태그가 "PlayerAttack"이라고 가정
            if (collision.CompareTag("PlayerAttack") == true)
            {
                Debug.Log("뒤통수 피격! 즉시 다운 상태로 전환");
                ChangeState(new EnemyKnockDownState());
            }
        }

    }

}



