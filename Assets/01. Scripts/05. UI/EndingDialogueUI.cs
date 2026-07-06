using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace WatchYourBack
{
    /// <summary>
    /// 엔딩 대화에 등장하는 인물 종류
    /// </summary>
    public enum EndingSpeaker
    {
        Player,
        Darkerman,
        Alleyman,
        Watchman,
        Ratman
    }

    /// <summary>
    /// 대화 한 줄 (누가 말하는지 + 대사 내용)
    /// </summary>
    [System.Serializable]
    public class EndingDialogueLine
    {
        public EndingSpeaker speaker;

        [TextArea(2, 4)]
        public string text;
    }

    /// <summary>
    /// 엔딩 씬의 대화를 순서대로 재생합니다
    /// 말하는 대상의 초상화만 밝게, 나머지는 어둡게 표시하며 스페이스바로 다음 대사로 넘어갑니다
    /// 대사가 모두 끝나면 게임 클리어 메뉴를 켭니다
    /// </summary>
    public class EndingDialogueUI : MonoBehaviour
    {   


        private const float ActiveAlpha = 1f;   // 지금 말하는 대상의 밝기
        private const float DimAlpha = 0.0f;   // 말하지 않는 대상의 밝기


        [Header("= 화면 페이드 (선택 사항) =")]
        [SerializeField] private SceneFadeIn sceneFadeIn;


        [Header("= 초상화 이미지들 =")]
        [SerializeField] private Image playerImage;
        [SerializeField] private Image darkermanImage;
        [SerializeField] private Image alleymanImage;
        [SerializeField] private Image watchmanImage;
        [SerializeField] private Image ratmanImage;

        [Header("= 하단 대사창 =")]
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("= 이름 표시 (플레이어만 실제 이름 사용) =")]
        [SerializeField] private string playerDisplayName = "Mahogany";

        [Header("= 대사 목록 (순서대로 재생) =")]
        [SerializeField] private List<EndingDialogueLine> lines;

        [Header("= 대화 종료 후 켤 오브젝트 =")]
        [SerializeField] private GameObject gameClearMenu;

        private int currentLineIndex = 0;
        private bool canAcceptInput = false; // 추가: 페이드 끝나기 전엔 입력 무시
        private void Start()
        {
            // 예외 처리: 대사가 하나도 등록되지 않았다면 바로 클리어 메뉴로 넘어감
            if (lines == null || lines.Count == 0)
            {
                Debug.LogWarning("[EndingDialogueUI] 등록된 대사가 없습니다");
                EndDialogue();
                return;
            }

            ShowLine(0);

            // 페이드 연출이 연결되어 있다면, 페이드가 끝난 뒤에야 입력을 받도록 대기
            if (sceneFadeIn != null)
            {
                sceneFadeIn.OnFadeComplete += HandleFadeComplete;
            }
            else
            {
                canAcceptInput = true; // 페이드가 없으면 바로 입력 허용
            }
        }

        private void Update()
        {
            if (canAcceptInput == false) return;
            if (Keyboard.current == null) return; // 예외 처리: 키보드 장치 없음

            if (Keyboard.current.spaceKey.wasPressedThisFrame == true)
            {
                AdvanceLine();
            }
        }

        /// <summary>
        /// 다음 대사로 넘어갑니다. 마지막 대사였다면 대화를 종료합니다
        /// </summary>
        private void AdvanceLine()
        {
            currentLineIndex++;

            if (currentLineIndex >= lines.Count)
            {
                EndDialogue();
                return;
            }

            ShowLine(currentLineIndex);
        }

        /// <summary>
        /// 지정한 순번의 대사를 화면에 표시하고, 말하는 대상의 초상화를 밝힙니다
        /// </summary>
        private void ShowLine(int index)
        {
            EndingDialogueLine line = lines[index];

            speakerNameText.text = GetDisplayName(line.speaker);
            dialogueText.text = line.text;

            HighlightSpeaker(line.speaker);
        }

       


        /// <summary>
        /// 화면에 표시할 이름을 가져옵니다 (플레이어는 별도 이름, 보스는 종류명 그대로 사용)
        /// </summary>
        private string GetDisplayName(EndingSpeaker speaker)
        {
            if (speaker == EndingSpeaker.Player) return playerDisplayName;
            return speaker.ToString();
        }

        /// <summary>
        /// 말하고 있는 대상만 밝게, 나머지는 전부 어둡게 표시합니다
        /// </summary>
        private void HighlightSpeaker(EndingSpeaker speaker)
        {
            SetImageAlpha(playerImage, speaker == EndingSpeaker.Player ? ActiveAlpha : DimAlpha);
            SetImageAlpha(darkermanImage, speaker == EndingSpeaker.Darkerman ? ActiveAlpha : DimAlpha);
            SetImageAlpha(alleymanImage, speaker == EndingSpeaker.Alleyman ? ActiveAlpha : DimAlpha);
            SetImageAlpha(watchmanImage, speaker == EndingSpeaker.Watchman ? ActiveAlpha : DimAlpha);
            SetImageAlpha(ratmanImage, speaker == EndingSpeaker.Ratman ? ActiveAlpha : DimAlpha);
        }

        /// <summary>
        /// 이미지의 투명도를 설정합니다 (연결되지 않은 이미지는 조용히 건너뜁니다)
        /// </summary>
        private void SetImageAlpha(Image image, float alpha)
        {
            if (image == null) return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        /// <summary>
        /// 대화를 종료하고, 대화 UI를 끈 뒤 게임 클리어 메뉴를 켭니다
        /// </summary>
        private void EndDialogue()
        {
            gameObject.SetActive(false);

            if (gameClearMenu != null)
            {
                gameClearMenu.SetActive(true);
            }
        }

        private void HandleFadeComplete()
        {
            canAcceptInput = true;
        }
    }
}