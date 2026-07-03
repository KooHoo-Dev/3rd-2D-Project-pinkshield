using System.Collections;
using UnityEngine;

namespace WatchYourBack
{

    [RequireComponent(typeof(DarkermanIdentity))]
    public class DarkermanInvisibleEffect : MonoBehaviour
    {
        private const float InvisibleInterval = 40f;
        private const float InvisibleDuration = 0.3f;
        private const float FadedAlpha = 0.2f;

        [SerializeField] private SpriteRenderer spriteRenderer;

        private DarkermanIdentity identity;
        private float originalAlpha;

        private void Awake()
        {
            identity = GetComponent<DarkermanIdentity>();
        }

        private void Start()
        {
            if (identity.IsRealDarkerman == false)
            {
                enabled = false;
                return;
            }

            originalAlpha = spriteRenderer.color.a;
            StartCoroutine(InvisibleRoutine());
        }

        private IEnumerator InvisibleRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(InvisibleInterval);

                SetAlpha(FadedAlpha);
                yield return new WaitForSeconds(InvisibleDuration);
                SetAlpha(originalAlpha);
            }

        }

        private void SetAlpha(float alpha)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = alpha;
            spriteRenderer.color = currentColor;
        }

    }
}



