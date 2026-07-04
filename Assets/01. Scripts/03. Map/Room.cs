using UnityEngine;

namespace WatchYourBack
{
    /// <summary>
    /// 방 하나를 표현하는 스크립트입니다
    /// 방 번호, 플레이어 시작 위치, 보스, 진입 문을 관리합니다
    /// </summary>
    public class Room : MonoBehaviour
    {
        [Header("= 방 정보 =")]
        [SerializeField] private int roomIndex;             // 방 순서 번호 (0부터 시작)
        [SerializeField] private Transform playerSpawnPoint; // 이 방으로 이어할 때 플레이어가 서는 위치
        [SerializeField] private GameObject bossObject;      // 이 방의 보스
        [SerializeField] private Collider2D doorCollider;    // 이 방 진입을 막는 문 콜라이더

        public int RoomIndex => roomIndex;
        public Transform PlayerSpawnPoint => playerSpawnPoint;

        /// <summary>
        /// 이미 클리어한 방으로 판단될 때 호출합니다
        /// 보스를 비활성화하고 문을 열어(콜라이더 끄기) 그냥 지나가게 합니다
        /// </summary>
        public void DeactivateAsAlreadyCleared()
        {
            if (bossObject != null)
            {
                bossObject.SetActive(false);
            }

            if (doorCollider != null)
            {
                doorCollider.enabled = false;
            }
        }
    }
}