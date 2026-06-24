using System.Collections;
using UnityEngine;

// 추격 상태 — 플레이어를 향해 이동한다
// 공격 범위 진입 시 → Attack
// 시야 밖으로 나가면 → 마지막 위치 탐색 → Origin 복귀
public class ChaseState : IEnemyState
{
    [SerializeField] private float attackRange = 0.8f;

    public void Enter(EnemyStateManager stateManager)
    {
        // Blend Tree — 이동 상태
        stateManager.PlayMove();
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {
        while (true)
        {
            bool playerVisible = stateManager.IsPlayerVisible();

            if (playerVisible == true)
            {
                stateManager.LastKnownTargetPosition = stateManager.Target.position;

                // 공격 범위 안이면 Attack으로 전환
                if (IsInAttackRange(stateManager) == true)
                {
                    stateManager.ChangeState(new AttackState());
                    yield break;
                }

                MoveTowardTarget(stateManager);
            }
            else
            {
                // 시야를 잃으면 마지막 위치 탐색 후 Origin 복귀
                stateManager.ChangeState(new SearchState());
                yield break;
            }

            yield return null;
        }
    }

    public void Exit(EnemyStateManager stateManager)
    {
        stateManager.Rigidbody.linearVelocity = Vector2.zero;
        Debug.Log("[ChaseState] Exit");
    }

    private void MoveTowardTarget(EnemyStateManager stateManager)
    {
        Vector2 direction = ((Vector2)stateManager.Target.position - (Vector2)stateManager.transform.position).normalized;

        stateManager.MoveDirection = direction;
        stateManager.Rigidbody.linearVelocity = direction * stateManager.MoveSpeed;
        stateManager.SetAnimatorDirection(direction);
        stateManager.RotateHitBoxes();
    }

    private bool IsInAttackRange(EnemyStateManager stateManager)
    {
        float distance = Vector2.Distance(stateManager.transform.position, stateManager.Target.position);
        return distance <= attackRange;
    }
}
