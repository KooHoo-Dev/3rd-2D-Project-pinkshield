using UnityEngine;

namespace WatchYourBack
{
    public class EnemyAttackHitBox : MonoBehaviour
    {

        private EnemyStateManager manager;

        private void Awake()
        {
            manager = GetComponentInParent<EnemyStateManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.CompareTag("Player") == false) return;

            //플레이어가 먼저 뒤통수를 친 경우 무시
            if (manager != null && manager.TryLockCombat() == false) return;

            
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.KnockDown();
            }
            
        }
    }
}


