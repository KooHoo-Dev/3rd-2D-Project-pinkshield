using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyAttackState : IEnemyState
    {
       
        private readonly float attackCooldown = 0.8f;

        public void Enter(EnemyStateManager manager)
        {   
            // 공격 시작 시 이동 정지 및 애니메이션 파라미터 설정        
            manager.Stop();
            manager.UpdateBoxRotations();
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.NotifyCombatStarted();
            }
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            if (manager == null) yield break;

            // 1. 공격 실행
            manager.Anim.SetBool("IsAttack", true);
            Debug.Log("적 일반 공격 실행");

            // 2. 공격 후 쿨타임 대기
            yield return new WaitForSeconds(attackCooldown);
            manager.Anim.SetBool("IsAttack", false);

            manager.ChangeState(new EnemyChaseState());
            yield break;
        }

        public void Exit(EnemyStateManager manager)
        {
            SoundManager.Instance.NotifyCombatEnded();
        }
    }
}

        
