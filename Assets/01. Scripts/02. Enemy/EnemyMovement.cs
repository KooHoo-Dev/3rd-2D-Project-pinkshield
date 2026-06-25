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
        [SerializeField] private Transform PatrolPoint;

        

        
        private Rigidbody2D rb;
        private Animator anim;
        private Transform player;
        private Vector2 originPosition;
        private bool isChasing = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();
            PatrolPoint = GetComponent<Transform>();
        }

        private void Update()
        {
            enemyIdle();
            enemyPatrol();


            enemyMove();
        }

        private void enemyMove()
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


        private void enemyIdle()
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool(isMoving, false);
        }

        private void enemyPatrol()
        {
            Vector3 patrolPoint;
            originPosition = transform.position;
            patrolPoint = (PatrolPoint.position - (Vector3)originPosition).normalized * -1f;

            rb.linearVelocity = patrolPoint * speed;
            anim.SetBool(isMoving, true);
            anim.SetFloat(moveX, patrolPoint.x);
            anim.SetFloat(moveY, patrolPoint.y);
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

