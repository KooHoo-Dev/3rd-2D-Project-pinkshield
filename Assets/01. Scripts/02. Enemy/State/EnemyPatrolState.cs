using UnityEngine;
using System.Collections;

namespace WatchYourBack
{
    public class EnemyPatrolState : IEnemyState
    {
        private const float ArrivalThreshold = 0.1f;

        public void Enter(EnemyStateManager manager) 
        {
            Debug.Log("순찰 시작");
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {   

            Vector2 targetPos = manager.IsGoingToPatrolPoint == true
                ? (Vector2)manager.PatrolPosition.position
                : manager.SpawnPosition;

            while (true)
            {
                if (manager == null) yield break;

                if (Vector2.Distance(manager.transform.position, targetPos) <= ArrivalThreshold)
                {
                    manager.IsGoingToPatrolPoint = (manager.IsGoingToPatrolPoint == false);
                    manager.ChangeState(new EnemyIdleState());
                    yield break;
                }



                if (manager.CheckPlayerInSight() == true)
                {
                    manager.ChangeState(new EnemyChaseState());
                    yield break;
                }

                Vector2 direction = (targetPos - (Vector2)manager.transform.position).normalized;
                manager.MoveWithAvoidance(direction);
                

                yield return null;
            }

            
        }

        public void Exit(EnemyStateManager manager) => manager.Stop();
    }
}

