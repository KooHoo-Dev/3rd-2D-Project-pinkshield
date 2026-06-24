using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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

        private Rigidbody2D rb;
        private Animator anim;
        private Transform player;
        private bool isChasing = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
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

