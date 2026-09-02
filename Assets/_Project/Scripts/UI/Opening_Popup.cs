using TMPro;
using UnityEngine;

namespace VoidLog.UI
{
    /// <summary>
    /// 타이핑 없이 대사가 즉시 팝업처럼 바로 나타나는 텍스트 컨트롤러.
    ///
    /// 세팅 방법 (지금, CSV 연동 전):
    /// 1. TMP 텍스트 오브젝트에 붙인다.
    /// 2. presetLine에 임시로 보여줄 대사를 적어둔다.
    /// 3. OpeningStepManager의 onStepStart에서 (해당 패널 SetActive(true)와 함께) ShowPreset()을 연결한다.
    ///
    /// CSV 연동 후:
    /// - textKey에 CSV의 행 식별자(예: "step2_system_log")를 적어둔다.
    /// - CSV 로더가 이 오브젝트를 찾아 Show(csv에서_읽은_문자열)을 호출하도록 만들면 된다.
    ///   즉 presetLine/ShowPreset()은 로더가 아직 없을 때의 임시 경로이고,
    ///   Show(string)이 실제 최종 진입점이라 나중에 구조를 바꿀 필요가 없다.
    /// </summary>
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
