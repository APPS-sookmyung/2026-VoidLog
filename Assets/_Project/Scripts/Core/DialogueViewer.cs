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

    private PlayerMovement playerMovement;

    private void Awake()
    {
        EnsurePlayerReference();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void EnsurePlayerReference()
    {
        if (playerMovement == null)
        {
            playerMovement = FindAnyObjectByType<PlayerMovement>();
        }
    }

    // 대사창 열릴 때
     public void OpenPanel()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        // 대사창이 열리면 이동 차단
        PlayerMovement.Instance?.setCanMove(false);
    }

    public void ClosePanel()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        // 대사창이 닫히면 이동 허용
        PlayerMovement.Instance?.setCanMove(true);
    }
    public void ShowText(string speaker, string sentence)
    {
        if (speakerText != null)
            speakerText.text = speaker;

        fullSentence = sentence;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentenceCoroutine(sentence));
    }

    private IEnumerator TypeSentenceCoroutine(string sentence)
    {
        IsTyping = true;
        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char letter in sentence)
        {
            if (dialogueText != null)
                dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        IsTyping = false;
    }

    public void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (dialogueText != null)
            dialogueText.text = fullSentence;

        IsTyping = false;
    }
}