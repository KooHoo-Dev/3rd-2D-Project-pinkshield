using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyChaseState : IEnemyState
    {
        private float _lostTimer = 0f;
        private Vector2 _lastSeenPosition;

        public void Enter(EnemyStateManager manager)
        {

            Debug.Log("추적 상태 시작");
            _lostTimer = 0f;
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            while (true)
            {
                bool canSeePlayer = manager.CheckPlayerInSight();

                if (canSeePlayer == true)
                {
                    // 1. 시야 내에 있으면 타이머 리셋 및 위치 갱신
                    _lostTimer = 0f;
                    _lastSeenPosition = manager.Player.position;

                    float distance = Vector2.Distance(manager.transform.position, _lastSeenPosition);

                    // 2. 공격 사거리 우선 체크
                    if (manager.CheckPlayerInAttackRange() == true)
                    {
                        // 공격 실행 (실행 중에는 이 루프가 여기서 대기함)
                        manager.ChangeState(new EnemyAttackState());
                        yield break;

                        // 공격이 끝나면 바로 다음 루프에서 시야와 거리를 재측정하여 추격 재개
                    }
                    // 3. 사거리 밖이면 정지 거리 확인 후 이동
                    else if (distance > manager.StopDistance)
                    {
                        Vector2 direction = (_lastSeenPosition - (Vector2)manager.transform.position).normalized;
                        manager.Move(direction);
                        manager.UpdateBoxRotations(); // 애니메이터 기반 회전 갱신
                    }
                    else
                    {
                        manager.Stop();
                    }
                }
                else
                {
                    // 4. 시야에서 놓쳤을 때만 마지막 위치로 3초간 이동
                    _lostTimer += Time.deltaTime;

                    Vector2 direction = (_lastSeenPosition - (Vector2)manager.transform.position).normalized;
                    manager.Move(direction);
                    manager.UpdateBoxRotations();

                    if (_lostTimer >= 3.0f)
                    {
                        manager.ChangeState(new EnemyIdleState());
                        yield break;
                    }
                }

                yield return null;
            }
        }

        

        public void Exit(EnemyStateManager manager) => manager.Stop();
    }
}