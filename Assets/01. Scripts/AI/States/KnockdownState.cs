using System.Collections;
using UnityEngine;

// 뒤통수 기습 성공 시 진입하는 상태
// 컷씬 Canvas 활성화 → 컷씬 종료 대기 → KnockDown 애니메이션 → 오브젝트 비활성화
public class KnockdownState : IEnemyState
{
    // 컷씬 재생 시간 (초) — Canvas 안의 Animator가 따로 있다면 이벤트로 대체 가능
    private const float CutsceneDuration = 2f;

    public void Enter(EnemyStateManager stateManager)
    {
        // 이동 완전 정지
        stateManager.Rigidbody.linearVelocity = Vector2.zero;

        // 컷씬 Canvas 활성화
        if (stateManager.KnockdownCanvas != null)
        {
            stateManager.KnockdownCanvas.SetActive(true);
        }
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {
        // 컷씬 재생 대기
        yield return new WaitForSeconds(CutsceneDuration);

        // Canvas 비활성화
        if (stateManager.KnockdownCanvas != null)
        {
            stateManager.KnockdownCanvas.SetActive(false);
        }

        // KnockDown 애니메이션 재생 (Trigger 방식)
        stateManager.PlayKnockdown();

        // 애니메이션이 끝날 때까지 대기
        yield return WaitForAnimationEnd(stateManager, "KnockDown");

        // 적 오브젝트 비활성화
        stateManager.gameObject.SetActive(false);
    }

    public void Exit(EnemyStateManager stateManager)
    {
        // KnockDown은 종료 후 오브젝트 자체가 꺼지므로 별도 처리 없음
        Debug.Log("[KnockdownState] Exit");
    }

    // 지정한 애니메이션 클립이 끝날 때까지 기다린다
    private IEnumerator WaitForAnimationEnd(EnemyStateManager stateManager, string animationName)
    {
        // 애니메이션이 시작될 때까지 한 프레임 대기
        yield return null;

        AnimatorStateInfo stateInfo = stateManager.Animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 클립이 재생되는 동안 대기
        while (stateInfo.IsName(animationName) == true && stateInfo.normalizedTime < 1f)
        {
            yield return null;
            stateInfo = stateManager.Animator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
