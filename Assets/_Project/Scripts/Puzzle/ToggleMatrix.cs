using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleMatrix : MonoBehaviour
{
    [Header("UI 연결 (레버 6개)")]
    [SerializeField] private Image[] leverImages = new Image[6]; // L0 ~ L5 레버

    [Header("UI 연결 (상태 텍스트)")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("색상 세팅")]
    [SerializeField] private Color offColor = Color.gray;   // 레버 OFF 색상
    [SerializeField] private Color onColor = Color.green;   // 레버 ON 색상

    [Header("외부 연결")]
    [SerializeField] private MainConsoleManager mainConsoleManager; // 메인 콘솔 매니저

    // 6개 레버의 현재 ON/OFF 상태 저장 (true = ON, false = OFF)
    private bool[] leverStates = new bool[6];

    private void Start()
    {
        // [프로토타입 초기 상태 세팅]
        leverStates[0] = false;  // L0
        leverStates[1] = false;  // L1
        leverStates[2] = false; // L2
        leverStates[3] = false; // L3
        leverStates[4] = false;  // L4
        leverStates[5] = false;  // L5

        CheckPuzzleClear();
    }

    // 버튼 클릭 이벤트 
    // 0번 버튼 클릭: 0번, 2번, 4번 레버 반전
    public void OnClick_Button0()
    {
        ToggleLever(0);
        ToggleLever(2);
        ToggleLever(4);
        CheckPuzzleClear();
    }

    // 1번 버튼 클릭: 0번, 1번, 3번 레버 반전
    public void OnClick_Button1()
    {
        ToggleLever(0);
        ToggleLever(1);
        ToggleLever(3);
        CheckPuzzleClear();
    }

    // 2번 버튼 클릭: 1번, 2번, 5번 레버 반전 
    public void OnClick_Button2()
    {
        ToggleLever(1);
        ToggleLever(2);
        ToggleLever(5);
        CheckPuzzleClear();
    }

    // 3번 버튼 클릭: 2번, 3번, 4번 레버 반전
    public void OnClick_Button3()
    {
        ToggleLever(2);
        ToggleLever(3);
        ToggleLever(4);
        CheckPuzzleClear();
    }

    // 4번 버튼 클릭: 0번, 3번, 5번 레버 반전
    public void OnClick_Button4()
    {
        ToggleLever(0);
        ToggleLever(3);
        ToggleLever(5);
        CheckPuzzleClear();
    }

    // 5번 버튼 클릭: 1번, 3번, 5번 레버 반전
    public void OnClick_Button5()
    {
        ToggleLever(1);
        ToggleLever(3);
        ToggleLever(5);
        CheckPuzzleClear();
    }

    // 내부 로직 함수
    // 특정 인덱스 레버의 상태를 반전 시키는 함수
    private void ToggleLever(int index)
    {
        if (index >= 0 && index < leverStates.Length)
        {
            leverStates[index] = !leverStates[index];
        }
    }

    // 레버 상태 변경에 따른 UI 색상 업데이트
    private void UpdateUI()
    {
        for (int i = 0; i < leverStates.Length; i++)
        {
            if (leverImages[i] != null)
            {
                leverImages[i].color = leverStates[i] ? onColor : offColor;
            }
        }
    }

    // 클리어 여부 검증 (6개 레버가 모두 true인지 확인)
    private void CheckPuzzleClear()
    {
        UpdateUI();

        bool isAllOn = true;
        foreach (bool state in leverStates)
        {
            if (!state)
            {
                isAllOn = false;
                break;
            }
        }

        if (isAllOn)
        {
            // TODO: 퍼즐 클리어 시 실행할 로직 (ex. 다음 단계 진행, 문 열기 등)
            // 퍼즐 클리어 성공
            if (statusText != null)
            {
                statusText.text = "<color=green>[ SIGNAL : ON ]</color>";
            }

            if (mainConsoleManager != null)
            {
                mainConsoleManager.SetPuzzleClear();
            }

            Debug.Log("Clear");
        }
        else
        {
            if (statusText != null)
            {
                statusText.text = "[ SIGNAL: OFF ]";
            }
        }
    }
}