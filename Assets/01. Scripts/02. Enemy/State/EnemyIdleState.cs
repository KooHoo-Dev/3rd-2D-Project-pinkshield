using UnityEngine;
using System.Collections;

namespace WatchYourBack
{
    public class EnemyIdleState : IEnemyState
    {
        private const float IdleDuration = 3.0f;
        private const float LookInterval = 1.0f; // 시선 변경

        public void Enter(EnemyStateManager manager) => manager.Stop();

        public IEnumerator Execute(EnemyStateManager manager)
        {
            float totalTimer = 0f;
            float lookTimer = 0f;
            

            while (true)
            {
                if (manager == null)
                {
                    yield break;
                }

                
                if (totalTimer >= LookInterval)
                {
                    manager.ChangeState(new EnemyPatrolState());
                    yield break;
                }

                totalTimer += Time.deltaTime;
                lookTimer += Time.deltaTime;

                if (lookTimer >= LookInterval)
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
            
        }

        public void Exit(EnemyStateManager manager) { }
    }
}


