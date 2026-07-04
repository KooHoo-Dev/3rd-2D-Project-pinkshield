using UnityEngine;

namespace WatchYourBack
{
    public class AnimationSoundEffects : MonoBehaviour
    {
        private const float MinSoundInterval = 0.1f;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip attackSound;

        private IKnockDownStatus knockDownStatus;

        private float lastPlayTime = -999f;

        private void Awake()
        {
            knockDownStatus = GetComponentInParent<IKnockDownStatus>();
        }

        public void PlayAttackSound()
        {
            if (audioSource == null || attackSound == null) return;

            float timeSinceLastPlay = Time.time - lastPlayTime;
            if (timeSinceLastPlay < MinSoundInterval) return;
            if (knockDownStatus != null && knockDownStatus.IsKnockDown == true) return;

            lastPlayTime = Time.time;
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound);
        }

        

    }
}


