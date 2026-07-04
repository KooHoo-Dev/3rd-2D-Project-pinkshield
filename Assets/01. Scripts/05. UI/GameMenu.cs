using Study.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace WatchYourBack
{
    /// <summary>
    /// 게임 메뉴 관리 스크립트
    /// 메뉴의 시작 화면부터 선택까지의 전체 흐름을 담당합니다.
    /// </summary>
    public class GameMenu : SingletonBase<GameMenu>
    {
        // =================== 인스펙터에 할당할 오브젝트들 ===================

        [Header("= Boss 설정 =")]
        [SerializeField] private GameObject[] bosses; // 첫 화면에 순서대로 나타날 보스들

        [Header("= 타이틀 화면 요소들 =")]
        [SerializeField] private GameObject mainPlayer;      // 메인 플레이어 이미지
        [SerializeField] private GameObject title;           // 게임 타이틀 이미지
        [SerializeField] private GameObject pressEnter;      // "Press Enter" 텍스트
        [SerializeField] private GameObject menu;            // 메뉴 패널

        
        [Header("= 메뉴 선택지 텍스트들 =")]
        [SerializeField] private GameObject gameStartText;   // 게임시작 텍스트
        [SerializeField] private GameObject optionText;      // 옵션 텍스트
        [SerializeField] private GameObject exitText;        // 종료 텍스트

        
        [Header("= 효과 설정값들 =")]
        [SerializeField] private float bossFadeInDuration = 0.5f;      // Boss 페이드인 지속 시간
        [SerializeField] private float pulseSpeed = 2f;                // 반짝임 속도 (빠를수록 빨리 반짝임)
        [SerializeField] private float minAlpha = 0.3f;                // 반짝임 최소 투명도
        [SerializeField] private float maxAlpha = 1f;                  // 반짝임 최대 투명도
        [SerializeField] private float delayBetweenElements = 0.5f;    // 요소 간 나타나는 시간 간격
        [SerializeField] private float bossDuration = 0.4f;            // 각 보스가 나타나있는 시간

        [Header("= 오디오 설정 =")]
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private float bgmFadeOutDuration = 1.0f; // 씬 전환 시 페이드아웃 시간


        // =================== 게임 상태 정의 ===================
        private enum GameState
        {
            Initializing,       // 게임 초기화 중
            ShowingBosses,      // 보스 이미지 표시 중
            ShowingTitle,       // 타이틀 표시 중
            WaitingForPressKey, // Enter 키 입력 대기
            MenuActive          // 메뉴 화면 활성화
        }

        // =================== 필드 변수들 ===================
        private GameState currentState = GameState.Initializing; // 현재 게임 상태
        private int currentMenuIndex = 0;                        // 현재 선택된 메뉴 (0: 시작, 1: 옵션, 2: 종료)
        private const int MENU_COUNT = 3;                        // 메뉴 개수

        // 코루틴 참조 (나중에 중지하기 위해 저장)
        private Coroutine pressEnterPulseCoroutine;
        private Coroutine menuTextPulseCoroutine;

        // =================== Unity 생명주기 함수 ===================

        /// <summary>
        /// 싱글톤 초기화 시 호출되는 함수
        /// </summary>
        protected override void OnInitialize()
        {
            
            // 모든 UI 요소 숨기기
            AllElementsDeactivate();

            // 배경음악 재생
            PlayBackgroundMusic(); 

            // 전체 흐름 시작
            StartCoroutine(MenuSequence());
        }

        /// <summary>
        /// 매 프레임마다 호출되는 함수
        /// </summary>
        private void Update()
        {
            // 메뉴 화면이 활성화됐을 때만 입력을 처리
            if (currentState == GameState.MenuActive)
            {
                HandleMenuInput();
            }
        }

        // =================== 초기화 함수들 ===================

        /// <summary>
        /// 모든 게임 오브젝트를 비활성화합니다
        /// </summary>
        private void AllElementsDeactivate()
        {
            // 모든 보스 비활성화
            foreach (GameObject boss in bosses)
            {
                boss.SetActive(false);
            }

            // 다른 UI 요소들 비활성화
            mainPlayer.SetActive(false);
            title.SetActive(false);
            pressEnter.SetActive(false);
            menu.SetActive(false);

            // 메뉴 텍스트들 비활성화
            gameStartText.SetActive(false);
            optionText.SetActive(false);
            exitText.SetActive(false);

            
        }

        /// <summary>
        /// 메뉴 배경음악을 재생합니다
        /// </summary>
        private void PlayBackgroundMusic()
        {
            if (bgmAudioSource == null || bgmClip == null) return;

            bgmAudioSource.clip = bgmClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }





        // =================== 메인 시퀀스 함수 ===================

        /// <summary>
        /// 게임 메뉴의 전체 흐름을 관리하는 코루틴
        /// </summary>
        private IEnumerator MenuSequence()
        {
            // [0단계] 보스들 순서대로 나타내기
            currentState = GameState.ShowingBosses;
            yield return ShowBossesSequentially();

            // [1단계] 타이틀과 메인플레이어 표시
            currentState = GameState.ShowingTitle;
            yield return new WaitForSeconds(delayBetweenElements);
            mainPlayer.SetActive(true);

            yield return new WaitForSeconds(delayBetweenElements);
            title.SetActive(true);

            // [2단계] "Press Enter" 표시 및 반짝임 효과
            currentState = GameState.WaitingForPressKey;
            yield return new WaitForSeconds(delayBetweenElements);
            pressEnter.SetActive(true);

            // PressEnter 반짝이기 시작 (Enter 키 입력까지 계속 반복)
            pressEnterPulseCoroutine = StartCoroutine(PulseElement(pressEnter, pulseSpeed, minAlpha, maxAlpha));

            // Enter 키 대기
            yield return WaitForEnterKeyPress();

            // PressEnter 반짝임 중지
            if (pressEnterPulseCoroutine != null)
            {
                StopCoroutine(pressEnterPulseCoroutine);
            }

            // PressEnter 숨기기
            pressEnter.SetActive(false);

            // [3단계] 메뉴 화면 활성화
            currentState = GameState.MenuActive;
            ActivateMenuScreen();

            // 메뉴 종료까지 대기 (메뉴 선택 시 다른 곳에서 상태 변경됨)
            yield return null;
        }

        // =================== 단계별 동작 함수들 ===================

        /// <summary>
        /// 보스들을 순서대로 나타냅니다 (각각 fade-in 효과 포함)
        /// </summary>
        private IEnumerator ShowBossesSequentially()
        {
            foreach (GameObject boss in bosses)
            {
                boss.SetActive(true);

                // Boss를 천천히 페이드인 (알파값 0 -> 1로 변경)
                StartCoroutine(FadeInElement(boss, bossFadeInDuration));

                // 다음 보스 나타나기 전까지 대기
                yield return new WaitForSeconds(bossDuration);
            }
        }

        /// <summary>
        /// Enter 키 입력을 대기합니다
        /// </summary>
        private IEnumerator WaitForEnterKeyPress()
        {
            bool enterPressed = false;

            while (enterPressed == false)
            {
                // 키보드 입력 시스템으로 Enter 키 감지
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    enterPressed = true;
                }

                yield return null; // 다음 프레임 대기
            }
        }

        /// <summary>
        /// 메뉴 화면을 활성화합니다
        /// </summary>
        private void ActivateMenuScreen()
        {
            // 메뉴 패널 활성화
            menu.SetActive(true);

            
            // 메뉴 텍스트들 모두 활성화
            gameStartText.SetActive(true);
            optionText.SetActive(true);
            exitText.SetActive(true);

            // 메뉴 초기화
            currentMenuIndex = 0;
            UpdateMenuDisplay();
        }

        // =================== 메뉴 입력 처리 함수들 ===================

        /// <summary>
        /// 메뉴 화면에서의 키 입력을 처리합니다 (방향키, Space)
        /// </summary>
        private void HandleMenuInput()
        {
            // 아래 방향키: 다음 메뉴로 이동
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                currentMenuIndex = (currentMenuIndex + 1) % MENU_COUNT;
                UpdateMenuDisplay();
            }

            // 위 방향키: 이전 메뉴로 이동
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                currentMenuIndex = (currentMenuIndex - 1 + MENU_COUNT) % MENU_COUNT;
                UpdateMenuDisplay();
            }

            // Enter 키: 메뉴 선택
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                SelectMenu(currentMenuIndex);
            }
        }

        /// <summary>
        /// 메뉴 표시를 업데이트합니다 (커서 위치, 텍스트 반짝임)
        /// </summary>
        private void UpdateMenuDisplay()
        {
            // 선택된 메뉴에 따라 커서 위치 변경
            switch (currentMenuIndex)
            {
                case 0: // 게임 시작
                     break;

                case 1: // 옵션
                    break;

                case 2: // 종료
                    break;
            }

            // 선택된 텍스트만 반짝이게 하기
            UpdateAllMenuTexts();
        }

        /// <summary>
        /// 모든 메뉴 텍스트의 반짝임 효과를 업데이트합니다
        /// </summary>
        private void UpdateAllMenuTexts()
        {
            // 이전 반짝임 중지
            if (menuTextPulseCoroutine != null)
            {
                StopCoroutine(menuTextPulseCoroutine);
            }

            // 선택되지 않은 텍스트들은 낮은 알파로 설정
            SetElementAlpha(gameStartText, currentMenuIndex == 0 ? maxAlpha : minAlpha);
            SetElementAlpha(optionText, currentMenuIndex == 1 ? maxAlpha : minAlpha);
            SetElementAlpha(exitText, currentMenuIndex == 2 ? maxAlpha : minAlpha);

            // 선택된 텍스트만 반짝임
            GameObject selectedText = GetSelectedMenuText();
            if (selectedText != null)
            {
                menuTextPulseCoroutine = StartCoroutine(PulseElement(selectedText, pulseSpeed, minAlpha, maxAlpha));
            }
        }

        /// <summary>
        /// 현재 선택된 메뉴 텍스트를 반환합니다
        /// </summary>
        private GameObject GetSelectedMenuText()
        {
            switch (currentMenuIndex)
            {
                case 0:
                    return gameStartText;
                case 1:
                    return optionText;
                case 2:
                    return exitText;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 메뉴를 선택합니다 (Space 키 입력 시)
        /// </summary>
        private void SelectMenu(int menuIndex)
        {
            Debug.Log($"메뉴 선택됨: {menuIndex}");

            switch (menuIndex)
            {
                case 0:
                    OnGameStartSelected();
                    break;

                case 1:
                    OnOptionSelected();
                    break;

                case 2:
                    OnExitSelected();
                    break;
            }
        }

        // =================== 메뉴 선택 시 동작 함수들 ===================

        /// <summary>
        /// 게임 시작 메뉴 선택 시 호출
        /// </summary>
        private void OnGameStartSelected()
        {
            Debug.Log("게임 시작!");
            StartCoroutine(FadeOutBgmAndLoadScene(1, bgmFadeOutDuration));
        }

        /// <summary>
        /// 옵션 메뉴 선택 시 호출
        /// </summary>
        private void OnOptionSelected()
        {
            Debug.Log("옵션 화면 열기");
            // TODO: 옵션 화면 로직 구현
        }

        /// <summary>
        /// 종료 메뉴 선택 시 호출
        /// </summary>
        private void OnExitSelected()
        {
            Debug.Log("게임 종료");
            Application.Quit();
        }

        // =================== 효과 함수들 (Coroutine) ===================


        /// <summary>
        /// 오브젝트를 천천히 페이드인합니다 (알파값 0 -> 1)
        /// </summary>
        private IEnumerator FadeInElement(GameObject element, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // 0부터 1로 진행되는 값 계산
                float progress = elapsedTime / duration;
                progress = Mathf.Clamp01(progress); // 0~1 범위로 제한

                SetElementAlpha(element, progress);

                yield return null; // 다음 프레임 대기
            }

            // 최종 알파값 확실히 1로 설정
            SetElementAlpha(element, 1f);
        }

        /// <summary>
        /// 오브젝트를 반짝이게 합니다 (알파값이 계속 변함)
        /// </summary>
        private IEnumerator PulseElement(GameObject element, float speed, float minAlpha, float maxAlpha)
        {
            while (true)
            {
                // 시간에 따라 0 ~ 1 ~ 0으로 반복되는 값 생성
                float pingPongValue = Mathf.PingPong(Time.time * speed, 1f);

                // minAlpha와 maxAlpha 사이에서 진동
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, pingPongValue);

                SetElementAlpha(element, alpha);

                yield return null; // 다음 프레임 대기
            }
        }

        /// <summary>
        /// 배경음악을 서서히 줄이면서, 페이드가 끝난 후 씬을 전환합니다
        /// </summary>
        private IEnumerator FadeOutBgmAndLoadScene(int sceneIndex, float fadeDuration)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            if (bgmAudioSource != null)
            {
                yield return FadeOutBgm(fadeDuration);
            }

            asyncLoad.allowSceneActivation = true;
        }

        /// <summary>
        /// BGM 볼륨을 현재값에서 0까지 서서히 낮춥니다
        /// </summary>
        private IEnumerator FadeOutBgm(float duration)
        {
            float startVolume = bgmAudioSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, progress);

                yield return null;
            }

            bgmAudioSource.volume = 0f;
            bgmAudioSource.Stop();
        }





        // =================== 유틸리티 함수들 ===================

        /// <summary>
        /// 오브젝트의 알파값을 설정합니다
        /// CanvasGroup, Image, TextMeshProUGUI, SpriteRenderer를 지원합니다
        /// </summary>
        private void SetElementAlpha(GameObject element, float alpha)
        {
            if (element == null) return;

            // 1. CanvasGroup으로 알파값 설정 시도
            CanvasGroup canvasGroup = element.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                return;
            }

            // 2. Image로 알파값 설정 시도
            UnityEngine.UI.Image image = element.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
                return;
            }

            // 3. TextMeshProUGUI로 알파값 설정 시도
            TMPro.TextMeshProUGUI tmpText = element.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                Color color = tmpText.color;
                color.a = alpha;
                tmpText.color = color;
                return;
            }

            // 4. SpriteRenderer로 알파값 설정 시도
            SpriteRenderer spriteRenderer = element.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
    }
}
