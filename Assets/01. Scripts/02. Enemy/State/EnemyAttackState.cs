using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    public class EnemyAttackState : IEnemyState
    {
        private float _attackCooldown = 0.8f;

        public void Enter(EnemyStateManager manager)
        {
            // 공격 시작 시 이동 정지 및 애니메이션 파라미터 설정 [1]
            manager.Stop();
            manager.Anim.SetBool("IsAttack", true);
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            while (true)
            {
                // 1. 공격 실행 (애니메이션 트리거 등)
                // 히트박스 회전은 이미 manager.Move() 시점의 마지막 방향으로 고정됨 [1]
                Debug.Log("적 일반 공격 실행");

                // 2. 공격 쿨타임 대기 (0.8초) [1]
                yield return new WaitForSeconds(_attackCooldown);

                // 3. 상태 전환 판정
                // 플레이어가 공격 사거리(AttackHitbox)를 벗어났는지 확인 [1]
                if (manager.CheckPlayerInAttackRange() == false)
                {
                    if (manager.CheckPlayerInSight() == true)
                    {
                        // 시야에는 있다면 다시 추격 [1]
                        manager.ChangeState(new EnemyChaseState());
                    }
                    else
                    {
                        // 완전히 놓쳤다면 대기 상태로 [1]
                        manager.ChangeState(new EnemyIdleState());
                    }
                    yield break;
                }

                yield return null;
            }
        }

        public void Exit(EnemyStateManager manager)
        {
            manager.Anim.SetBool("IsAttack", false);
        }
    }
}