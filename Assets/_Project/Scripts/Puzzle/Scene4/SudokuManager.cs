using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 이동/로드에 필요한 네임스페이스

public class SudokuManager : MonoBehaviour
{
    [Header("연결할 것들 (Inspector에서 드래그)")]
    public GameObject cellPrefab;   // Cell_00 프리팹
    public Transform gridParent;    // SudokuGrid 오브젝트
    public TMP_Text resultText;     // "Correct!" / "Game Over!" 등을 보여줄 텍스트 (없어도 동작함)

    [Header("오답 카운트 관련")]
    public TMP_Text wrongCountText; // "오답: 0 / 3" 처럼 보여줄 텍스트 (Inspector에서 연결)
    public int maxWrongCount = 3;   // 몇 번 틀리면 게임오버가 될지

    [Header("색상 / 애니메이션 관련")]
    public Color correctColor = Color.green; // 정답일 때 글자 색
    public Color wrongColor = Color.red;     // 오답일 때 글자 색
    public Color defaultColor = Color.black; // 원래 처음부터 있던 숫자, 빈칸일 때 색
    public float pulseScale = 1.25f;    // 정답 맞췄을 때 몇 배로 커질지
    public float pulseDuration = 0.12f; // 커졌다가 원래대로 돌아오는데 걸리는 시간(초)

    [Header("클리어 후 연출 (맵 복귀 → 대사 → 다음 씬)")]
    [Tooltip("스도쿠 UI 전체 패널. 클리어하면 이 패널이 꺼지면서 맵 화면으로 돌아갑니다.\n" +
             "주의: SudokuManager가 붙어있는 오브젝트(또는 그 자식)는 넣지 마세요! 코루틴이 멈춥니다.")]
    [SerializeField] private GameObject sudokuPanel;
    [Tooltip("Scene4DialogueController가 붙어있는 오브젝트 (GameManager 하위)")]
    [SerializeField] private Scene4DialogueController dialogueController;
    [Tooltip("\"Correct!\"를 보여주고 패널을 닫기까지 대기 시간 (초)")]
    [SerializeField] private float closeDelayTime = 1f;
    [Tooltip("맵으로 돌아온 뒤 대사가 시작되기까지 대기 시간 (초)")]
    [SerializeField] private float dialogueStartDelay = 0.5f;
    [Tooltip("스도쿠 패널이 닫혀 맵으로 돌아왔을 때 호출 (예: 플레이어 이동 잠금 해제, 도어락 UI 숨기기 등)")]
    [SerializeField] private UnityEvent onReturnToMap;

    [Header("씬 전환 설정")]
    [Tooltip("PuzzleClear 대사가 끝난 뒤 이동할 다음 씬의 이름 (예: Scene_05...). Build Settings에 등록되어 있어야 합니다.")]
    [SerializeField] private string nextSceneName = "";
    [Tooltip("게임오버 후 씬이 재시작되기 전 대기 시간 (초)")]
    [SerializeField] private float loadDelayTime = 1.5f;

    // 9x9 입력칸들을 담아두는 배열
    private TMP_InputField[,] cells = new TMP_InputField[9, 9];

    // 애니메이션을 재생할 때 필요한, 각 칸의 실제 GameObject 배열
    private GameObject[,] cellObjects = new GameObject[9, 9];

    // 지금까지 오답을 몇 번 냈는지 기억하는 변수
    private int wrongCount = 0;

    // 게임이 이미 끝났는지(오답 3번 이상) 여부
    private bool isGameOver = false;

    // 스도쿠를 클리어했는지 여부 (중복 저장 방지)
    private bool isCleared = false;

    [Header("클리어 저장 시 이어하기 위치 (비우면 기본 위치)")]
    [SerializeField] private string saveSpawnPointName = "";

    // 0 = 빈칸(플레이어가 채워야 함), 나머지 숫자 = 처음부터 고정
    private int[,] puzzle = new int[9, 9]
    {
        {5,3,0, 0,7,0, 0,0,0},
        {6,0,0, 1,9,5, 0,0,0},
        {0,9,8, 0,0,0, 0,6,0},
        {8,0,0, 0,6,0, 0,0,3},
        {4,0,0, 8,0,3, 0,0,1},
        {7,0,0, 0,2,0, 0,0,6},
        {0,6,0, 0,0,0, 2,8,0},
        {0,0,0, 4,1,9, 0,0,5},
        {0,0,0, 0,8,0, 0,7,9}
    };

