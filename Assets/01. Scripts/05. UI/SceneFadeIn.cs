using System;
using System.Collections;
using UnityEngine;

namespace WatchYourBack
{
    /// <summary>
    /// 씬 시작 시 화면을 검게 덮은 상태에서 서서히 밝아지는 페이드인 연출을 담당합니다
    /// </summary>
    public class SceneFadeIn : MonoBehaviour
    {
        [Header("= 페이드 설정 =")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 1.0f;

        private void Start()
        {
            // 예외 처리: 패널이 연결 안 되어 있으면 페이드 없이 즉시 종료
            if (fadeCanvasGroup == null)
            {
                Debug.LogWarning("[SceneFadeIn] fadeCanvasGroup이 연결되지 않았습니다");
                return;
            }

            fadeCanvasGroup.alpha = 1f; // 씬 시작과 동시에 완전히 검게
            fadeCanvasGroup.blocksRaycasts = true;

            StartCoroutine(FadeInRoutine());
        }

        /// <summary>
        /// 알파값을 1(완전 검정)에서 0(완전 투명)까지 서서히 낮춥니다
        /// </summary>
        private IEnumerator FadeInRoutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fadeDuration;

                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);

                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false; // 페이드 끝나면 입력 차단 해제

            OnFadeComplete?.Invoke();
        }

        /// <summary>
        /// 페이드가 완전히 끝났을 때 호출되는 콜백입니다 (대화 시작 등에 연결)
        /// </summary>
        public event Action OnFadeComplete;
    }
}