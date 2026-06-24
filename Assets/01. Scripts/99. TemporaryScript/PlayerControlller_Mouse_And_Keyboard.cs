using UnityEngine;
using UnityEngine.InputSystem;
using Study.Utilities;

namespace WatchOutYourBack
{
    public class PlayerControlller : MonoBehaviour
{   

    public float speed = 3.0f;

    private Rigidbody2D rBody;
    private PolygonCollider2D pCol;

    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
        pCol = GetComponent<PolygonCollider2D>();
        rBody.bodyType = RigidbodyType2D.Kinematic;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }


    public void MovePlayer()
    {
        Vector2 moveDirection = SimpleInput.GetMoveAxisRaw();
        
        bool isRun = SimpleInput.GetKey(Key.LeftShift);
        float applySpeed = speed * (isRun ? 2.5f : 1.0f);
        
        // Apply movement using both X and Y axes with the calculated speed
        Vector3 movement = new Vector3(moveDirection.x, moveDirection.y, 0) * applySpeed * Time.deltaTime;
        transform.Translate(movement);
    }
    }
}




