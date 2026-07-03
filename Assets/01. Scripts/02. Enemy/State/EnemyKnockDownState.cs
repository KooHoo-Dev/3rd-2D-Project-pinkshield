using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyKnockDownState : IEnemyState
    {
        private const float CutsceneDuration = 1.8f;
        private const float PreTriggerDelay = 0.3f;
        private const float PostTriggerDelay = 0.9f;



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
