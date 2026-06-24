using UnityEngine;
using UnityEngine.InputSystem;
using Study.Utilities;


namespace WatchYourBack
{
    public class PlyaerController_Only_Keyboard : MonoBehaviour
    {
        public float moveSpeed = 3f;

        private Vector2 movement;
        private float applySpeed;

        [field: SerializeField] private Rigidbody2D rb;


        #region Unity_Method

        private void FixedUpdate()
        {
            // Movement
            rb.MovePosition(rb.position + movement * applySpeed * Time.fixedDeltaTime);
        }

        private void Update()
        {
            //Input
            playerMove();



        }

        #endregion

        #region Private_Method

        private void playerMove()
        {
            movement = SimpleInput.GetMoveAxisRaw();

            bool isRun = SimpleInput.GetKey(Key.LeftShift);
            applySpeed = moveSpeed * (isRun ? 2.5f : 1.0f);
        }


        #endregion




    }

}





