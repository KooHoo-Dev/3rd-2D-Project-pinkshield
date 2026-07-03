using UnityEngine;
using Study.Utilities;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;


namespace WatchYourBack
{
    public class PlayerMovement : MonoBehaviour
    {   
    
        private const string horizontal = "Horizontal";
        private const string vertical = "Vertical";
        private const string lastHorizontal = "LastHorizontal";
        private const string lastVertical = "LastVertical";
        private const string isAttack = "IsAttack";
    
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float attackCooldown = 0.8f; // 쿨타임시간
        [SerializeField] private Collider2D AttackHitBox;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject hitCutScene;

        [Header("Debug")]
        [SerializeField] private EnemyStateManager debugTargetEnemy;

               
        private Vector2 movement;
        private Rigidbody2D rb;
        private Animator animator;
        private bool isKnockDown = false;
        
        private bool canAttack = true; // 공격 가능 여부

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            
            

            if(animator == null)
            {
                Debug.LogError("Animator를 찾을 수 없다!");

            }
        }

        private void FixedUpdate()
        {
            if (isKnockDown) return; // 대쉬나 다운중에는 이동 제어권 상실
            MovementControl();
        }


        private void Update()
        {
            if (isKnockDown == true) return; // 다운 상태면 로직 중단

            Attack();
            

            if (SimpleInput.GetKeyDown(Key.F1))
            {
                Restart();
            }

            if (SimpleInput.GetKeyDown(Key.F2))
            {
                KillEnemy();
            }
                       
        }

        

        public void KnockDown()
        {   
            if (isKnockDown == true) return; // 중복 실행 방지

            isKnockDown = true;
            rb.linearVelocity = Vector2.zero; // 즉시 정지
            
            animator.SetTrigger("OnKnockDown"); // 애니메이션 재생

            StartCoroutine(KnockDownRoutine());
            
        }

        

        public void Restart()
        {   

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            SceneManager.LoadScene(currentSceneIndex);                       
        }

        public void KillEnemy()
        {
            if (debugTargetEnemy == null)
            {
                Debug.LogError("KillEnemy : debugTargetEnemy가 연결되지 않았습니다.");
                return;
            }

            debugTargetEnemy.ChangeState(new EnemyKnockDownState());
        }


        private void MovementControl()
        {
            movement.Set(InputManager.Movement.x, InputManager.Movement.y);
            rb.linearVelocity = movement * moveSpeed;

            animator.SetFloat(horizontal, movement.x);
            animator.SetFloat(vertical, movement.y);


            if (movement != Vector2.zero)
            {
                animator.SetFloat(lastHorizontal, movement.x);
                animator.SetFloat(lastVertical, movement.y);
            }

        }

        


        // attack 기능 재 수정 필요 
        private void Attack()
        {


            if (SimpleInput.GetKeyDown(Key.Z) && canAttack)
                    {
                        // 공격 쿨타임 중이 아닐 때만 트리거 실행 
                        if (SimpleInput.GetKeyDown(Key.Z) && canAttack)
                        {
                            StartCoroutine(AttackCooldownRoutine());
                        }
                    }

        }

        private IEnumerator AttackCooldownRoutine()
        {
            canAttack = false;
            animator.SetBool(isAttack, true); // 애니메이터의 전이 조건 

            yield return new WaitForSeconds(attackCooldown); // 0.5초 등 설정된 쿨타임
            animator.SetBool(isAttack, false); 
            canAttack = true;
        }



        

        private IEnumerator KnockDownRoutine()
        {
            if (hitCutScene != null)
            {
                hitCutScene.SetActive(true);
            }
            // 1. 컷신 및 애니메이션 대기 (예: 2초)
            yield return new WaitForSeconds(2f);

            hitCutScene.SetActive(false);
            gameObject.SetActive(false);

            // 2. 게임오버 UI 활성화
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
        }


    }
}

