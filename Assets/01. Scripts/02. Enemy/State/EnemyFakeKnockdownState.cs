using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyFakeKnockdownState : IEnemyState
    {
        public void Enter(EnemyStateManager manager)
        {
            manager.Stop();
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            yield return manager.PlayKnockDownRoutine(false); // 컷신 재생 X
        }

        public void Exit(EnemyStateManager manager)
        {
            
        }
    }
}


