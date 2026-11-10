using TMPro;
using UnityEngine;

namespace VoidLog.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class PopupText : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;

        [Header("CSV 연동용 식별자 (나중에 로더가 이 값으로 텍스트를 찾음)")]
        [SerializeField] private string textKey;

        [Header("CSV 연동 전 임시로 쓸 대사 (ShowPreset 용)")]
        [TextArea(2, 6)]
        [SerializeField] private string presetLine;

        private void Awake()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            targetText.text = string.Empty;
        }

        public string TextKey => textKey;

        /// <summary>CSV 연동 전 임시용. Inspector의 presetLine을 즉시 표시.</summary>
        public void ShowPreset()
        {
            Show(presetLine);
        }

        /// <summary>실제 최종 진입점. CSV 로더도 이 함수를 호출하면 된다.</summary>
        public void Show(string text)
        {
            targetText.text = text ?? string.Empty;
        }

        /// <summary>텍스트를 비운다 (패널을 끄기 전/다음 스텝으로 넘어갈 때 사용).</summary>
        public void Clear()
        {
            targetText.text = string.Empty;
        }
    }
}
