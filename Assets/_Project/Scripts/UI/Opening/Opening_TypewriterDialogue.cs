using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace VoidLog.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterDialogueQueue : MonoBehaviour
    {
        [Header("텍스트 소스")]
        [SerializeField] private TMP_Text targetText;

        [Header("순서대로 보여줄 대사들")]
        [TextArea(2, 4)]
        [SerializeField] private List<string> lines = new List<string>();

        [Header("타이핑 속도")]
        [SerializeField] private float charDelay = 0.05f;
        [Tooltip("쉼표, 마침표, 말줄임표(…) 등에서 조금 더 머무는 시간")]
        [SerializeField] private float punctuationExtraDelay = 0.15f;

        [Header("클릭 입력")]
        [SerializeField] private bool useMouseClick = true;
        [Tooltip("한 줄이 끝난 직후 클릭이 곧바로 다음 줄로 새는 걸 막기 위한 짧은 무시 시간")]
        [SerializeField] private float inputGuardTime = 0.15f;

        [Header("이벤트")]
        public UnityEvent onLineComplete;     
        public UnityEvent onAllLinesComplete; 

        public bool IsTyping { get; private set; }
        public bool IsActive { get; private set; }

        private int currentIndex = -1;
        private Coroutine typingRoutine;
        private float lineCompleteTime;

        private void Awake()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            targetText.text = string.Empty;
        }

        private void Update()
        {
            if (!IsActive || !useMouseClick) return;
            if (!Input.GetMouseButtonDown(0)) return;

            HandleClick();
        }

        public void StartSequence()
        {
            currentIndex = -1;
            IsActive = true;
            lineCompleteTime = Time.time;
            AdvanceToNextLine();
        }

        public void HandleClick()
        {
            if (!IsActive) return;

            if (Time.time - lineCompleteTime < inputGuardTime) return;

            if (IsTyping)
            {
                SkipToEnd();
            }
            else
            {
                AdvanceToNextLine();
            }
        }

        private void AdvanceToNextLine()
        {
            currentIndex++;

            if (currentIndex >= lines.Count)
            {
                IsActive = false;
                onAllLinesComplete?.Invoke();
                return;
            }

            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
            }
            typingRoutine = StartCoroutine(TypeRoutine(lines[currentIndex]));
        }

        private IEnumerator TypeRoutine(string text)
        {
            IsTyping = true;
            targetText.text = string.Empty;
            var builder = new StringBuilder();

            foreach (char c in text)
            {
                Debug.Log($"typing char: {c}");
                builder.Append(c);
                targetText.text = builder.ToString();

                float delay = charDelay;
                if (IsPunctuation(c))
                {
                    delay += punctuationExtraDelay;
                }
                yield return new WaitForSeconds(delay);
            }

            FinishLine(text);
        }

        public void SkipToEnd()
        {
            if (!IsTyping || currentIndex < 0 || currentIndex >= lines.Count) return;

            if (typingRoutine != null)
            {
                StopCoroutine(typingRoutine);
                typingRoutine = null;
            }

            FinishLine(lines[currentIndex]);
        }

        private void FinishLine(string fullText)
        {
            targetText.text = fullText;
            IsTyping = false;
            typingRoutine = null;
            lineCompleteTime = Time.time;
            onLineComplete?.Invoke();
        }

        private bool IsPunctuation(char c)
        {
            return c == '.' || c == ',' || c == '!' || c == '?' || c == '…';
        }
    }
}
