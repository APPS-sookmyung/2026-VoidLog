using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace VoidLog.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterText : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;

        [TextArea(2, 6)]
        [SerializeField] private string presetLine;

        [SerializeField] private float charDelay = 0.05f;

        private Coroutine typingRoutine;

        private void Awake()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            targetText.text = string.Empty;
        }

        public void ShowPreset()
        {
            Show(presetLine);
        }

        public void Show(string text)
        {
            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
            }
            typingRoutine = StartCoroutine(TypeRoutine(text ?? string.Empty));
        }

        public void Clear()
        {
            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
                typingRoutine = null;
            }
            targetText.text = string.Empty;
        }

        private IEnumerator TypeRoutine(string text)
        {
            var builder = new StringBuilder();

            foreach (char c in text)
            {
                builder.Append(c);
                targetText.text = builder.ToString();
                yield return new WaitForSeconds(charDelay);
            }

            typingRoutine = null;
        }
    }
}