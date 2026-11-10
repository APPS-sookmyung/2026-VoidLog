using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace VoidLog.UI
{
    public class NotePopup : MonoBehaviour
    {
        [SerializeField] private RectTransform panelTransform;
        [SerializeField] private TMP_Text noteText;

        [TextArea(3, 8)]
        [SerializeField] private string presetNoteContent;

        [SerializeField] private float showDuration = 0.4f;
        [SerializeField] private float hideDuration = 0.25f;
        [SerializeField]
        private AnimationCurve showEase = new AnimationCurve(
            new Keyframe(0f, 0f), new Keyframe(0.7f, 1.1f), new Keyframe(1f, 1f));

        public UnityEvent onShown;
        public UnityEvent onHidden;

        private Coroutine routine;

        private void Awake()
        {
            if (panelTransform == null)
            {
                panelTransform = GetComponent<RectTransform>();
            }
            panelTransform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            Show(presetNoteContent);
        }

        public void Show(string content)
        {
            gameObject.SetActive(true);
            if (noteText != null)
            {
                noteText.text = content;
            }

            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ScaleRoutine(1f, showDuration, showEase, () => onShown?.Invoke()));
        }

        public void Hide()
        {
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(ScaleRoutine(0f, hideDuration, null, () =>
            {
                gameObject.SetActive(false);
                onHidden?.Invoke();
            }));
        }

        private IEnumerator ScaleRoutine(float to, float duration, AnimationCurve curve, System.Action onComplete)
        {
            float from = panelTransform.localScale.x;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float scale = curve != null ? curve.Evaluate(t) : Mathf.Lerp(from, to, t);
                panelTransform.localScale = Vector3.one * scale;
                yield return null;
            }

            panelTransform.localScale = Vector3.one * to;
            routine = null;
            onComplete?.Invoke();
        }
    }
}