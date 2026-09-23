using UnityEngine;
using TMPro;
using System.Collections;

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

    // 9x9 입력칸들을 담아두는 배열. 예: cells[0,0] = 맨 왼쪽 위 칸의 입력창
    private TMP_InputField[,] cells = new TMP_InputField[9, 9];

    // 애니메이션(커졌다 작아졌다)을 재생할 때 필요한, 각 칸의 실제 GameObject 배열
    private GameObject[,] cellObjects = new GameObject[9, 9];

    // 지금까지 오답을 몇 번 냈는지 기억하는 변수
    private int wrongCount = 0;

    // 게임이 이미 끝났는지(오답 3번 이상) 여부. 게임오버 후에는 더 이상 입력을 못하게 막는 용도
    private bool isGameOver = false;

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
    }

    void GenerateGrid()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // ↓↓↓ 중요! for문 안에서 row, col을 그대로 델리게이트(아래 onEndEdit)에 넘기면
                //     나중에 함수가 실제로 실행될 때는 row, col이 이미 9, 9로 바뀌어있어서
                //     엉뚱한 칸을 가리키게 됩니다. 그래서 반드시 r, c 라는 "그 순간의 값을 복사한
                //     지역 변수"를 새로 만들어서 넘겨줘야 합니다. (C#의 클로저 캡처 문제)
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

                    // onEndEdit: 사용자가 이 칸에 숫자를 쓰고 Enter를 누르거나
                    //            다른 칸을 클릭해서 포커스를 벗어났을 때 자동으로 호출됩니다.
                    input.onEndEdit.AddListener((value) => OnCellEndEdit(r, c));
                }
            }
        }
    }

    // 한 칸의 입력이 "끝났을 때" (Enter 또는 다른 곳 클릭) 호출되는 함수.
    // 여기서 정답인지 오답인지 바로바로 체크해서 색깔/애니메이션/오답카운트를 처리합니다.
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

        // 혹시 숫자가 아닌 값이 입력됐을 경우를 대비 (예: 알파벳 등 이상한 입력 방지)
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

    // 칸이 잠깐 커졌다가(pulseScale 배율) 다시 원래 크기로 돌아오는 애니메이션.
    // 코루틴(IEnumerator)은 Unity에서 "시간이 걸리는 동작"을 여러 프레임에 걸쳐
    // 자연스럽게 처리할 때 쓰는 방법입니다. StartCoroutine으로 실행시킵니다.
    IEnumerator PulseCell(GameObject cellObj)
    {
        Vector3 originalScale = cellObj.transform.localScale;
        Vector3 targetScale = originalScale * pulseScale;

        float elapsed = 0f;

        // 1단계: 원래 크기 -> 커진 크기로 부드럽게 변화 (Lerp = 두 값 사이를 보간)
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            cellObj.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null; // 한 프레임 기다렸다가 다음 줄로 이어서 실행
        }

        elapsed = 0f;

        // 2단계: 커진 크기 -> 다시 원래 크기로 부드럽게 변화
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            cellObj.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // 혹시 오차가 남았을 수도 있으니 마지막엔 정확히 원래 크기로 고정
        cellObj.transform.localScale = originalScale;
    }

    // 화면 위쪽 등에 "오답: 1 / 3" 처럼 표시해주는 함수
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

        // 모든 칸을 더 이상 못 누르게 잠금
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

        Debug.Log("게임 오버! 오답이 " + maxWrongCount + "번 누적되었습니다.");
    }

    // (선택 기능) "정답 확인" 버튼을 따로 만들고 싶다면 이 함수를 연결해서
    // 보드 전체가 다 채워졌고 전부 맞았는지 한번에 검사할 수도 있습니다.
    // 지금은 칸마다 실시간으로 채점하기 때문에 필수는 아니지만, 남겨두었습니다.
    public void CheckAnswer()
    {
        if (isGameOver) return;

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

        Debug.Log(isCorrect ? "스도쿠 클리어!" : "오답");
    }
}