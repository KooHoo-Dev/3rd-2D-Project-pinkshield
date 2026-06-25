using System.Collections;
using UnityEngine;

// 대기 상태 — 제자리에서 랜덤 방향을 바라보며 2초 대기한다
// 플레이어를 발견하면 → Chase / 미발견이면 → Patrol
public class IdleState : IEnemyState
{
    private const float IdleDuration = 2f;

    // 랜덤으로 선택할 수 있는 4방향
    private static readonly Vector2[] Directions =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    public void Enter(EnemyStateManager stateManager)
    {
        stateManager.Rigidbody.linearVelocity = Vector2.zero;

        // 랜덤 방향으로 히트박스와 시야 콜라이더를 회전시킨다
        Vector2 randomDirection = GetRandomDirection();
        stateManager.MoveDirection = randomDirection;
        stateManager.SetAnimatorDirection(randomDirection);
        stateManager.RotateHitBoxes();

        // Blend Tree — 정지 상태
        stateManager.PlayIdle();
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            // 플레이어 발견 시 즉시 Chase로 전환
            if (stateManager.IsPlayerVisible() == true)
            {
                stateManager.ChangeState(new ChaseState());
                yield break;
            }

            // 2초가 지나면 순찰로 전환
            if (timer >= IdleDuration)
            {
                stateManager.ChangeState(new PatrolState());
                yield break;
            }

            yield return null;
        }
    }

    public void Exit(EnemyStateManager stateManager)
    {
        Debug.Log("[IdleState] Exit");
    }

    private Vector2 GetRandomDirection()
    {
        int index = Random.Range(0, Directions.Length);
        return Directions[index];
    }
}
