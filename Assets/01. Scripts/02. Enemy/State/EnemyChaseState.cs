using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyChaseState : IEnemyState
    {
        private const float LostTimeLimit = 3.0f;

        private float _lostTimer = 0f;
        private Vector2 _lastSeenPosition;

        public void Enter(EnemyStateManager manager)
        {
            _lostTimer = 0f;
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            while (true)
            {
                if (manager == null)
                {
                    yield break;
                }

                bool canSeePlayer = manager.CheckPlayerInSight();

                if (canSeePlayer == true)
                {
                    _lostTimer = 0f;
                    _lastSeenPosition = manager.Player.position;

                    // 공격 범위 안 → AttackState로 전환
                    if (manager.CheckPlayerInAttackRange() == true)
                    {
                        manager.ChangeState(new EnemyAttackState());
                        yield break; // ← 반드시 필요. 없으면 같은 프레임에 루프 계속 실행됨
                    }

                    // 정지 거리 밖 → 플레이어 방향으로 이동
                    float distance = Vector2.Distance(manager.transform.position, _lastSeenPosition);
                    if (distance > manager.StopDistance)
                    {
                        Vector2 direction = GetDirectionTo(manager, _lastSeenPosition);
                        manager.Move(direction);
                    }
                    else
                    {
                        manager.Stop();
                    }
                }
                else
                {
                    // 시야를 잃으면 마지막 위치로 3초간 이동 후 Idle
                    _lostTimer += Time.deltaTime;

                    Vector2 direction = GetDirectionTo(manager, _lastSeenPosition);
                    manager.Move(direction);

                    if (_lostTimer >= LostTimeLimit)
                    {
                        manager.ChangeState(new EnemyIdleState());
                        yield break;
                    }
                }

                yield return null;

                
            }
        }

        public void Exit(EnemyStateManager manager)
        {
            manager.Stop();
        }

        private Vector2 GetDirectionTo(EnemyStateManager manager, Vector2 target)
        {
            return (target - (Vector2)manager.transform.position).normalized;
        }
    }
}