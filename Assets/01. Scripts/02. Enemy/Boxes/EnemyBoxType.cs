using UnityEngine;

namespace WatchYourBack
{

    public enum EnemyBoxType
    {
        Sight,
        AttackRange,
        BackHurt

    }


    public class EnemyBoxTrigger : MonoBehaviour
    {

        [SerializeField] private EnemyBoxType boxType;
        [SerializeField] private EnemyStateManager manager;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") == false) return;

            manager.OnBoxTriggerEnter(boxType, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") == false) return;

            manager.OnBoxTriggerExit(boxType, collision);
        }


    }
}


