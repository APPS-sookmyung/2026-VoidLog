using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


// 오프닝과 완전히 분리된 엔딩 전용 대사 매니저
public class EndingDialogueManager : MonoBehaviour
{
    public static EndingDialogueManager Instance { get; private set; }

    [SerializeField] private OpeningDialogueViewer viewer;

    [Tooltip("한 줄 타이핑이 끝난 뒤, 다음으로 넘어가기까지 대기하는 시간(초)")]
    [SerializeField] private float readDelayPerLine = 1.5f;

    private Dictionary<string, List<OpeningDialogueData>> endingDatabase = new Dictionary<string, List<OpeningDialogueData>>();
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
        if (isDialogueRunning && viewer != null && viewer.IsTyping)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                viewer.SkipTyping();
            }
        }
    }

    // 엔딩 전용 CSV 데이터 로드
    public void LoadEndingDatabase(TextAsset csvFile)
    {
        endingDatabase.Clear();

        if (csvFile == null)
        {
            Debug.LogError("[EndingDialogueManager] 엔딩 CSV 파일이 지정되지 않았습니다.");
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

                if (!endingDatabase.ContainsKey(groupId))
                {
                    endingDatabase[groupId] = new List<OpeningDialogueData>();
                }
                endingDatabase[groupId].Add(data);
            }
        }
    }

    public void PlayRandomEndingLine(string groupId, Action onComplete = null)
    {
        if (!endingDatabase.ContainsKey(groupId))
        {
            Debug.LogWarning($"[EndingDialogueManager] GroupID '{groupId}'에 해당하는 엔딩 대사를 찾을 수 없습니다.");
            onComplete?.Invoke();
            return;
        }

        List<OpeningDialogueData> group = endingDatabase[groupId];
        if (group.Count == 0 || viewer == null)
        {
            onComplete?.Invoke();
            return;
        }

        OpeningDialogueData randomLine = group[UnityEngine.Random.Range(0, group.Count)];

        StopAllCoroutines();
        StartCoroutine(PlaySingleLineRoutine(randomLine, onComplete));
    }

    private IEnumerator PlaySingleLineRoutine(OpeningDialogueData line, Action onComplete)
    {
        isDialogueRunning = true;
        viewer.OpenPanel();

        viewer.ShowText(line.Speaker, line.Text, line.PanelType);

        while (viewer.IsTyping)
        {
            yield return null;
        }

        yield return new WaitForSeconds(readDelayPerLine);

        viewer.ClosePanel();
        isDialogueRunning = false;
        onComplete?.Invoke();
    }

    public bool GetIsDialogueRunning()
    {
        return isDialogueRunning;
    }
}