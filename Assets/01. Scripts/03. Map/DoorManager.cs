using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace WatchYourBack

{
    public class DoorManager : MonoBehaviour
    {
        // 각 보스들이 처치(비활성화) 될때마다 각 방의 BoxCollider 들이 비활성화되어 열림

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip doorOpenSound;

        [Header("Cameras")]
        [SerializeField] private CinemachineCamera firstDoorCam;
        [SerializeField] private CinemachineCamera secondDoorCam;
        [SerializeField] private CinemachineCamera thirdDoorCam;


        [Header("EnemyList")]
        [SerializeField] private GameObject Alleyman;
        [SerializeField] private GameObject Watchman;
        [SerializeField] private GameObject Ratman;
        [SerializeField] private GameObject Darkerman;

        [Header("RoomDoors")]
        [SerializeField] private GameObject FirstDoor; //Alleyman 처치 시 비활성화
        [SerializeField] private GameObject SecondDoor; //Watchman 처치 시 비활성화
        [SerializeField] private GameObject ThirdDoor; //Ratman 처치 시 비활성화

        private float doorTime = 2.0f;


        private void Update()
        {
            // 보스가 죽었을 때 연출 실행
            if (Alleyman != null && !Alleyman.activeSelf && FirstDoor.GetComponent<BoxCollider2D>().enabled)
            {
                StartCoroutine(OpenDoorSequence(FirstDoor, firstDoorCam));
            }
        }

        private IEnumerator OpenDoorSequence(GameObject door, CinemachineCamera doorCam)
        {
            // 1. 효과음 재생 
            audioSource.PlayOneShot(doorOpenSound);

            // 2. 문 카메라 우선순위 높여서 화면 전환 
            doorCam.Priority = 20;

            // 3. 문 열기 (콜라이더 비활성화)
            door.GetComponent<BoxCollider2D>().enabled = false;

            // 4. 2초간 문을 보여준 뒤 다시 플레이어에게 복귀 
            yield return new WaitForSeconds(doorTime);
            doorCam.Priority = 0;
        }



    }

}