    // 완성됐을 때 정답
    private int[,] solution = new int[9, 9]
    {
        {5,3,4, 6,7,8, 9,1,2},
        {6,7,2, 1,9,5, 3,4,8},
        {1,9,8, 3,4,2, 5,6,7},
        {8,5,9, 7,6,1, 4,2,3},
        {4,2,6, 8,5,3, 7,9,1},
        {7,1,3, 9,2,4, 8,5,6},
        {9,6,1, 5,3,7, 2,8,4},
        {2,8,7, 4,1,9, 6,3,5},
        {3,4,5, 2,8,6, 1,7,9}
    };

    void Start()
    {
        GenerateGrid();
        UpdateWrongCountUI(); // 시작할 때 "오답: 0 / 3" 을 미리 화면에 표시

        // 이미 클리어한 저장 데이터가 있으면 정답이 채워진 상태로 복구
        if (SaveManager.Instance != null && SaveManager.Instance.Data.isScene4Cleared)
        {
            RestoreClearedState();
        }
    }

    void GenerateGrid()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int r = row;
                int c = col;

                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                cellObj.name = "Cell_" + row + "_" + col;
                cellObjects[row, col] = cellObj;

                TMP_InputField input = cellObj.GetComponent<TMP_InputField>();
                cells[row, col] = input;

                int number = puzzle[row, col];

