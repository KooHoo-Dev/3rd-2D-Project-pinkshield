using UnityEngine;

namespace WatchYourBack
{
    public class AnimationSoundEffects_Slap : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource; // 소리를 낼 스피커
        [SerializeField] private AudioClip slapSound;   // 재생할 휘두름 소리


        public void PlaySlapSound()
        {
            
            if (audioSource == null || slapSound == null)
            {
                return;
            }

            // 효과음을 겹쳐서 깔끔하게 재생
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(slapSound); 
        }
    }




}
