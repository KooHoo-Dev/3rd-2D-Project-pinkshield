using System.Collections;
using UnityEngine;

namespace WatchYourBack
{

    public class RatmanPatternState : IEnemyState
    {

        private readonly float arrivalConfirmDis = 0.2f; // 도착 인정거리



        public void Enter(EnemyStateManager Manager)
        {   
            // 상태 진입 시 기존 물리 속도 초기화
            Manager.Rb.linearVelocity = Vector2.zero;
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            Vector2 targetPos = manager.LastPlayerPosition;

            while (true)
            {
                if (manager == null)
                {
                    yield break;
                }


                // 도착여부 체크
                float distance = Vector2.Distance(manager.transform.position, targetPos);
                if (distance <= arrivalConfirmDis)
                {
                    break; // 목표 지점 도달했으므로 탈출
                }

                // 이동 수행

                Vector2 dir = (targetPos - (Vector2)manager.transform.position).normalized;
                manager.MoveWithAvoidance(dir);

                yield return null;

            }

            // 루프 탈출 후 행동
            manager.Stop();


        }

        public void Exit(EnemyStateManager Manager)
        {
            throw new System.NotImplementedException();
        }

        
    }

}


