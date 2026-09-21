using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpeningDialogueViewer : MonoBehaviour
{
    [Header("기본 패널 (PanelType 매칭이 없을 때 사용)")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [System.Serializable]
    public class PanelStyle
    {
        [Tooltip("CSV의 PanelType 컬럼 값과 정확히 일치해야 함")]
        public string panelType;
        public GameObject panelRoot;
        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI dialogueText;
    }

    [Header("PanelType별로 다른 패널 (필요한 만큼 추가)")]
    [SerializeField] private List<PanelStyle> panelStyles = new List<PanelStyle>();

    [Header("연출 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    public bool IsTyping { get; private set; } = false;

    private Coroutine typingCoroutine;
    private string fullSentence;
    private PanelStyle activeStyle;

    private void Awake()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        foreach (var style in panelStyles)
        {
            if (style.panelRoot != null)
            {
                style.panelRoot.SetActive(false);
            }
        }
    }

    public void OpenPanel()
    {
        // 오프닝엔 플레이어 이동 제어가 필요 없으므로 비워둠.
    }

    public void ClosePanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        foreach (var style in panelStyles)
        {
            if (style.panelRoot != null)
            {
                style.panelRoot.SetActive(false);
            }
        }
    }

    public void ShowText(string speaker, string sentence)
    {
        ShowText(speaker, sentence, null);
    }

    public void ShowText(string speaker, string sentence, string panelType)
    {
        PanelStyle matched = string.IsNullOrEmpty(panelType)
            ? null
            : panelStyles.Find(s => s.panelType == panelType);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(matched == null);
        }
        foreach (var style in panelStyles)
        {
            if (style.panelRoot != null)
            {
                style.panelRoot.SetActive(style == matched);
            }
        }
        activeStyle = matched;

        TMP_Text targetSpeakerText = matched != null ? matched.speakerText : speakerText;
        TMP_Text targetDialogueText = matched != null ? matched.dialogueText : dialogueText;

        if (targetSpeakerText != null)
        {
            targetSpeakerText.text = speaker;
        }

        fullSentence = sentence;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentenceCoroutine(sentence, targetDialogueText));
    }

    private IEnumerator TypeSentenceCoroutine(string sentence, TMP_Text target)
    {
        IsTyping = true;
        if (target != null)
        {
            target.text = "";
        }

        foreach (char letter in sentence)
        {
            if (target != null)
            {
                target.text += letter;
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    public void SkipTyping()
    {
        // 클릭해도 전체 대사가 한 번에 나오지 않도록 스킵 기능을 막아둠
    }
}