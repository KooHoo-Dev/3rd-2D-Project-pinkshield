using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace WatchYourBack
{
    /// <summary>
    /// 게임오버 화면의 메뉴를 관리하는 스크립트
    /// 방향키(위/아래)로 메뉴를 이동하고, 스페이스바로 선택을 확정합니다
    /// </summary>
    public class GameOverUIController : MonoBehaviour
    {
        // 메뉴 개수 (0: 처음부터 시작, 1: 이어하기, 2: 게임메뉴로 돌아가기)
        private const int MenuCount = 3;

        [Header("= 메뉴 선택지 텍스트들 =")]
        [SerializeField] private GameObject restartText;   // "처음부터 시작" 텍스트 오브젝트
        [SerializeField] private GameObject continueText;  // "이어하기" 텍스트 오브젝트
        [SerializeField] private GameObject mainMenuText;  // "게임메뉴로 돌아가기" 텍스트 오브젝트

        [Header("= 선택 표시 밝기 =")]
        [SerializeField] private float selectedAlpha = 1f;      // 선택된 텍스트의 밝기(불투명도)
        [SerializeField] private float unselectedAlpha = 0.4f;  // 선택 안 된 텍스트의 밝기

        // 현재 커서가 위치한 메뉴 번호 (0~2)
        private int currentMenuIndex = 0;

        /// <summary>
        /// 게임오버 화면이 활성화될 때마다 커서를 맨 위(처음부터 시작)로 초기화합니다
        /// </summary>
        private void OnEnable()
        {
            currentMenuIndex = 0;
            UpdateMenuHighlight();
        }

        /// <summary>
        /// 매 프레임 키 입력을 확인합니다
        /// </summary>
        private void Update()
        {
            HandleMenuInput();
        }

        /// <summary>
        /// 위/아래 방향키로 메뉴 이동, 스페이스바로 메뉴 선택을 처리합니다
        /// </summary>
        private void HandleMenuInput()
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame == true)
            {
                currentMenuIndex = (currentMenuIndex + 1) % MenuCount;
                UpdateMenuHighlight();
            }

            if (Keyboard.current.upArrowKey.wasPressedThisFrame == true)
            {
                currentMenuIndex = (currentMenuIndex - 1 + MenuCount) % MenuCount;
                UpdateMenuHighlight();
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                SelectCurrentMenu();
            }
        }

        /// <summary>
        /// 현재 선택된 메뉴만 밝게, 나머지는 어둡게 표시해 커서 역할을 합니다
        /// </summary>
        private void UpdateMenuHighlight()
        {
            SetTextAlpha(restartText, currentMenuIndex == 0 ? selectedAlpha : unselectedAlpha);
            SetTextAlpha(continueText, currentMenuIndex == 1 ? selectedAlpha : unselectedAlpha);
            SetTextAlpha(mainMenuText, currentMenuIndex == 2 ? selectedAlpha : unselectedAlpha);
        }

        /// <summary>
        /// 텍스트 오브젝트의 투명도를 설정합니다 (TextMeshPro, 일반 Image 둘 다 지원)
        /// </summary>
        private void SetTextAlpha(GameObject textObject, float alpha)
        {
            if (textObject == null) return;

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
                    RestartCurrentScene();
                    break;

                case 1:
                    ContinueFromLastRoom();
                    break;

                case 2:
                    LoadMainMenu();
                    break;
            }
        }

        /// <summary>
        /// 인 게임을 처음부터 다시 시작합니다 (현재 씬을 재로드)
        /// </summary>
        public void RestartCurrentScene()
        {
            GameProgressManager.Instance.ResetProgress();

            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        /// <summary>
        /// 가장 최근 처치한 보스 기준, 다음 방에서부터 이어서 시작합니다
        /// (진행 상황을 기억하는 별도 시스템 필요 - 아래 답변 참고)
        /// </summary>
        public void ContinueFromLastRoom()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }

        /// <summary>
        /// 인게임을 벗어나 메인 게임 메뉴(타이틀 화면)로 돌아갑니다
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene("GameMenu");
        }
    }
}