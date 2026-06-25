using System.Collections;
using UnityEngine;

// 탐색 상태 — 플레이어를 잃었을 때 마지막 위치로 이동 후 주변 탐색
// 탐색 중 발견 시 → Chase / 못 찾으면 → Origin 복귀 → Idle
public class SearchState : IEnemyState
{
    private const float ArrivalDistance = 0.1f;

    public void Enter(EnemyStateManager stateManager)
    {
        // Blend Tree — 이동 상태
        stateManager.PlayMove();
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {
        // 1단계 : 마지막으로 목격한 위치까지 이동
        yield return MoveToPosition(stateManager, stateManager.LastKnownTargetPosition);

        // 2단계 : 그 자리에서 SearchDuration 동안 주변 탐색
        yield return SearchAround(stateManager);

        // 3단계 : 플레이어를 찾지 못했으므로 Origin으로 복귀
        yield return MoveToPosition(stateManager, stateManager.OriginPosition);

        stateManager.ChangeState(new IdleState());
    }

    public void Exit(EnemyStateManager stateManager)
    {
        stateManager.Rigidbody.linearVelocity = Vector2.zero;
        Debug.Log("[SearchState] Exit");
    }

    // 목표 위치까지 이동한다. 이동 중 플레이어를 발견하면 Chase로 즉시 전환한다.
    private IEnumerator MoveToPosition(EnemyStateManager stateManager, Vector2 destination)
    {
        while (HasArrived(stateManager, destination) == false)
        {
            // 이동 중 플레이어 재발견
            if (stateManager.IsPlayerVisible() == true)
            {
                stateManager.ChangeState(new ChaseState());
                yield break;
            }

            MoveToward(stateManager, destination);
            yield return null;
        }

        stateManager.Rigidbody.linearVelocity = Vector2.zero;
    }

    // 제자리에서 4방향을 돌아보며 SearchDuration 동안 탐색한다
    private IEnumerator SearchAround(EnemyStateManager stateManager)
    {
        // Blend Tree — 정지 상태로 전환하여 Idle 애니메이션 재생
        stateManager.PlayIdle();

        float timer = 0f;
        float lookInterval = stateManager.SearchDuration / 4f;
        int directionIndex = 0;

        Vector2[] directions = { Vector2.down, Vector2.right, Vector2.up, Vector2.left };

        while (timer < stateManager.SearchDuration)
        {
            // 일정 시간마다 방향을 바꾼다
            directionIndex = Mathf.FloorToInt(timer / lookInterval);
            directionIndex = Mathf.Clamp(directionIndex, 0, directions.Length - 1);

            stateManager.MoveDirection = directions[directionIndex];
            stateManager.SetAnimatorDirection(directions[directionIndex]);
            stateManager.RotateHitBoxes();

            // 탐색 중 플레이어 재발견
            if (stateManager.IsPlayerVisible() == true)
            {
                stateManager.ChangeState(new ChaseState());
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void MoveToward(EnemyStateManager stateManager, Vector2 destination)
    {
        Vector2 direction = (destination - (Vector2)stateManager.transform.position).normalized;

        stateManager.MoveDirection = direction;
        stateManager.Rigidbody.linearVelocity = direction * stateManager.MoveSpeed;
        stateManager.SetAnimatorDirection(direction);
        stateManager.RotateHitBoxes();
    }

    private bool HasArrived(EnemyStateManager stateManager, Vector2 destination)
    {
        return Vector2.Distance(stateManager.transform.position, destination) <= ArrivalDistance;
    }
}
