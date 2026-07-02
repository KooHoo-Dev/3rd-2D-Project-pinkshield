using UnityEngine;

namespace WatchYourBack
{
    public class EnemyAttackHitBox : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 플레이어의 캡슐 콜라이더(HurtBox 역할)와 접촉 확인
            if (collision.CompareTag("Player") == true)
            {
                PlayerMovement player = collision.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.KnockDown();
                }
            }
        }
    }
}


