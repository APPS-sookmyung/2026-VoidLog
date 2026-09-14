using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidLog.UI
{
    public class ColorToneController : MonoBehaviour
    {
        [System.Serializable]
        public class TonePreset
        {
            public string name;
            public Color tintColor = new Color(0.2f, 0.25f, 0.35f, 0.25f); // 기본값: 청회색
        }

        [SerializeField] private Image toneImage;
        [SerializeField] private TonePreset[] presets;
        [SerializeField] private float defaultTransitionDuration = 2f;

        private Coroutine transitionRoutine;

        private void Awake()
        {
            if (toneImage == null) return;

            Color c = toneImage.color;
            c.a = 0f;
            toneImage.color = c;
            toneImage.raycastTarget = false;
        }

        public void TransitionToPreset(int index, float duration = -1f)
        {
            if (toneImage == null || presets == null || index < 0 || index >= presets.Length)
            {
                Debug.LogWarning("[ColorToneController] 잘못된 프리셋 인덱스이거나 toneImage가 비어있습니다.");
                return;
            }

            float d = duration > 0 ? duration : defaultTransitionDuration;

            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
            }
            transitionRoutine = StartCoroutine(TransitionRoutine(presets[index].tintColor, d));
        }

        public void TransitionToTone0() => TransitionToPreset(0);
        public void TransitionToTone1() => TransitionToPreset(1);
        public void TransitionToTone2() => TransitionToPreset(2);

        public void ClearToneImmediate()
        {
            if (transitionRoutine != null)
            {
                StopCoroutine(transitionRoutine);
                transitionRoutine = null;
            }
            if (toneImage == null) return;

            Color c = toneImage.color;
            c.a = 0f;
            toneImage.color = c;
        }

        private IEnumerator TransitionRoutine(Color target, float duration)
        {
            Color start = toneImage.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                toneImage.color = Color.Lerp(start, target, t);
                yield return null;
            }

            toneImage.color = target;
            transitionRoutine = null;
        }
    }
}