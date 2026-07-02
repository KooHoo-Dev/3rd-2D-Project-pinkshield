using System.Collections;
using UnityEngine;


namespace WatchYourBack
{
    public class EnemyStateManager : MonoBehaviour
    {

        private const float LookRoutineInterval = 0.6f;

        [Header("Settings")]
        public float Speed = 3.0f;
        public Transform PatrolPosition;
        public Vector2 SpawnPosition;

        [Header("Components")]
        public Rigidbody2D Rb;
        public Animator Anim;
        public Transform Player;
        public GameObject KnockdownCutscnene;

        [Header("Box Transforms")] // 회전 제어용
        public Transform SightBox;      
        public Transform AttackRangebox;   
        public Transform AttackHitBox;
        public Transform BackHurtBox;    
        

        [Header("Box Colliders")]
        public Collider2D SightBoxCollider;     // 플레이어 감지용
        public Collider2D AttackRangeboxCollider;  // 공격 사거리 판정용
        public Collider2D BackHurtBoxCollider;   // 뒤통수 피격 판정용

        [Header("Chase Settings")]
        public float StopDistance = 0.5f;   // 플레이어와 유지할 최소 거리
        public float FlipThreshold = 0.1f;  // 방향 반전을 허용할 최소 이동량

        [Header("Rotation Settings")]
        public float RotationSpeed = 10f; // 회전의 부드러운 정도

        [Header("Advanced Avoidance")] // 장애물 회피 전용 
        public float DetectionDistance = 1.5f; // 발견 거리
        public float AvoidanceAngle = 30f; // 레이빔 사이의 각도
        public int RayCount = 5; // 쏠 레이빔의 개수 (홀수 권장)

        [Header("BoxCast Settings")]
        public Vector2 BoxSize = new Vector2(1.0f, 1.0f); // 캐릭터 실제 크기보다 약간 작게 설정
        public float SkinWidth = 0.1f; // 끼임 방지 여유 공간



        public LayerMask ObstacleLayer;
        public bool IsGoingToPatrolPoint = true;
        public Vector2 MoveDirection { get; set; }
        public Vector2 LastPlayerPosition { get; set; }



        private IEnemyState currentState;
        private Coroutine stateCoroutine;

        // 각도들
        private float targetAttackRot; 
        private float targetHurtRot;
        private float targetSightRot;
        

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Anim = GetComponentInChildren<Animator>();
            SpawnPosition = transform.position; // 시작 위치 기억 
        }

        private void Start()
        {
            ChangeState(new EnemyIdleState()); // 초기 상태 설정 
        }


        private void Update()
        {
            UpdateBoxRotations();

        }

        public void ChangeState(IEnemyState newState)
        {
            if (currentState != null) currentState.Exit(this); // 상태 종료 처리 [7]

            if (stateCoroutine != null) StopCoroutine(stateCoroutine); // 기존 루프 중단 [8]

            currentState = newState;
            currentState.Enter(this); // 새로운 상태 진입 
            stateCoroutine = StartCoroutine(currentState.Execute(this)); // 상태 루프 시작 [9]
        }

        // 상태들이 공통으로 사용할 이동 함수 (기능 병합)
       

        public void MoveWithAvoidance(Vector2 targetDirection)
        {
            Vector2 finalDirection = targetDirection;
            

            // 1. 여러 갈래로 레이를 쏴서 비어있는 가장 가까운 경로 탐색 
            for (int i = 0; i < RayCount; i++)
            {
                // 0, 30, -30... 순서로 각도 계산
                float angle = AvoidanceAngle * (i % 2 == 0 ? i / 2 : -(i / 4 + 1));
                Vector2 checkDir = Quaternion.Euler(0, 0, angle) * targetDirection;

                // 선(Ray)이 아닌 사각형(Box)을 쏴서 충돌 감지 
                RaycastHit2D hit = Physics2D.BoxCast(Rb.position, BoxSize, 0f, checkDir, DetectionDistance, ObstacleLayer);

                if (hit.collider == null)
                {
                    finalDirection = checkDir;
                    break;
                }
            }

            // 2. 이동 및 애니메이션 적용 
            MoveDirection = finalDirection;
            Rb.linearVelocity = finalDirection * Speed;
            Anim.SetBool("IsMoving", true);
            Anim.SetFloat("MoveX", finalDirection.x);
            Anim.SetFloat("MoveY", finalDirection.y);
            
        }


