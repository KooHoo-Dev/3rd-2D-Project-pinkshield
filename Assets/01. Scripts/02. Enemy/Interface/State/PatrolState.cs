using System.Collections;
using UnityEngine;

// 순찰 상태 — Origin ↔ PatrolPoint 사이를 왕복하며 이동한다
// 플레이어 발견 시 → Chase
// 목표 지점 도착 시 → Idle (이후 Idle이 다음 목표를 향해 재출발)
public class PatrolState : IEnemyState
{
    private const float ArrivalDistance = 0.1f;

    // true : PatrolPoint로 이동 중 / false : Origin으로 이동 중
    private bool isHeadingToPatrolPoint = true;

    public void Enter(EnemyStateManager stateManager)
    {
        // Blend Tree — 이동 상태
        stateManager.PlayMove();
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {
        while (true)
        {
            // 플레이어 발견 시 즉시 Chase로 전환
            if (stateManager.IsPlayerVisible() == true)
            {
                stateManager.ChangeState(new ChaseState());
                yield break;
            }

            Vector2 destination = GetDestination(stateManager);
            MoveToward(stateManager, destination);

            // 목적지에 도착하면 Idle로 전환하고, 다음 순찰 방향을 뒤집는다
            if (HasArrived(stateManager, destination) == true)
            {
                isHeadingToPatrolPoint = isHeadingToPatrolPoint == false;
                stateManager.ChangeState(new IdleState());
                yield break;
            }

            yield return null;
        }
    }

    public void Exit(EnemyStateManager stateManager)
    {
        stateManager.Rigidbody.linearVelocity = Vector2.zero;
        Debug.Log("[PatrolState] Exit");
    }

    private Vector2 GetDestination(EnemyStateManager stateManager)
    {
        if (isHeadingToPatrolPoint == true)
        {
            return stateManager.PatrolPoint.position;
        }
        return stateManager.OriginPosition;
    }

    private void MoveToward(EnemyStateManager stateManager, Vector2 destination)
    {
        Vector2 direction = ((Vector2)stateManager.transform.position - destination).normalized * -1f;

        stateManager.MoveDirection = direction;
        stateManager.Rigidbody.linearVelocity = direction * stateManager.MoveSpeed;
        stateManager.SetAnimatorDirection(direction);
        stateManager.RotateHitBoxes();
    }

    private bool HasArrived(EnemyStateManager stateManager, Vector2 destination)
    {
        float distance = Vector2.Distance(stateManager.transform.position, destination);
        return distance <= ArrivalDistance;
    }
}
