using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleMatrix : MonoBehaviour
{
    // 퍼즐이 클리어된 순간 구독자(Scene3DialogueController 등)에게 알려주는 이벤트
    public event Action OnPuzzleClearedEvent;


    [Header("UI 연결 (레버 6개)")]
    [SerializeField] private Image[] leverImages = new Image[6]; // L0 ~ L5 레버

    [Header("UI 연결 (토글 버튼 6개, 클리어 시 비활성화)")]
    [SerializeField] private Button[] toggleButtons = new Button[6]; // Button0 ~ Button5

    [Header("UI 연결 (상태 텍스트)")]
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("색상 세팅")]
    [SerializeField] private Color offColor = Color.gray;   // 레버 OFF 색상
    [SerializeField] private Color onColor = Color.green;   // 레버 ON 색상

    [Header("외부 연결")]
    [SerializeField] private MainConsoleManager mainConsoleManager; // 메인 콘솔 매니저

    // 6개 레버의 현재 ON/OFF 상태 저장 (true = ON, false = OFF)
    private bool[] leverStates = new bool[6];

    // 퍼즐 클리어 여부 (클리어 후에는 버튼 입력을 막기 위한 가드)
    private bool isPuzzleCleared = false;

    private void Start()
    {
        // [프로토타입 초기 상태 세팅]
        leverStates[0] = false;  // L0
        leverStates[1] = false;  // L1
        leverStates[2] = false; // L2
        leverStates[3] = false; // L3
        leverStates[4] = false;  // L4
        leverStates[5] = false;  // L5

        isPuzzleCleared = false;
        SetButtonsInteractable(true);

        CheckPuzzleClear();
    }

    // 버튼 클릭 이벤트
    // 0번 버튼 클릭: 0번, 2번, 4번 레버 반전
    public void OnClick_Button0()
    {
        if (isPuzzleCleared) return;

        ToggleLever(0);
        ToggleLever(2);
        ToggleLever(4);
        CheckPuzzleClear();
    }

    // 1번 버튼 클릭: 0번, 1번, 3번 레버 반전
    public void OnClick_Button1()
    {
        if (isPuzzleCleared) return;

        ToggleLever(0);
        ToggleLever(1);
        ToggleLever(3);
        CheckPuzzleClear();
    }

    // 2번 버튼 클릭: 1번, 2번, 5번 레버 반전
    public void OnClick_Button2()
    {
        if (isPuzzleCleared) return;

        ToggleLever(1);
        ToggleLever(2);
        ToggleLever(5);
        CheckPuzzleClear();
    }

    // 3번 버튼 클릭: 2번, 3번, 4번 레버 반전
    public void OnClick_Button3()
    {
        if (isPuzzleCleared) return;

        ToggleLever(2);
        ToggleLever(3);
        ToggleLever(4);
        CheckPuzzleClear();
    }

    // 4번 버튼 클릭: 0번, 3번, 5번 레버 반전
    public void OnClick_Button4()
    {
        if (isPuzzleCleared) return;

        ToggleLever(0);
        ToggleLever(3);
        ToggleLever(5);
        CheckPuzzleClear();
    }

    // 5번 버튼 클릭: 1번, 3번, 5번 레버 반전
    public void OnClick_Button5()
    {
        if (isPuzzleCleared) return;

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

    // 토글 버튼들을 한번에 활성/비활성화
    private void SetButtonsInteractable(bool interactable)
    {
        if (toggleButtons == null) return;

        foreach (var button in toggleButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
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
            // 이미 클리어 처리된 상태면 중복 실행 방지
            if (isPuzzleCleared) return;

            // 퍼즐 클리어 성공
            isPuzzleCleared = true;
            SetButtonsInteractable(false); // 클리어 후 버튼 입력 차단

            if (statusText != null)
            {
                statusText.text = "<color=green>[ SIGNAL : ON ]</color>";
            }

            // 미니맵 등에서 바로 참조할 수 있도록 클리어 여부부터 동기화
            // 콘솔 화면 전환은 Scene3DialogueController가 대사 타이밍에 맞춰 RefreshConsoleScreen()으로 처리
            if (mainConsoleManager != null)
            {
                mainConsoleManager.isPuzzleCleared = true;
            }

            Debug.Log("Clear");

            OnPuzzleClearedEvent?.Invoke();
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