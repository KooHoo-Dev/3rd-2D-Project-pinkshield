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
            manager.KnockdownCutscnene.SetActive(true);
            // 애니메이터 파라미터 호출
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {

            // 2초 동안 쓰러진 연출 대기
            yield return new WaitForSeconds(CutsceneDuration);
            manager.KnockdownCutscnene.SetActive(false);

            // 연출 종료 후 캐릭터 제거 또는 비활성화

            yield return new WaitForSeconds(PreTriggerDelay);
            manager.Anim.SetTrigger("OnKnockDown");

            yield return new WaitForSeconds(PostTriggerDelay);
            manager.gameObject.SetActive(false); // 
        }

        public void Exit(EnemyStateManager manager) { }
    }
}
