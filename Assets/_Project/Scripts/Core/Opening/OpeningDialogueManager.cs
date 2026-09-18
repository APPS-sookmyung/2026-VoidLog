using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// 오프닝 전용 대사 매니저 - 자동 진행 버전.
/// 클릭/스페이스 없이도 한 줄 타이핑이 끝나면 읽을 시간을 준 뒤 자동으로 다음 줄로 넘어간다.
/// (타이핑 도중 클릭하면 그 줄만 즉시 완성되는 스킵 기능은 편의상 남겨뒀다 - 필요 없으면 지워도 됨)
/// </summary>
public class OpeningDialogueManager : MonoBehaviour
{
    public static OpeningDialogueManager Instance { get; private set; }

    [SerializeField] private OpeningDialogueViewer viewer;

    [Tooltip("한 줄 타이핑이 끝난 뒤, 다음 줄로 자동으로 넘어가기까지 대기하는 시간(초)")]
    [SerializeField] private float readDelayPerLine = 1.2f;

    private Dictionary<string, List<OpeningDialogueData>> dialogueDatabase = new Dictionary<string, List<OpeningDialogueData>>();
    private bool isDialogueRunning = false;

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
            viewer = GetComponent<OpeningDialogueViewer>();
    }

    private void Update()
    {
        // 타이핑 도중 클릭하면 그 줄만 즉시 완성 (자동 진행 자체는 그대로 유지됨).
        // 이 기능 자체가 필요 없으면 이 Update()를 통째로 지워도 된다.
        if (isDialogueRunning && viewer != null && viewer.IsTyping)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                viewer.SkipTyping();
            }
        }
    }

    public void LoadDialogueDatabase(TextAsset csvFile)
    {
        dialogueDatabase.Clear();

        if (csvFile == null)
        {
            Debug.LogError("[OpeningDialogueManager] CSV 파일이 지정되지 않았습니다.");
            return;
        }

        string[] rows = csvFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string csvSplitPattern = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

        for (int i = 1; i < rows.Length; i++)
        {
            string[] cols = Regex.Split(rows[i], csvSplitPattern);

            if (cols.Length >= 4)
            {
                string groupId = cols[0].Trim();
                int.TryParse(cols[1].Trim(), out int order);
                string speaker = cols[2].Trim();
                string text = cols[3].Trim().Trim('"').Replace("\\n", "\n");
                string portrait = cols.Length > 4 ? cols[4].Trim().Trim('"') : "";
                string soundEffect = cols.Length > 5 ? cols[5].Trim().Trim('"') : "";
                string panelType = cols.Length > 7 ? cols[7].Trim().Trim('"') : "";

                var data = new OpeningDialogueData(groupId, order, speaker, text, portrait, soundEffect, panelType);

                if (!dialogueDatabase.ContainsKey(groupId))
                {
                    dialogueDatabase[groupId] = new List<OpeningDialogueData>();
                }
                dialogueDatabase[groupId].Add(data);
            }
        }

        foreach (var group in dialogueDatabase.Values)
        {
            group.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }

    /// <summary>groupId의 대사를 처음부터 끝까지 자동으로 재생하고, 끝나면 onComplete를 호출한다.</summary>
    public void StartDialogueGroup(string groupId, Action onComplete = null)
    {
        if (!dialogueDatabase.ContainsKey(groupId))
        {
            Debug.LogWarning($"[OpeningDialogueManager] GroupID '{groupId}'에 해당하는 대사를 찾을 수 없습니다.");
            onComplete?.Invoke();
            return;
        }

        List<OpeningDialogueData> group = dialogueDatabase[groupId];
        if (group.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (viewer == null)
        {
            Debug.LogError("[OpeningDialogueManager] OpeningDialogueViewer가 연결되어 있지 않습니다.");
            onComplete?.Invoke();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(PlayGroupRoutine(group, onComplete));
    }

    private IEnumerator PlayGroupRoutine(List<OpeningDialogueData> group, Action onComplete)
    {
        isDialogueRunning = true;
        viewer.OpenPanel();

        foreach (var line in group)
        {
            viewer.ShowText(line.Speaker, line.Text, line.PanelType);

            // 타이핑이 끝날 때까지 대기 (도중에 클릭하면 Update()가 즉시 완성시켜줌)
            while (viewer.IsTyping)
            {
                yield return null;
            }

            // 다 읽을 시간을 준 뒤 자동으로 다음 줄로
            yield return new WaitForSeconds(readDelayPerLine);
        }

        viewer.ClosePanel();
        isDialogueRunning = false;
        onComplete?.Invoke();
    }

    public bool getIsDialogueRunning()
    {
        return isDialogueRunning;
    }
}