using System.Collections;
using UnityEngine;


namespace WatchYourBack
{
    public class EnemyMovement : MonoBehaviour
    {
        private const string moveX = "MoveX";
        private const string moveY = "MoveY";
        private const string isMoving = "IsMoving";
        private const string isAttack = "IsAttack";


        [Header("Enemy Settings")]
        [field: SerializeField] private float speed = 3.0f;
        [field: SerializeField] private Transform PatrolPosition;


        private Rigidbody2D rb;
        private Animator anim;
        private Transform player;
        private Transform EnemySpawnPoint;
        private bool isChasing = false;
        private bool isPatroling = false;
        private float idleDuration = 2.0f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();

        }

        private void Update()
        {
            //enemyIdle();
            //enemyPatrol();
            enemyChasing();



        }

        private void enemyChasing()
        {
            if (isChasing == true)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
                anim.SetBool(isMoving, true);
                anim.SetFloat(moveX, direction.x);
                anim.SetFloat(moveY, direction.y);
            }
        }


        private IEnumerator enemyIdle()
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool(isMoving, false);

            yield return null;
        }

        private void enemyPatrol()
        {
            isPatroling = true;

            if (isPatroling == true)
            {
                EnemySpawnPoint = gameObject.transform;

                Vector2 patrolPoint = (PatrolPosition.position - EnemySpawnPoint.position).normalized;

                rb.linearVelocity = patrolPoint * speed;
                anim.SetBool(isMoving, true);
                anim.SetFloat(moveX, patrolPoint.x);
                anim.SetFloat(moveY, patrolPoint.y);
            }




        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") == true)
            {
                if (player == null)
                {
                    player = collision.transform;
                }
                isChasing = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isChasing = false;
                rb.linearVelocity = Vector2.zero;
                anim.SetBool(isMoving, false);
            }
        }
    }


}

