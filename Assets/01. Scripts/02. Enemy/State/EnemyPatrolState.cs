using UnityEngine;
using System.Collections;

namespace WatchYourBack
{
    public class EnemyPatrolState : IEnemyState
    {
        public void Enter(EnemyStateManager manager) 
        {
        
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            Vector2 targetPos = manager.IsGoingToPatrolPoint == true
                ? (Vector2)manager.PatrolPosition.position
                : manager.SpawnPosition;

            while (Vector2.Distance(manager.transform.position, targetPos) > 0.1f)
            {
                if (manager == null)
                {
                    yield break;
                }

                if (manager.CheckPlayerInSight() == true)
                {
                    manager.ChangeState(new EnemyChaseState());
                    yield break;
                }

                Vector2 direction = (targetPos - (Vector2)manager.transform.position).normalized;
                manager.Move(direction);
                

                yield return null;
            }

            // 목적지 도착 시 다음 목적지 반전 및 대기 상태로
            manager.IsGoingToPatrolPoint = (manager.IsGoingToPatrolPoint == false);
            manager.ChangeState(new EnemyIdleState());
        }

        public void Exit(EnemyStateManager manager) => manager.Stop();
    }
}

