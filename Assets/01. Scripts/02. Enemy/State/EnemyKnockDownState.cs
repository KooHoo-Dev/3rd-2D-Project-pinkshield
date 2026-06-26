using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyKnockDownState : IEnemyState
{
    public void Enter(EnemyStateManager manager)
    {
        manager.Stop();
        manager.Anim.SetTrigger("OnKnockDown"); // 애니메이터 파라미터 호출
    }

    public IEnumerator Execute(EnemyStateManager manager)
    {
        // 2초 동안 쓰러진 연출 대기
        yield return new WaitForSeconds(2.0f);
        
        // 연출 종료 후 캐릭터 제거 또는 비활성화
        manager.gameObject.SetActive(false); // 
    }

    public void Exit(EnemyStateManager manager) { }
}
}
