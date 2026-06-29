using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyKnockDownState : IEnemyState
{
    public void Enter(EnemyStateManager manager)
    {
        manager.Stop();
        manager.KnockdownCutscnene.SetActive(true);
         // 애니메이터 파라미터 호출
    }

    public IEnumerator Execute(EnemyStateManager manager)
    {
            
            // 2초 동안 쓰러진 연출 대기
            yield return new WaitForSeconds(1.8f);
            manager.KnockdownCutscnene.SetActive(false);
            
            // 연출 종료 후 캐릭터 제거 또는 비활성화

            yield return new WaitForSeconds(0.3f);
            manager.Anim.SetTrigger("OnKnockDown");

            yield return new WaitForSeconds(0.9f);
            manager.gameObject.SetActive(false); // 
    }

    public void Exit(EnemyStateManager manager) { }
}
}
