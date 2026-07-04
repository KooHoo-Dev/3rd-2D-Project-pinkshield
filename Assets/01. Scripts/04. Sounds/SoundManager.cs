using Study.Utilities;
using UnityEngine;

namespace WatchYourBack
{
    public class SoundManager : SingletonBase<SoundManager>
    {
        [Header("BGM 설정")]
        [SerializeField] private AudioSource bgmSource;

        [Header("SFX 설정")]
        [SerializeField] private AudioSource sfxSource;

        public void PlayBgm(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBgm()
        {
            bgmSource.Stop();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }
    }
}


