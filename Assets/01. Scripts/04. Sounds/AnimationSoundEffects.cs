using UnityEngine;

namespace WatchYourBack
{
    public class AnimationSoundEffects : MonoBehaviour
    {
        private const float MinSoundInterval = 0.1f;
        private const float FootstepVolumeScale = 0.7f;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip attackSound;
        [SerializeField] private AudioClip footstepSound;

        private IKnockDownStatus knockDownStatus;
        private Camera mainCamera;

        private float lastPlayTime = -999f;

        private void Awake()
        {
            knockDownStatus = GetComponentInParent<IKnockDownStatus>();
            mainCamera = Camera.main; // 메인 카메라 지정
        }

        public void PlayAttackSound()
        {
            if (audioSource == null || attackSound == null) return;

            float timeSinceLastPlay = Time.time - lastPlayTime;
            if (timeSinceLastPlay < MinSoundInterval) return;
            if (knockDownStatus != null && knockDownStatus.IsKnockDown == true) return;
            if (IsVisibleFromCamera() == false) return; // 화면 밖이면 재생하지 않음

            lastPlayTime = Time.time;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound);
        }

        public void PlayWalkSound()
        {

            if (audioSource == null || footstepSound == null) return;
            if (IsVisibleFromCamera() == false) return;

            // 효과음을 겹쳐서 깔끔하게 재생

            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound, FootstepVolumeScale);

        }

        /// <summary>
        /// 이 오브젝트가 현재 메인 카메라의 화면 범위 안에 보이는지 확인합니다
        /// </summary>
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


