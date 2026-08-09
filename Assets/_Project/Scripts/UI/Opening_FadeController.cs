using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidLog.UI
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private float defaultDuration = 1.5f;

        private Coroutine currentFadeRoutine;

        private void Awake()
        {
            if (fadeImage == null)
            {
                Debug.LogWarning("[FadeController] fadeImage가 연결되지 않았습니다. Inspector에서 연결해주세요.");
                return;
            }

            SetAlphaImmediate(1f);
        }

        public void FadeIn(float duration = -1f, System.Action onComplete = null)
        {
            float d = duration > 0 ? duration : defaultDuration;
            StartFade(1f, 0f, d, onComplete);
        }

        public void FadeOut(float duration = -1f, System.Action onComplete = null)
        {
            float d = duration > 0 ? duration : defaultDuration;
            StartFade(0f, 1f, d, onComplete);
        }

        public void FadeInEvent()
        {
            FadeIn();
        }

        public void FadeOutEvent()
        {
            FadeOut();
        }

        private void StartFade(float from, float to, float duration, System.Action onComplete)
        {
            if (currentFadeRoutine != null)
            {
                StopCoroutine(currentFadeRoutine);
            }
            currentFadeRoutine = StartCoroutine(FadeRoutine(from, to, duration, onComplete));
        }

        private IEnumerator FadeRoutine(float from, float to, float duration, System.Action onComplete)
        {
            if (fadeImage == null) yield break;

            float elapsed = 0f;
            SetAlphaImmediate(from);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                SetAlphaImmediate(Mathf.Lerp(from, to, t));
                yield return null;
            }

            SetAlphaImmediate(to);
            currentFadeRoutine = null;
            onComplete?.Invoke();
        }

        private void SetAlphaImmediate(float alpha)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;

            fadeImage.raycastTarget = alpha > 0f;
        }
    }
}