                if (number != 0)
                {
                    // 원래 문제에 있던 숫자 (플레이어가 못 바꾸는 칸)
                    input.text = number.ToString();
                    input.interactable = false;

                    if (input.textComponent != null)
                        input.textComponent.color = defaultColor;
                }
                else
                {
                    // 플레이어가 채워야 하는 빈 칸
                    input.text = "";
                    input.interactable = true;

                    input.onEndEdit.AddListener((value) => OnCellEndEdit(r, c));
                }
            }
        }
    }

    void OnCellEndEdit(int row, int col)
    {
        // 이미 게임오버/클리어 상태라면 더 이상 아무것도 하지 않음
        if (isGameOver || isCleared) return;

        TMP_InputField input = cells[row, col];
        string text = input.text;

        // 칸을 비워두고 나갔다면 그냥 기본 색으로 되돌리고 끝
        if (string.IsNullOrEmpty(text))
        {
            if (input.textComponent != null)
                input.textComponent.color = defaultColor;
            return;
        }

        int playerNumber;

        if (!int.TryParse(text, out playerNumber))
        {
            input.text = "";
            return;
        }

        if (playerNumber == solution[row, col])
        {
            // ---- 정답인 경우 ----
            if (input.textComponent != null)
                input.textComponent.color = correctColor;

            StartCoroutine(PulseCell(cellObjects[row, col]));

            // 마지막 칸까지 다 맞췄으면 자동 클리어
            if (IsBoardSolved())
            {
                OnSudokuCleared();
            }
        }
        else
        {
            // ---- 오답인 경우 ----
            if (input.textComponent != null)
                input.textComponent.color = wrongColor;

            wrongCount++;
            UpdateWrongCountUI();

            if (wrongCount >= maxWrongCount)
            {
                GameOver();
            }
        }
    }

    IEnumerator PulseCell(GameObject cellObj)
    {
        Vector3 originalScale = cellObj.transform.localScale;
        Vector3 targetScale = originalScale * pulseScale;

        float elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            cellObj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            cellObj.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        cellObj.transform.localScale = originalScale;
    }

    void UpdateWrongCountUI()
    {
        if (wrongCountText != null)
        {
            wrongCountText.text = "오답: " + wrongCount + " / " + maxWrongCount;
        }
    }

    // 오답이 maxWrongCount(기본 3)번 쌓이면 호출되는 게임오버 처리
    void GameOver()
    {
        isGameOver = true;

        LockAllCells();

        if (resultText != null)
        {
            resultText.text = "Game Over!";
            resultText.color = wrongColor;
        }

        Debug.Log("[Sudoku] 게임 오버! 오답이 " + maxWrongCount + "번 누적되었습니다. 씬을 처음부터 다시 로드합니다.");

        // 실패 시 현재 씬을 처음부터 다시 로드 (잠시 대기 후 실행)
        StartCoroutine(RestartSceneAfterDelay());
    }

    // 일정 시간 대기 후 현재 씬 재시작 코루틴
    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CheckAnswer()
    {
        if (isGameOver || isCleared) return;

        bool isCorrect = true;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                string text = cells[row, col].text;

                if (string.IsNullOrEmpty(text))
                {
                    isCorrect = false;
                    continue;
                }

                int playerNumber;
                if (!int.TryParse(text, out playerNumber) || playerNumber != solution[row, col])
                {
                    isCorrect = false;
                }
            }
        }

        if (resultText != null)
        {
            if (isCorrect)
            {
                resultText.text = "Correct!";
                resultText.color = correctColor;
            }
            else
            {
                resultText.text = "Wrong. Try again.";
                resultText.color = wrongColor;
            }
        }

        if (isCorrect)
        {
            OnSudokuCleared();
        }
    }

    bool IsBoardSolved()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int playerNumber;
                if (!int.TryParse(cells[row, col].text, out playerNumber) ||
                    playerNumber != solution[row, col])
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 스도쿠 클리어 처리 + 저장
    void OnSudokuCleared()
    {
        if (isCleared) return;
        isCleared = true;

        LockAllCells();

        if (resultText != null)
        {
            resultText.text = "Correct!";
            resultText.color = correctColor;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetSceneCleared(4, saveSpawnPointName);
        }

        Debug.Log("[Sudoku] 스도쿠 클리어!");

        // 클리어 후: 스도쿠 닫고 맵으로 복귀 → PuzzleClear 대사 → 끝나면 다음 씬으로 이동
        StartCoroutine(ClearSequence());
    }

    // 클리어 직후 연출: "Correct!" 잠깐 → 스도쿠 패널 닫기(맵 복귀) → 대사 → 다음 씬
    IEnumerator ClearSequence()
    {
        // 1) "Correct!" 잠깐 보여줌
        yield return new WaitForSeconds(closeDelayTime);

        // 2) 스도쿠 패널을 닫아서 맵 화면으로 복귀
        if (sudokuPanel != null)
        {
            sudokuPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[Sudoku] sudokuPanel이 연결되지 않았습니다. 스도쿠 UI가 대사 패널을 가릴 수 있어요!");
        }

        onReturnToMap?.Invoke();

        // 3) 대사 시작 (맵 복귀 후 dialogueStartDelay초 뒤에 시작)
        // 주의: sudokuPanel이 꺼지면서 이 오브젝트가 비활성화될 수도 있으므로,
        //       여기서부터는 이 코루틴이 아니라 "항상 켜져 있는" Scene4DialogueController가 대기/진행을 맡습니다.
        PlayClearDialogueThenNextScene();
    }

    void PlayClearDialogueThenNextScene()
    {
        Debug.Log("[Sudoku] PuzzleClear 대사 시작");

        if (dialogueController != null)
        {
            // 대사가 끝나면 LoadNextScene 실행
            dialogueController.PlayClearDialogue(LoadNextScene, dialogueStartDelay);
        }
        else if (DialogueManager.Instance != null)
        {
            // 컨트롤러를 연결 안 했다면 DialogueManager를 직접 사용 (CSV는 Scene4DialogueController가 이미 로드해 둠)
            Debug.LogWarning("[Sudoku] dialogueController가 연결되지 않아 DialogueManager를 직접 호출합니다.");
            DialogueManager.Instance.StartDialogueGroup("PuzzleClear", LoadNextScene);
        }
        else
        {
            Debug.LogError("[Sudoku] DialogueManager.Instance가 없습니다. 대사 없이 다음 씬으로 이동합니다.");
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("다음 씬 이름(nextSceneName)이 비어있습니다! 인스펙터 창에서 이동할 씬 이름을 입력해주세요.");
        }
    }

    void RestoreClearedState()
    {
        isCleared = true;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                TMP_InputField input = cells[row, col];
                if (input == null) continue;

                if (puzzle[row, col] == 0)
                {
                    input.SetTextWithoutNotify(solution[row, col].ToString());
                    if (input.textComponent != null)
                        input.textComponent.color = correctColor;
                }
            }
        }

        LockAllCells();

        if (resultText != null)
        {
            resultText.text = "Correct!";
            resultText.color = correctColor;
        }
    }

    void LockAllCells()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (cells[row, col] != null)
                    cells[row, col].interactable = false;
            }
        }
    }
}