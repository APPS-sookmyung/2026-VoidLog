using UnityEngine;
using TMPro;

public class SudokuManager : MonoBehaviour
{
    [Header("연결할 것들 (Inspector에서 드래그)")]
    public GameObject cellPrefab;   // Cell_00 프리팹
    public Transform gridParent;    // SudokuGrid 오브젝트
    public TMP_Text resultText;     // 결과 표시 텍스트 (없어도 동작함)

    private TMP_InputField[,] cells = new TMP_InputField[9, 9];

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
    }

    void GenerateGrid()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                cellObj.name = "Cell_" + row + "_" + col;

                TMP_InputField input = cellObj.GetComponent<TMP_InputField>();
                cells[row, col] = input;

                int number = puzzle[row, col];

                if (number != 0)
                {
                    input.text = number.ToString();
                    input.interactable = false; // 처음부터 있는 숫자는 못 바꾸게
                }
                else
                {
                    input.text = "";
                    input.interactable = true;
                }
            }
        }
    }

    // "정답 확인" 버튼에 연결할 함수
    public void CheckAnswer()
    {
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

                int playerNumber = int.Parse(text);

                if (playerNumber != solution[row, col])
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
                resultText.color = Color.green;
            }
            else
            {
                resultText.text = "Wrong. Try again.";
                resultText.color = Color.red;
            }
        }

        Debug.Log(isCorrect ? "스도쿠 클리어!" : "오답");
    }
}