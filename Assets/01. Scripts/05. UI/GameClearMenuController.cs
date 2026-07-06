using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace WatchYourBack
{
    /// <summary>
    /// 게임 클리어 화면의 메뉴를 관리하는 스크립트
    /// 방향키(위/아래)로 메뉴를 이동하고, 스페이스바로 선택을 확정합니다
    /// </summary>
    public class GameClearMenuController : MonoBehaviour
    {
        // 메뉴 개수 (0: 게임메뉴로 돌아가기, 1: 게임 종료)
        private const int MenuCount = 2;

        [Header("= 메뉴 선택지 텍스트들 =")]
        [SerializeField] private GameObject gameMenuText; // "Menu" 텍스트 오브젝트
        [SerializeField] private GameObject exitText;     // "Exit" 텍스트 오브젝트

        [Header("= 선택 표시 밝기 =")]
        [SerializeField] private float selectedAlpha = 1f;
        [SerializeField] private float unselectedAlpha = 0.4f;

        [Header("= 커서 이동/선택 효과음 =")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip cursorMoveSound;
        [SerializeField] private AudioClip selectSound;

        // 현재 커서가 위치한 메뉴 번호 (0~1)
        private int currentMenuIndex = 0;

        /// <summary>
        /// 이 화면이 활성화될 때마다 커서를 맨 위(Menu)로 초기화합니다
        /// </summary>
        private void OnEnable()
        {
            currentMenuIndex = 0;
            UpdateMenuHighlight();
        }

        private void Update()
        {
            HandleMenuInput();
        }

        // ============================================================================
        // 아래 HandleMenuInput()은 다른 메뉴 스크립트에도 그대로 복사해서 재사용 가능합니다.
        // 복사해서 쓰실 땐 스크립트마다 MenuCount, currentMenuIndex, UpdateMenuHighlight(),
        // SelectCurrentMenu(), PlayCursorMoveSound(), PlaySelectSound()가 함께 있어야 합니다.
        // ============================================================================

        /// <summary>
        /// 위/아래 방향키로 메뉴 이동, 스페이스바로 메뉴 선택을 처리합니다
        /// </summary>
        private void HandleMenuInput()
        {
            if (Keyboard.current == null) return; // 예외 처리: 키보드 장치가 연결되지 않은 경우

            if (Keyboard.current.downArrowKey.wasPressedThisFrame == true)
            {
                currentMenuIndex = (currentMenuIndex + 1) % MenuCount;
                PlayCursorMoveSound();
                UpdateMenuHighlight();
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame == true)
            {
                currentMenuIndex = (currentMenuIndex - 1 + MenuCount) % MenuCount;
                PlayCursorMoveSound();
                UpdateMenuHighlight();
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                PlaySelectSound();
                SelectCurrentMenu();
            }
        }

        // ============================================================================

        /// <summary>
        /// 현재 선택된 메뉴만 밝게, 나머지는 어둡게 표시해 커서 역할을 합니다
        /// </summary>
        private void UpdateMenuHighlight()
        {
            SetTextAlpha(gameMenuText, currentMenuIndex == 0 ? selectedAlpha : unselectedAlpha);
            SetTextAlpha(exitText, currentMenuIndex == 1 ? selectedAlpha : unselectedAlpha);
        }

        /// <summary>
        /// 텍스트 오브젝트의 투명도를 설정합니다 (TextMeshPro, 일반 Image 둘 다 지원)
        /// </summary>
        private void SetTextAlpha(GameObject textObject, float alpha)
        {
            if (textObject == null) return; // 예외 처리: Inspector 연결 누락

            TMPro.TextMeshProUGUI tmpText = textObject.GetComponent<TMPro.TextMeshProUGUI>();
            if (tmpText != null)
            {
                Color color = tmpText.color;
                color.a = alpha;
                tmpText.color = color;
                return;
            }

            UnityEngine.UI.Image image = textObject.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }
        }

        /// <summary>
        /// 현재 커서가 위치한 메뉴에 맞는 기능을 실행합니다
        /// </summary>
        private void SelectCurrentMenu()
        {
            switch (currentMenuIndex)
            {
                case 0:
                    OnGameMenuSelected();
                    break;

                case 1:
                    OnExitSelected();
                    break;
            }
        }

        /// <summary>
        /// 게임 메뉴(타이틀 화면)로 돌아갑니다
        /// </summary>
        private void OnGameMenuSelected()
        {
            SceneManager.LoadScene("GameMenu");
        }

        /// <summary>
        /// 게임 프로그램을 종료합니다
        /// </summary>
        private void OnExitSelected()
        {
            Application.Quit();
        }

        /// <summary>
        /// 커서 이동 효과음을 재생합니다
        /// </summary>
        private void PlayCursorMoveSound()
        {
            if (audioSource == null || cursorMoveSound == null) return; // 예외 처리: 미연결 시 조용히 무시

            audioSource.PlayOneShot(cursorMoveSound);
        }

        /// <summary>
        /// 메뉴 선택 효과음을 재생합니다
        /// </summary>
        private void PlaySelectSound()
        {
            if (audioSource == null || selectSound == null) return; // 예외 처리: 미연결 시 조용히 무시

            audioSource.PlayOneShot(selectSound);
        }
    }
}