        public void Stop()
        {
            Rb.linearVelocity = Vector2.zero;
            Anim.SetBool("IsMoving", false);
            
        }

        public void SetAttackAnimation(bool isAttaking)
        {
            Anim.SetBool("IsAttack", isAttaking);
        }


        // 적의 플레이어 발견 로직.

        public void OnBoxTriggerEnter(EnemyBoxType boxType, Collider2D collision)
        {
            if (boxType == EnemyBoxType.Sight)
            {
                Player = collision.transform;
            }
        }

        public void OnBoxTriggerExit(EnemyBoxType boxType, Collider2D collision)
        {
                        
            if (boxType == EnemyBoxType.Sight)
            {
                Player = null; // 시야를 벗어났을 때만 해제
            }
        }

        // 시야 장애물 체크
        public bool CheckPlayerInSight()
        {
            if (Player == null) return false;

            Vector2 direction = (Player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, Player.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, ObstacleLayer);

            if (hit.collider == null) return true; // 장애물에 가려지지 않음 
            return false;
        }


        // 멈춘 상태에서 주변 돌아보기 (추격 혹은 조사 시)

        public void SetLookAround(Vector2 direction)
        {
            Anim.SetFloat("MoveX", direction.x);            
            Anim.SetFloat("MoveY", direction.y);
            
        }


        // 랜덤 방향으로 주변 돌아보기
        public void SetRandomLookAround()
        {
            Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
            Vector2 randomDir = directions[Random.Range(0, directions.Length)];

            SetLookAround(randomDir);
            
        }

        public IEnumerator LookAroundRoutine()
        {
            Vector2[] directions = { Vector2.down, Vector2.right, Vector2.up, Vector2.left };
            

            foreach (Vector2 dir in directions)
            {
                SetLookAround(dir);                
                yield return new WaitForSeconds(LookRoutineInterval);
            }
        }


        // 플레이어가 공격 범위(AttackHitbox) 안에 있는지 체크
        public bool CheckPlayerInAttackRange()
        {
            if (Player == null) return false;

            // IsTouching을 사용하여 플레이어의 콜라이더와 겹치는지 확인
            Collider2D playerCollider = Player.GetComponent<Collider2D>();
            if (playerCollider == null) return false;

            bool isTouching = AttackRangeboxCollider.IsTouching(playerCollider);

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

        private void SetTargetAngles(float attackSight,float hurt, float sight)
        {
            targetAttackRot = attackSight;
            targetHurtRot = hurt;
            targetSightRot = sight;
        }

        private void ApplyRotations()
        {
            // 1. 공격 히트박스와 뒤통수 허트박스는 즉시 회전하여 정확한 방향을 가리킵니다. 
            AttackRangebox.localEulerAngles = new Vector3(0, 0, targetAttackRot);
            BackHurtBox.localEulerAngles = new Vector3(0, 0, targetHurtRot);
            AttackHitBox.localEulerAngles = new Vector3(0, 0, targetAttackRot);


            // 2. 시야 박스만 보간을 사용하여 부드럽게 회전합니다. 
            float currentSight = SightBox.localEulerAngles.z;
            float nextSight = Mathf.LerpAngle(currentSight, targetSightRot, Time.deltaTime * RotationSpeed);

            SightBox.localEulerAngles = new Vector3(0, 0, nextSight);
        }

        public void OnBackHit(Collider2D collision)
        {
            // 플레이어의 공격 히트박스 태그가 "PlayerAttack"
            if (collision.CompareTag("PlayerAttack") == true)
            {
                Debug.Log("뒤통수 피격! 즉시 다운 상태로 전환");
                ChangeState(new EnemyKnockDownState());
            }
        }

    }

}