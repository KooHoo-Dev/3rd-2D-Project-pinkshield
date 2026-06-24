using Study.Utilities;
using UnityEngine;

namespace Study_2D_DirectionAndRotation
{
    public class Study_WorldMousePoint : MonoBehaviour
    {
        private void Update()
        {
            // 화면 좌표계 에서 World(Scene)의 좌표계로 전환합니다.
            // 전환할때는 Camera의 기능을 이용합니다.

            Vector2 mousePosition = SimpleInput.GetMousePosition();
            Vector3 worldMousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePoint.z = transform.position.z; // z값 보정 해줍니다

            // 전환된 위치를 대입합니다.
            transform.position = worldMousePoint;
        }
    }

}

