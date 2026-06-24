using UnityEngine;
using UnityEngine.Animations;
using Study.Utilities;
using UnityEngine.InputSystem;




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
        [SerializeField] private Collider2D HitBox;
        
        private Vector2 movement;
        private Rigidbody2D rb;
        private Animator animator;
        


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
            //enemyStateManager = GetComponent<EnemyStateManager>();
            // 같은 게임오브젝트에 있는 컴포넌트가 아니라서 null임
            
            if(animator == null)
            {
                Debug.LogError("Animator를 찾을 수 없다!");

            }
        }

       private void Update()
        {
            Attack();
            MovementControl();


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

        private void Attack()
        {


            if (SimpleInput.GetKeyDown(Key.Z))
            {
                animator.SetBool(isAttack, true);
            }
            else if (SimpleInput.GetKeyUp(Key.Z))
            {
                animator.SetBool(isAttack, false);
            }                

        }

       

        
    }
}

