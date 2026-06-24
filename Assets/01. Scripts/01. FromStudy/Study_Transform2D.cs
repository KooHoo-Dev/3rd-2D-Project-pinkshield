using Study.Utilities;
using UnityEngine;

namespace Study_2D_DirectionAndRotation
{
    public class Study_Transform2D : MonoBehaviour
    {
        [field:SerializeField] private Transform Cursor { get; set; }

        private void Update()
        {
            // 삼각형을 회전 시키는 방법을 알아봅시다.

            // 바라보게할 위치
            Vector3 goalPosition = Cursor.position;
            // 내 위치
            Vector3 myPosition = transform.position;

            // 방향벡터 구하기 (목표 위치 - 내 위치)
            Vector3 dir = (goalPosition - myPosition).normalized;
            transform.up = dir;
        }
    }
}


