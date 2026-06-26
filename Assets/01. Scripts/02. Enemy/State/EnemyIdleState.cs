using UnityEngine;
using System.Collections;

namespace WatchYourBack
{
    public class EnemyIdleState : IEnemyState
    {
        public void Enter(EnemyStateManager manager) => manager.Stop();

        public IEnumerator Execute(EnemyStateManager manager)
        {
            float timer = 0f;
            while (timer < 2.0f)
            {
                timer += Time.deltaTime;

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


