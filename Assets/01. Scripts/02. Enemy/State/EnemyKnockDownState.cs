using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyKnockDownState : IEnemyState
    {
        
        public void Enter(EnemyStateManager manager)
        {
            manager.Stop();
            
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {

            yield return manager.PlayKnockDownRoutine(true);
        }

        public void Exit(EnemyStateManager manager) { }
    }
}
