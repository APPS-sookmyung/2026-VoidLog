using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 이동/로드에 필요한 네임스페이스

public class SudokuManager : MonoBehaviour
{
    [Header("연결할 것들 (Inspector에서 드래그)")]
    public GameObject cellPrefab;   // Cell_00 프리팹
    public Transform gridParent;    // SudokuGrid 오브젝트
    public TMP_Text resultText;     // "Correct!" / "Game Over!" 등을 보여줄 텍스트 (없어도 동작함)

    [Header("오답 카운트 관련 (새로 추가된 부분)")]
    public TMP_Text wrongCountText; // "오답: 0 / 3" 처럼 보여줄 텍스트 (Inspector에서 연결)
    public int maxWrongCount = 3;   // 몇 번 틀리면 게임오버가 될지 (Inspector에서 숫자만 바꿔도 됨)

    [Header("색상 / 애니메이션 관련 (새로 추가된 부분)")]
    public Color correctColor = Color.green; // 정답일 때 글자 색
    public Color wrongColor = Color.red;     // 오답일 때 글자 색
    public Color defaultColor = Color.black; // 원래 처음부터 있던 숫자, 빈칸일 때 색
    public float pulseScale = 1.25f;    // 정답 맞췄을 때 몇 배로 커질지 (1.25 = 25% 더 커짐)
    public float pulseDuration = 0.12f; // 커졌다가 원래대로 돌아오는데 걸리는 시간(초)

    [Header("씬 전환 설정 (새로 추가됨)")]
    [Tooltip("스도쿠 클리어 시 이동할 다음 씬의 이름")]
    [SerializeField] private string nextSceneName = "";
    [Tooltip("씬이 전환되거나 재시작되기 전 대기 시간 (초)")]
    [SerializeField] private float loadDelayTime = 1.5f;

    // 9x9 입력칸들을 담아두는 배열. 예: cells[0,0] = 맨 왼쪽 위 칸의 입력창
    private TMP_InputField[,] cells = new TMP_InputField[9, 9];

    // 애니메이션(커졌다 작아졌다)을 재생할 때 필요한, 각 칸의 실제 GameObject 배열
    private GameObject[,] cellObjects = new GameObject[9, 9];

    // 지금까지 오답을 몇 번 냈는지 기억하는 변수
    private int wrongCount = 0;

    // 게임이 이미 끝났는지(오답 3번 이상) 여부. 게임오버 후에는 더 이상 입력을 못하게 막는 용도
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
                cellObjects[row, col] = cellObj; // 나중에 애니메이션 재생할 때 쓰려고 저장

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
        // 이미 게임오버 상태라면 더 이상 아무것도 하지 않음
        if (isGameOver) return;

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
                input.textComponent.color = correctColor; // 초록색으로 변경

            StartCoroutine(PulseCell(cellObjects[row, col])); // 살짝 커졌다 돌아오는 효과 재생

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
                input.textComponent.color = wrongColor; // 빨간색으로 변경

            wrongCount++;
            UpdateWrongCountUI(); // "오답: n / 3" 텍스트 갱신

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

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (cells[row, col] != null)
                    cells[row, col].interactable = false;
            }
        }

        if (resultText != null)
        {
            resultText.text = "Game Over!";
            resultText.color = wrongColor;
        }

        Debug.Log("게임 오버! 오답이 " + maxWrongCount + "번 누적되었습니다. 씬을 다시 로드합니다.");

        // 오답으로 실패 시 현재 씬 다시 로드 (코루틴을 통해 잠시 대기 후 실행)
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

        Debug.Log("스도쿠 클리어! 다음 씬으로 이동합니다.");

        // 클리어 시 다음 씬으로 이동 (코루틴을 통해 잠시 대기 후 실행)
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    // 일정 시간 대기 후 다음 씬 로드 코루틴
    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelayTime);

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