using System.Collections;
using UnityEngine;

// 적의 모든 데이터를 보관하고, 상태 전환만 담당하는 매니저
public class EnemyStateManager : MonoBehaviour
{
    // ───────────── 컴포넌트 참조 ─────────────
    [HideInInspector] public Animator Animator;
    [HideInInspector] public Rigidbody2D Rigidbody;

    // ───────────── 인스펙터 설정값 ─────────────
    [Header("감지 범위")]
    [SerializeField] private Collider2D viewCollider;       // 시야 콜라이더
    [SerializeField] private Collider2D attackHitBox;       // 공격 히트박스
    [SerializeField] private Collider2D backHitBox;         // 뒤통수 히트박스

    [Header("이동")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("순찰")]
    [SerializeField] private Transform patrolPoint;         // 순찰 목표 지점

    [Header("공격")]
    [SerializeField] private float attackCooldown = 0.8f;
    [SerializeField] private float counterAttackDelay = 0.5f;

    [Header("추적 이탈 후 탐색")]
    [SerializeField] private float searchDuration = 3f;

    [Header("컷씬")]
    [SerializeField] private GameObject knockdownCanvas;    // KnockDown 컷씬 캔버스

    // ───────────── 공개 프로퍼티 (State에서 읽기 전용) ─────────────
    public float MoveSpeed => moveSpeed;
    public float AttackCooldown => attackCooldown;
    public float CounterAttackDelay => counterAttackDelay;
    public float SearchDuration => searchDuration;
    public Collider2D ViewCollider => viewCollider;
    public Collider2D AttackHitBox => attackHitBox;
    public Collider2D BackHitBox => backHitBox;
    public GameObject KnockdownCanvas => knockdownCanvas;

    // ───────────── 런타임 공유 데이터 ─────────────
    public Transform Target { get; set; }                   // 현재 추적 대상 (플레이어)
    public Vector2 OriginPosition { get; private set; }     // 최초 배치 위치
    public Transform PatrolPoint => patrolPoint;            // 순찰 지점
    public Vector2 LastKnownTargetPosition { get; set; }   // 시야를 잃었을 때 마지막 위치
    public Vector2 MoveDirection { get; set; }              // 현재 이동 방향 (히트박스 회전용)

    // ───────────── 상태 머신 내부 ─────────────
    private IEnemyState currentState;
    private Coroutine executeCoroutine;

    private void Awake()
    {
        // Animator는 자식 오브젝트에 붙어 있을 수 있으므로 GetComponentInChildren을 사용한다
        Animator = GetComponentInChildren<Animator>();
        Rigidbody = GetComponent<Rigidbody2D>();
        OriginPosition = transform.position;

        if (Animator == null)
        {
            Debug.LogError($"[EnemyStateManager] Animator를 찾을 수 없습니다. {gameObject.name} 또는 자식 오브젝트에 Animator가 있는지 확인하세요.");
        }
    }

    private void Start()
    {
        ChangeState(new IdleState());
    }

    // 새로운 상태로 전환한다
    public void ChangeState(IEnemyState newState)
    {
        // 이전 상태의 Coroutine을 중단한다
        if (executeCoroutine != null)
        {
            StopCoroutine(executeCoroutine);
        }

        // 이전 상태 종료
        currentState?.Exit(this);

        // 새 상태 시작
        currentState = newState;
        currentState.Enter(this);

        // 새 상태의 Execute Coroutine을 시작한다
        executeCoroutine = StartCoroutine(currentState.Execute(this));

        Debug.Log($"[EnemyStateManager] 상태 전환 → {newState.GetType().Name}");
    }

    // 뒤통수 히트박스와 시야 콜라이더를 이동 방향에 맞게 회전시킨다
    public void RotateHitBoxes()
    {
        float angle = GetAngleFromDirection(MoveDirection);
        RotateTransform(backHitBox.transform, angle);
        RotateTransform(viewCollider.transform, GetViewAngleFromDirection(MoveDirection));
    }

    // 공격 히트박스를 이동 방향에 맞게 회전시킨다
    public void RotateAttackHitBox()
    {
        float angle = GetAngleFromDirection(MoveDirection);
        RotateTransform(attackHitBox.transform, angle);
    }

    // 이동 방향 → 뒤통수·공격 히트박스 회전 각도
    // 아래:0° / 위:180° / 왼쪽:270° / 오른쪽:90°
    private float GetAngleFromDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? 90f : 270f;
        }
        return direction.y > 0 ? 180f : 0f;
    }

    // 이동 방향 → 시야 콜라이더 회전 각도
    // 아래:270° / 위:90° / 왼쪽:180° / 오른쪽:0°
    private float GetViewAngleFromDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? 0f : 180f;
        }
        return direction.y > 0 ? 90f : 270f;
    }

    private void RotateTransform(Transform target, float angle)
    {
        target.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Blend Tree — 정지 상태로 전환 (Idle 애니메이션)
    public void PlayIdle()
    {
        Animator.SetBool("IsMoving", false);
        Animator.SetBool("IsAttacking", false);
    }

    // Blend Tree — 이동 상태로 전환 (Move 애니메이션)
    public void PlayMove()
    {
        Animator.SetBool("IsMoving", true);
        Animator.SetBool("IsAttacking", false);
    }

    // Blend Tree — 공격 상태로 전환 (Attack 애니메이션)
    public void PlayAttack()
    {
        Animator.SetBool("IsMoving", false);
        Animator.SetBool("IsAttacking", true);
    }

    // KnockDown은 Trigger로 처리 — 한 번만 발동하면 되기 때문
    public void PlayKnockdown()
    {
        Animator.SetTrigger("KnockDown");
    }

    // Animator 파라미터 설정 헬퍼 — 방향에 맞는 애니메이션을 재생한다
    public void SetAnimatorDirection(Vector2 direction)
    {
        Animator.SetFloat("MoveX", direction.x);
        Animator.SetFloat("MoveY", direction.y);
    }

    // 플레이어가 시야 콜라이더 안에 있고 장애물이 없으면 true를 반환한다
    public bool IsPlayerVisible()
    {
        if (Target == null)
        {
            return false;
        }

        // 시야 콜라이더 범위 확인
        if (viewCollider.bounds.Contains(Target.position) == false)
        {
            return false;
        }

        // Raycast로 장애물 확인
        return IsRaycastClear();
    }

    private bool IsRaycastClear()
    {
        Vector2 toTarget = (Vector2)Target.position - (Vector2)transform.position;
        int obstacleLayer = LayerMask.GetMask("Obstacle");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toTarget.normalized, toTarget.magnitude, obstacleLayer);
        return hit.collider == null;
    }

    // 시야 콜라이더 안으로 플레이어가 진입하면 Target에 할당한다
    // ViewCollider의 IsTrigger가 체크되어 있어야 호출된다
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        Target = other.transform;
        Debug.Log("[EnemyStateManager] 플레이어 감지 → Target 할당");
    }

    // 플레이어가 시야 콜라이더 밖으로 나가면 Target을 해제한다
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
        {
            return;
        }

        Target = null;
        Debug.Log("[EnemyStateManager] 플레이어 시야 이탈 → Target 해제");
    }
}
