using DG.Tweening;
using Study.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Study_DoTween
{
    public class DoTweenPracticeTemplate : MonoBehaviour
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Vector3 moveOffset = new Vector3(3, 0, 0);

        private SpriteRenderer sprite;
        private Vector3 startPos;
        private Color startColor;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            startPos = transform.position;
            if (sprite != null) startColor = sprite.color;
        }

        private void Update()
        {
            if (SimpleInput.GetKeyDown(Key.Digit1)) Task1_Move();
            if (SimpleInput.GetKeyDown(Key.Digit2)) Task2_Appear();
            if (SimpleInput.GetKeyDown(Key.Digit3)) Task3_CoinPickup();
            if (SimpleInput.GetKeyDown(Key.Digit4)) Task4_HitFeedback();
            if (SimpleInput.GetKeyDown(Key.Digit5)) Task5_FloatLoop();
            if (SimpleInput.GetKeyDown(Key.Digit0)) ResetAll();
        }

        private void Task1_Move()
        {
            // TODO: transform 을 (startPos + moveOffset) 으로 DOMove(…, duration).
            //       SetEase 를 Linear / OutQuad / OutBack 으로 바꿔가며 느낌확인.

            transform.DOMove(startPos + moveOffset, duration)
                .SetEase(Ease.OutQuad);

        }

        private void Task2_Appear()
        {
            // TODO: localScale 을 0으로 만든 뒤 DOScale(1, …).SetEase(Ease.OutBack)
            transform.localScale = Vector3.zero;
            transform.DOScale(3, 1.0f)
                .SetEase(Ease.OutBack);
        }

        // ── 과제 3 : 코인 획득 연출 (Sequence) ─────────────────────
        private void Task3_CoinPickup()
        {
            // TODO: DOTween.Sequence() 로
            //  1) Append(DOMoveY 위로) + Join(DOScale 커짐)   ← 동시
            //  2) Append(sprite.DOFade(0, …))                  ← 순차
            //  3) AppendCallback(() => 점수 로그 + gameObject.SetActive(false))
            //  4) SetLink(gameObject)

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMoveY(1, 1)).SetEase(Ease.OutQuad);
            seq.Join(transform.DOScale(1.4f, 0.3f));
            seq.Append(sprite.DOFade(0f, 0.3f));
            seq.AppendCallback(() => Debug.Log("코인 획득! 점수 +1 "));
            seq.OnComplete(() => Destroy(gameObject));

        }

        private void Task4_HitFeedback()
        {
            // TODO: transform.DOShakePosition(…)
            //       + sprite.DOColor(Color.red, …).SetLoops(2, LoopType.Yoyo)  ← 원래색 복귀
            transform.DOShakePosition(0.2f, 0.2f);
            sprite.DOColor(Color.red, 0.1f)
                .SetLoops(2, LoopType.Yoyo);

        }

        private void Task5_FloatLoop()
        {
            // TODO: DOMoveY 왕복을 SetLoops(-1, LoopType.Yoyo), Ease.InOutSine, SetLink(gameObject)
            transform.DOMoveY(1,duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutCirc)
                .SetLink(gameObject);

        }

        private void ResetAll()
        {
            transform.DOKill();
            transform.position = startPos;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            if (sprite != null) sprite.color = startColor;
            Debug.Log("리셋 완료");
        }

        private void OnDestroy() => transform.DOKill(); // 파괴 시 트윈 정리
    }
}