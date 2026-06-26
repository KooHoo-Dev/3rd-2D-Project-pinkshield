using UnityEngine;

namespace WatchYourBack
{
    public class EnemyBackHurtBox : MonoBehaviour
    {
        private EnemyStateManager _manager;

        private void Awake()
        {
            // 부모 객체의 매니저 참조 캐싱
            _manager = GetComponentInParent<EnemyStateManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_manager != null)
            {
                _manager.OnBackHit(collision);
            }
        }
    }
}