using System.Collections;
using UnityEngine;


namespace WatchYourBack
{
    
    public class EnemyChaseState : IEnemyState
    {
        private float _lostTimer = 0f;
        private Vector2 _lastSeenPosition;
        private bool _isAttacking = false;

        public void Enter(EnemyStateManager manager)
        {
            _lostTimer = 0f;
            _isAttacking = false;
        }

        public IEnumerator Execute(EnemyStateManager manager)
        {
            while (true)
            {
                if (_isAttacking == false)
                {
                    bool canSeePlayer = manager.CheckPlayerInSight();

                    if (canSeePlayer == true)
                    {
                        _lostTimer = 0f;
                        _lastSeenPosition = manager.Player.position;

                        float distance = Vector2.Distance(manager.transform.position, _lastSeenPosition);

                        // 정지 거리보다 멀 때만 이동 (지터링 방지 핵심) [1]
                        if (distance > manager.StopDistance)
                        {
                            Vector2 direction = (_lastSeenPosition - (Vector2)manager.transform.position).normalized;
                            manager.Move(direction);
                            manager.UpdateBoxRotations(direction); // 방향 업데이트 호출
                        }
                        else
                        {
                            manager.Stop(); // 가까워지면 제자리 대기
                        }

                        // 공격 범위 체크는 이동 여부와 상관없이 수행
                        if (manager.CheckPlayerInAttackRange() == true)
                        {
                            yield return manager.StartCoroutine(PerformAttack(manager));
                        }
                    }
                    else
                    {
                        // ... (3초간 마지막 위치 추적 로직은 동일) ...
                    }
                }
                yield return null;
            }
        }

        private IEnumerator PerformAttack(EnemyStateManager manager)
        {
            _isAttacking = true;
            manager.Stop();
            manager.SetAttackAnimation(true);

            yield return new WaitForSeconds(0.8f); // 공격 쿨타임 [3]

            manager.SetAttackAnimation(false);
            _isAttacking = false;
        }

        public void Exit(EnemyStateManager manager) => manager.Stop();
    }
}

