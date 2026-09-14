using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace VoidLog.UI
{

    public class TitleReveal : MonoBehaviour
    {
        [SerializeField] private Image titleImage;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float delayBeforeFade = 0.1f;

        public UnityEvent onRevealComplete;

        private void Awake()
        {
            if (titleImage != null)
            {
                SetAlpha(0f);
            }
        }

        public void Reveal()
        {
            StartCoroutine(RevealRoutine());
        }

        private IEnumerator RevealRoutine()
        {
            yield return new WaitForSeconds(delayBeforeFade);

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }
            SetAlpha(1f);
            onRevealComplete?.Invoke();
        }

        private void SetAlpha(float alpha)
        {
            if (titleImage == null) return;
            Color c = titleImage.color;
            c.a = alpha;
            titleImage.color = c;
        }
    }
}
