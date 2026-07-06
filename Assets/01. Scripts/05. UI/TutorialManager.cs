using UnityEngine;
using UnityEngine.InputSystem;

namespace WatchYourBack
{
    /// <summary>
    /// 게임 시작 시 한 번만 재생되는 튜토리얼 UI를 관리합니다
    /// 스페이스바로 다음 페이지로 넘어가며, 마지막 페이지 이후 자동으로 꺼집니다
    /// "처음부터 시작"이나 "이어하기"로 들어온 경우에는 표시되지 않습니다
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [Header("= 튜토리얼 페이지들 (보여줄 순서대로 등록) =")]
        [SerializeField] private GameObject[] tutorialPages;

        [Header("= 플레이어 =")]
        [SerializeField] private PlayerMovement player;


        // 현재 보여주고 있는 페이지 번호
        private int currentPageIndex = 0;

        private void Start()
        {
            // 예외 처리 1: 이미 튜토리얼을 본 적이 있다면 아예 실행하지 않음
            if (ShouldShowTutorial() == false)
            {
                gameObject.SetActive(false);
                return;
            }

            // 예외 처리 2: 페이지가 하나도 등록되지 않았다면 조용히 꺼짐 (기획 실수로 게임이 멈추지 않도록)
            if (tutorialPages == null || tutorialPages.Length == 0)
            {
                Debug.LogWarning("[TutorialManager] 등록된 튜토리얼 페이지가 없습니다");
                gameObject.SetActive(false);
                return;
            }

            LockPlayerInput(true);

            // 에디터에서 실수로 여러 페이지가 동시에 켜져 있어도 항상 첫 페이지부터 정확히 시작하도록 초기화
            DeactivateAllPages();
            ShowPage(0);
        }

        /// <summary>
        /// 지금 튜토리얼을 보여줘야 하는 상황인지 판단합니다
        /// </summary>
        private bool ShouldShowTutorial()
        {
            // 예외 처리 3: 진행 상황 매니저를 못 찾으면, 안전하게 "보여주는 쪽"을 기본값으로 함
            if (GameProgressManager.Instance == null) return true;

            return GameProgressManager.Instance.HasSeenTutorial == false;
        }

        private void Update()
        {
            // 예외 처리 4: 키보드 장치가 연결되어 있지 않은 특수한 상황 대비
            if (Keyboard.current == null) return;

            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                AdvanceToNextPage();
            }
        }



        /// <summary>
        /// 플레이어 입력 잠금을 설정합니다. player 참조가 비어있으면 안전하게 무시합니다
        /// </summary>
        private void LockPlayerInput(bool isLocked)
        {
            if (player == null)
            {
                Debug.LogWarning("[TutorialManager] player 참조가 연결되지 않았습니다");
                return;
            }

            player.SetInputLocked(isLocked);
        }




        /// <summary>
        /// 다음 페이지로 넘어갑니다. 마지막 페이지였다면 튜토리얼을 종료합니다
        /// </summary>
        private void AdvanceToNextPage()
        {
            HidePage(currentPageIndex);

            currentPageIndex++;

            if (currentPageIndex >= tutorialPages.Length)
            {
                EndTutorial();
                return;
            }

            ShowPage(currentPageIndex);
        }

        /// <summary>
        /// 등록된 페이지를 전부 꺼서 초기 상태로 만듭니다
        /// </summary>
        private void DeactivateAllPages()
        {
            foreach (GameObject page in tutorialPages)
            {
                if (page != null) // 예외 처리 5: 인스펙터에 빈 슬롯이 있어도 죽지 않게
                {
                    page.SetActive(false);
                }
            }
        }

        private void ShowPage(int pageIndex)
        {
            GameObject page = tutorialPages[pageIndex];
            if (page == null) return;

            page.SetActive(true);
        }

        private void HidePage(int pageIndex)
        {
            GameObject page = tutorialPages[pageIndex];
            if (page == null) return;

            page.SetActive(false);
        }

        /// <summary>
        /// 튜토리얼을 종료하고, 다시 보지 않도록 기록을 남긴 뒤 전체 UI를 끕니다
        /// </summary>
        private void EndTutorial()
        {
            if (GameProgressManager.Instance != null)
            {
                GameProgressManager.Instance.MarkTutorialSeen();
            }

            LockPlayerInput(false);
            gameObject.SetActive(false); // BackgroundPanel 포함 전체가 꺼짐
        }
    }
}