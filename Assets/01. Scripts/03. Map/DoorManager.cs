using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace WatchYourBack

{
    public class DoorManager : MonoBehaviour
    {
        // 각 보스들이 처치(비활성화) 될때마다 각 방의 BoxCollider 들이 비활성화되어 열림

        private const float MaxAlphaValue = 255f;
        private const int StandardCurtainAlpha = 177;
        private const int DarkerCurtainAlpha = 213;
        private const int ActiveCameraPriority = 20;
        private const int InactiveCameraPriority = 0;
        private const float DoorTime = 2.0f;


        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip doorOpenSound;

        [Header("Cameras")]
        [SerializeField] private CinemachineCamera firstDoorCam;
        [SerializeField] private CinemachineCamera secondDoorCam;
        [SerializeField] private CinemachineCamera thirdDoorCam;

        [Header("Map Curtains")]
        [SerializeField] private SpriteRenderer firstCurtain;
        [SerializeField] private SpriteRenderer secondCurtain;
        [SerializeField] private SpriteRenderer thirdCurtain;
        [SerializeField] private SpriteRenderer fourthCurtain;


        [Header("EnemyList")]
        [SerializeField] private GameObject alleyman;
        [SerializeField] private GameObject watchman;
        [SerializeField] private GameObject ratman;
        [SerializeField] private GameObject darkerman;

        [Header("RoomDoors")]
        [SerializeField] private GameObject firstDoor; //Alleyman 처치 시 비활성화
        [SerializeField] private GameObject secondDoor; //Watchman 처치 시 비활성화
        [SerializeField] private GameObject thirdDoor; //Ratman 처치 시 비활성화

        


        private void Start()
        {
            // 첫 번째 구역 장막을 177(약 0.69)값으로 즉시 변경
            SetCurtainAlpha(firstCurtain, StandardCurtainAlpha);
        }


        private void Update()
        {
            HandleBossDefeat(alleyman, firstDoor, firstDoorCam, secondCurtain, StandardCurtainAlpha);
            HandleBossDefeat(watchman, secondDoor, secondDoorCam, thirdCurtain, StandardCurtainAlpha);
            HandleBossDefeat(ratman, thirdDoor, thirdDoorCam, fourthCurtain, DarkerCurtainAlpha);
        
        }


        /// <summary>
        /// "이어하기"로 씬이 시작될 때, 이미 클리어한 방까지의 문/커튼을 연출 없이 즉시 적용합니다
        /// clearedRoomIndex: 0=Alleyman까지 클리어, 1=Watchman까지, 2=Ratman까지
        /// </summary>
        public void ApplyProgressInstantly(int clearedRoomIndex)
        {
            if (clearedRoomIndex >= 0)
            {
                OpenDoorInstantly(firstDoor);
                SetCurtainAlpha(secondCurtain, StandardCurtainAlpha);
            }

            if (clearedRoomIndex >= 1)
            {
                OpenDoorInstantly(secondDoor);
                SetCurtainAlpha(thirdCurtain, StandardCurtainAlpha);
            }

            if (clearedRoomIndex >= 2)
            {
                OpenDoorInstantly(thirdDoor);
                SetCurtainAlpha(fourthCurtain, DarkerCurtainAlpha);
            }
        }

        /// <summary>
        /// 연출(카메라, 효과음) 없이 문 콜라이더만 즉시 비활성화합니다
        /// </summary>
        private void OpenDoorInstantly(GameObject door)
        {
            if (door == null) return;

            if (door.TryGetComponent<BoxCollider2D>(out BoxCollider2D col))
            {
                col.enabled = false;
            }
        }



        private void HandleBossDefeat(GameObject enemy, GameObject door, CinemachineCamera doorCam, 
            SpriteRenderer nextCurtain, int targetAlpha)
        {   
            //보스가 비활성화되었고, 문이 아직 닫혀있는지(콜라이더 켜진) 상태인지 확인
            if (enemy != null && (enemy.activeSelf == false) && door != null)
            {
                if (door.TryGetComponent<BoxCollider2D>(out BoxCollider2D col) && col.enabled)
                {
                    SetCurtainAlpha(nextCurtain, targetAlpha);
                    StartCoroutine(OpenDoorSequence(door, doorCam));
                }
                    
            }
        }


        // 
        private void SetCurtainAlpha(SpriteRenderer curtain, int alpha255)
        {
            if (curtain == null)
            {
                return;
            }

            Color color = curtain.color;
            color.a = alpha255 / MaxAlphaValue; // 반복되는 255f를 상수로 처리
            curtain.color = color;
        }



        private IEnumerator OpenDoorSequence(GameObject door, CinemachineCamera doorCam)
        {
            // 1. 효과음 재생 
            audioSource.PlayOneShot(doorOpenSound);

            // 2. 문 카메라 우선순위 높여서 화면 전환 
            doorCam.Priority = ActiveCameraPriority;

            // 3. 문 열기 (콜라이더 비활성화)
            OpenDoorInstantly(door);

            // 4. 2초간 문을 보여준 뒤 다시 플레이어에게 복귀 
            yield return new WaitForSeconds(DoorTime);
            doorCam.Priority = InactiveCameraPriority;
        }



       



    }

}



