using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueViewer viewer;

    // GroupID별로 대사 목록을 보관하는 Dictionary
    private Dictionary<string, List<DialogueData>> dialogueDatabase = new Dictionary<string, List<DialogueData>>();
    
    private List<DialogueData> activeDialogueGroup = new List<DialogueData>();
    private int currentIndex = 0;
    private bool isDialogueRunning = false;
    private Action onDialogueComplete;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (viewer == null)
            viewer = GetComponent<DialogueViewer>();
    }

    private void Update()
    {
        if (!isDialogueRunning) return;

        // 클릭이나 Spacebar 입력 시
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            HandlePlayerInput();
        }
    }

    // 씬 시작 시 해당 씬의 CSV를 1회 로드하여 파싱해둠
    public void LoadDialogueDatabase(TextAsset csvFile)
    {
        dialogueDatabase.Clear();

        if (csvFile == null)
        {
            Debug.LogError("[DialogueManager] CSV 파일이 지정되지 않았습니다.");
            return;
        }

        string[] rows = csvFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄(Header) 제외하고 1번 인덱스부터 순회
        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = rows[i].Split(',');

            if (cols.Length >= 5)
            {
                string groupId = cols[0].Trim();
                int.TryParse(cols[1].Trim(), out int order);
                string speaker = cols[2].Trim();
                string dialogue = cols[3].Trim();
                string emotion = cols[4].Trim();
                string soundEffect = cols[5].Trim();

                DialogueData data = new DialogueData(groupId, order, speaker, dialogue, emotion, soundEffect);

                if (!dialogueDatabase.ContainsKey(groupId))
                {
                    dialogueDatabase[groupId] = new List<DialogueData>();
                }
                dialogueDatabase[groupId].Add(data);
            }
        }

        // 각 그룹 내 Order 기준 정렬
        foreach (var group in dialogueDatabase.Values)
        {
            group.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }

    // 특정 GroupID 대사 출력
    public void StartDialogueGroup(string groupId, Action onComplete = null)
    {
        if (!dialogueDatabase.ContainsKey(groupId))
        {
            Debug.LogWarning($"[DialogueManager] GroupID '{groupId}'에 해당하는 대사를 찾을 수 없습니다.");
            onComplete?.Invoke();
            return;
        }

        activeDialogueGroup = dialogueDatabase[groupId];
        if (activeDialogueGroup.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        onDialogueComplete = onComplete;
        currentIndex = 0;
        isDialogueRunning = true;

        viewer.OpenPanel();
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        var line = activeDialogueGroup[currentIndex];
        viewer.ShowText(line.Speaker, line.Text);
    }

    private void HandlePlayerInput()
    {
        // 1. 글자 출력 중이면 즉시 전체 대사 표시 (스킵)
        if (viewer.IsTyping)
        {
            viewer.SkipTyping();
            return;
        }

        // 2. 대사 출력이 끝났다면 다음 대사로 넘김
        currentIndex++;
        if (currentIndex < activeDialogueGroup.Count)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueRunning = false;
        viewer.ClosePanel();
        onDialogueComplete?.Invoke();
    }
}