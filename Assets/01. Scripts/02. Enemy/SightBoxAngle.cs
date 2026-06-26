using UnityEngine;

namespace WatchYourBack
{
    public class SightBoxAngle : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 7f; // 회전 속도 조절
        private Rigidbody2D enemyRb;

        private void Awake()
        {
            enemyRb = GetComponentInParent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (enemyRb == null) return;

            Vector2 velocity = enemyRb.linearVelocity;

            // 정지 상태(임계값 이하)일 때는 회전을 업데이트하지 않아 마지막 방향 유지 [5, 6]
            if (velocity.sqrMagnitude < 0.01f) return;

            
            
            if (Mathf.Abs(velocity.x) > 0.01f)
            {
                float targetZangle = (velocity.x > 0) ? 0f : 180f;
                
            }

            // 2. 부드러운 회전 (시야 박스의 localRotation 조절)
            float targetZAngle = 0f;

            // Y축 이동량이 더 클 때만 상/하(90도/-90도)를 바라봅니다.
            if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
            {
                targetZAngle = (velocity.y > 0) ? 90f : -90f;
            }

            // 목표 쿼터니언 생성
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetZAngle);

            // Slerp를 사용하여 상하 시야만 부드럽게 전환
            transform.localRotation = Quaternion.Slerp(transform.localRotation,targetRotation,Time.fixedDeltaTime * rotationSpeed);

        }

    }
}


