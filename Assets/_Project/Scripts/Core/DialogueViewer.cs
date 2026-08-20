using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueViewer : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("연출 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    public bool IsTyping { get; private set; } = false;
    private Coroutine typingCoroutine;
    private string fullSentence;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void OpenPanel() => dialoguePanel.SetActive(true);
    public void ClosePanel() => dialoguePanel.SetActive(false);

    public void ShowText(string speaker, string sentence)
    {
        speakerText.text = speaker;
        fullSentence = sentence;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentenceCoroutine(sentence));
    }

    private IEnumerator TypeSentenceCoroutine(string sentence)
    {
        IsTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    /// 타이핑 중일 때 누르면 즉시 전체 문장 표출 (스킵)
    public void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = fullSentence;
        IsTyping = false;
    }
}