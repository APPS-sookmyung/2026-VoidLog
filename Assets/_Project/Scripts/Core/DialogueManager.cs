using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    private bool canSkipThisFrame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (viewer == null)
            viewer = GetComponent<DialogueViewer>();
    }

    private void Update()
    {
        // 대사 진행 중이 아니거나, 첫 프레임 입력 방지 상태면 무시
        if (!isDialogueRunning || !canSkipThisFrame) return;

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

        // 따옴표 안의 쉼표는 무시하고 자르는 정규식 패턴
        string csvSplitPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

        // 첫 번째 줄(Header) 제외하고 1번 인덱스부터 순회
        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = Regex.Split(rows[i], csvSplitPattern);

            // 필수 항목 4개(GroupID, Order, Speaker, Text) 이상인 행만 유효하게 처리
            if (cols.Length >= 4)
            {
                string groupId = cols[0].Trim();
                int.TryParse(cols[1].Trim(), out int order);
                string speaker = cols[2].Trim();
                // 양끝의 큰따옴표("") 제거 및 줄바꿈(\n) 치환
                string text = cols[3].Trim().Trim('"').Replace("\\n", "\n");

                // 5번째(Portrait), 6번째(SoundEffect) 열은 비어있을 수도 있으므로 안전하게 추출
                string portrait = cols.Length > 4 ? cols[4].Trim().Trim('"') : "";
                string soundEffect = cols.Length > 5 ? cols[5].Trim().Trim('"') : "";

                DialogueData data = new DialogueData(groupId, order, speaker, text, portrait, soundEffect);

                if (!dialogueDatabase.ContainsKey(groupId))
                {
                    dialogueDatabase[groupId] = new List<DialogueData>();
                }
                dialogueDatabase[groupId].Add(data);
            }
        }

        // 각 그룹 내 Order 기준 오름차순 정렬
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
        canSkipThisFrame = false; // 대사창 열린 즉시 클릭 넘김 차단

        if (viewer != null)
        {
            viewer.OpenPanel();
            DisplayCurrentLine();
        }
        else
        {
            Debug.LogError("[DialogueManager] DialogueViewer 컴포넌트가 연결되어 있지 않습니다.");
        }

        StopAllCoroutines();
        StartCoroutine(EnableDialogueInputNextFrame());
    }

    private IEnumerator EnableDialogueInputNextFrame()
    {
        yield return null; // 1프레임 뒤부터 입력 활성화
        canSkipThisFrame = true;
    }

    private void DisplayCurrentLine()
    {
        if (viewer == null || currentIndex >= activeDialogueGroup.Count) return;

        var line = activeDialogueGroup[currentIndex];
        viewer.ShowText(line.Speaker, line.Text);
    }

    private void HandlePlayerInput()
    {
        if (viewer == null) return;

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
        canSkipThisFrame = false;

        if (viewer != null)
        {
            viewer.ClosePanel();
        }

        onDialogueComplete?.Invoke();
    }

    public bool getIsDialogueRunning()
    {
        return isDialogueRunning;
    }
}