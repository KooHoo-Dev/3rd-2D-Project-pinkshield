using System.Collections;
using UnityEngine;

// 공격 상태 — Attack 애니메이션 재생 후 쿨타임 대기
// 플레이어가 전방/측면 공격 시 → 반격(Counter Attack)
// 공격 후 플레이어가 생존하면 → Chase로 복귀
public class AttackState : IEnemyState
{
    // 플레이어에게 전방 공격을 당했는지 외부에서 설정하는 플래그
    public static bool ReceivedFrontalHit = false;

    public void Enter(EnemyStateManager stateManager)
    {
        stateManager.Rigidbody.linearVelocity = Vector2.zero;
        stateManager.RotateAttackHitBox();
        ReceivedFrontalHit = false;

        // Blend Tree — 공격 상태
        stateManager.PlayAttack();
    }

    public IEnumerator Execute(EnemyStateManager stateManager)
    {   
         
        
        yield return new WaitForSeconds(stateManager.AttackCooldown);

        if (ReceivedFrontalHit == true)
        {
            yield return CounterAttack(stateManager);
            yield break;
        }

        if (stateManager.Target != null && stateManager.Target.gameObject.activeSelf == true)
        {
            stateManager.ChangeState(new ChaseState());
            yield break;
        }
        else stateManager.ChangeState(new ChaseState());
    }

    public void Exit(EnemyStateManager stateManager)
    {
        ReceivedFrontalHit = false;
        Debug.Log("[AttackState] Exit");
        
    }

    // 전방 공격을 받은 경우 딜레이 후 반격 공격을 수행한다
    private IEnumerator CounterAttack(EnemyStateManager stateManager)
    {
        yield return new WaitForSeconds(stateManager.CounterAttackDelay);

        stateManager.PlayAttack();
        yield return new WaitForSeconds(stateManager.AttackCooldown);

        stateManager.ChangeState(new ChaseState());
    }
}
