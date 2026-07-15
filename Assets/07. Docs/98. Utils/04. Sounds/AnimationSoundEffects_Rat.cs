using UnityEngine;

namespace WatchYourBack

{
    public class AnimationSoundEffects_Rat : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource; // 소리를 낼 스피커
        [SerializeField] private AudioClip ratKilledSound;   // 쥐 죽는 소리
        [SerializeField] private AudioClip ratSqueaksSound;

        private const float ratstepVolumeScale = 0.4f;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main; // 메인 카메라 지정
        }


        public void PlayRatWalkSound()
        {
            if (audioSource == null || ratSqueaksSound == null) return;
            if (IsVisibleFromCamera() == false) return;

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(ratSqueaksSound);

        }

        public void PlayRatKilledSound()
        {

            if (audioSource == null || ratKilledSound == null) return;
            

            // 효과음을 겹쳐서 깔끔하게 재생

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(ratKilledSound);

        }

        private bool IsVisibleFromCamera()
        {
            if (mainCamera == null) return true; // 카메라를 못 찾으면 안전하게 항상 재생

            Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

            // z가 0보다 커야 카메라 앞쪽에 있다는 뜻 (뒤에 있으면 화면에 안 보임)
            bool isInFrontOfCamera = viewportPos.z > 0f;
            bool isInHorizontalRange = viewportPos.x >= 0f && viewportPos.x <= 1f;
            bool isInVerticalRange = viewportPos.y >= 0f && viewportPos.y <= 1f;

            return isInFrontOfCamera && isInHorizontalRange && isInVerticalRange;
        }
    }

}

