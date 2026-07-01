using UnityEngine;
using System.Collections;

namespace WatchYourBack
{
    public class EnemyIdleState : IEnemyState
    {
        public void Enter(EnemyStateManager manager) => manager.Stop();

        public IEnumerator Execute(EnemyStateManager manager)
        {
            float totalTimer = 0f;
            float lookTimer = 0f;
            float lookInterval = 1.0f; // 시선변경 값

            while (totalTimer < 3.0f)
            {
                if (manager == null)
                {
                    yield break;
                }

                totalTimer += Time.deltaTime;
                lookInterval += Time.deltaTime;

                if (lookTimer >= lookInterval)
                {
                    manager.SetRandomLookAround();
                    lookTimer = 0f;
                }


                // 플레이어 발견 시 즉시 추격
                if (manager.CheckPlayerInSight() == true)
                {
                    manager.ChangeState(new EnemyChaseState());
                    yield break;
                }

                yield return null;
            }
            // 2초 지나면 순찰 시작
            manager.ChangeState(new EnemyPatrolState());
        }

        public void Exit(EnemyStateManager manager) { }
    }
}


