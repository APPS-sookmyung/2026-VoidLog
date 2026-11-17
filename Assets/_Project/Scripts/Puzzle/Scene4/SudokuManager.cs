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
    public TMP_Text resultText;     // "Success!" / "Game Over!" 등을 보여줄 텍스트 (없어도 동작함)

    [Header("오답 카운트 관련")]
    public TMP_Text wrongCountText; // "오답: 0 / 3" 처럼 보여줄 텍스트 (Inspector에서 연결)
    public int maxWrongCount = 3;   // 몇 번 틀리면 게임오버가 될지

    [Header("색상 / 애니메이션 관련")]
    public Color correctColor = Color.green; // 정답일 때 글자 색
    public Color wrongColor = Color.red;     // 오답일 때 글자 색
    public Color defaultColor = Color.black; // 원래 처음부터 있던 숫자, 빈칸일 때 색
    public float pulseScale = 1.25f;    // 정답 맞췄을 때 몇 배로 커질지
    public float pulseDuration = 0.12f; // 커졌다가 원래대로 돌아오는데 걸리는 시간(초)

    [Header("클리어 후 연출 (Success 화면 → E → 대사 → 벽 통과)")]
    [Tooltip("클리어 시 켜줄 Success 화면 (비워두면 resultText에 Success 문구만 표시)")]
    [SerializeField] private GameObject successPanel;
    [Tooltip("Scene4DialogueController가 붙어있는 오브젝트 (GameManager 하위)")]
    [SerializeField] private Scene4DialogueController dialogueController;
    [Tooltip("스도쿠 창을 닫은 뒤 PuzzleClear 대사를 재생할지 여부")]
    [SerializeField] private bool playClearDialogue = true;
    [Tooltip("스도쿠 창이 닫힌 뒤 대사가 시작되기까지 대기 시간 (초)")]
    [SerializeField] private float dialogueStartDelay = 0.5f;
    [Tooltip("SceneTransitionWall 스크립트가 붙은 '그 벽'의 Box Collider 2D.\n" +
             "처음엔 Is Trigger 해제(진짜 벽)로 두세요. 클리어 후 스도쿠 창을 닫으면 Is Trigger가 켜져서 통과 → 다음 씬으로 이동합니다.")]
    [SerializeField] private Collider2D exitWallCollider;
    [Tooltip("스도쿠 창이 닫혀 맵으로 돌아왔을 때 추가로 실행할 이벤트 (없어도 됨)")]
    [SerializeField] private UnityEvent onReturnToMap;

    [Header("게임오버")]
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

    // 클리어 후 처리(대사 + 벽 열기)를 이미 했는지 (중복 실행 방지)
    private bool clearHandled = false;

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
        // 씬을 나갔다 돌아와도 이어서 하도록, 저장된 오답 횟수를 불러옴
        wrongCount = Scene4Progress.wrongCount;

        GenerateGrid(); // 저장된 칸 입력값도 여기서 복구됨
        UpdateWrongCountUI();

        // 이미 클리어한 기록이 있으면 정답이 채워진 상태로 복구
        bool clearedBefore = Scene4Progress.sudokuCleared ||
            (SaveManager.Instance != null && SaveManager.Instance.Data.isScene4Cleared);

        if (clearedBefore)
        {
            RestoreClearedState();

            // 클리어 후 처리(창 닫기 + 대사)까지 끝낸 적이 있으면 벽도 바로 통과 가능하게 열어둠
            if (Scene4Progress.clearHandled)
            {
                clearHandled = true;
                OpenExitWall();
            }
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

                    // 이전에 입력해 둔 값이 있으면 복구 (맞으면 초록, 틀리면 빨강)
                    int saved = Scene4Progress.board[row, col];
                    if (saved != 0)
                    {
                        input.SetTextWithoutNotify(saved.ToString());
                        if (input.textComponent != null)
                            input.textComponent.color = (saved == solution[row, col]) ? correctColor : wrongColor;
                    }

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
            Scene4Progress.board[row, col] = 0; // 저장값도 비움
            if (input.textComponent != null)
                input.textComponent.color = defaultColor;
            return;
        }

        int playerNumber;

        if (!int.TryParse(text, out playerNumber))
        {
            Scene4Progress.board[row, col] = 0;
            input.text = "";
            return;
        }

        // 입력한 값을 저장 (씬을 나갔다 돌아와도 유지)
        Scene4Progress.board[row, col] = playerNumber;

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
            Scene4Progress.wrongCount = wrongCount; // 오답 횟수 저장
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

        // 게임오버면 스도쿠만 처음부터 (편지/도어락/대사 진행은 유지)
        Scene4Progress.ResetSudoku();

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

        if (isCorrect)
        {
            // 클리어 처리 (Success 문구는 OnSudokuCleared 안에서 표시)
            OnSudokuCleared();
        }
        else if (resultText != null)
        {
            resultText.text = "Wrong. Try again.";
            resultText.color = wrongColor;
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

    // 스도쿠 클리어 처리 + 저장 (창은 닫지 않고 Success 화면만 보여줌)
    void OnSudokuCleared()
    {
        if (isCleared) return;
        isCleared = true;
        Scene4Progress.sudokuCleared = true; // 씬을 나갔다 돌아와도 클리어 상태 유지

        LockAllCells();
        ShowSuccess();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetSceneCleared(4, saveSpawnPointName);
        }

        Debug.Log("[Sudoku] 스도쿠 클리어! E를 누르면 창이 닫힙니다.");
    }

    void ShowSuccess()
    {
        if (successPanel != null) successPanel.SetActive(true);

        if (resultText != null)
        {
            resultText.text = "Success!\n[E]를 눌러 닫기";
            resultText.color = correctColor;
        }
    }

    // 도어락(InteractableObject)의 On Close 이벤트에 연결할 함수
    // 스도쿠 창이 닫힐 때마다 호출되지만, 클리어한 경우에만 뒷처리를 함
    public void OnSudokuWindowClosed()
    {
        // 풀던 중에 닫은 경우: 아무것도 안 함 (칸 입력값/오답 횟수는 그대로 보존됨)
        if (!isCleared || clearHandled) return;
        clearHandled = true;
        Scene4Progress.clearHandled = true; // 돌아왔을 때 대사 재생 없이 벽이 바로 열려 있도록 기록

        onReturnToMap?.Invoke();

        if (playClearDialogue && dialogueController != null)
        {
            // 대사가 끝나면 출구 벽을 열어줌
            dialogueController.PlayClearDialogue(OpenExitWall, dialogueStartDelay);
        }
        else
        {
            OpenExitWall();
        }
    }

    void OpenExitWall()
    {
        // 벽 콜라이더를 Trigger로 전환:
        // 더 이상 길을 막지 않고(통과 가능), 플레이어가 닿으면 SceneTransitionWall의 OnTriggerEnter2D가 실행되어 다음 씬으로 이동
        if (exitWallCollider != null)
            exitWallCollider.isTrigger = true;
        else
            Debug.LogWarning("[Sudoku] exitWallCollider가 연결되지 않았습니다! Inspector에서 벽의 Box Collider 2D를 연결해주세요.");
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
            resultText.text = "Success!\n[E]를 눌러 닫기";
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