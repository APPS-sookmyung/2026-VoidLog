using TMPro;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ArchiveRoomPuzzle : MonoBehaviour
{

    [Header("UI References (Digits)")]
    [Tooltip("각 숫자가 표시될 4개의 TextMeshPro UI")]
    [SerializeField] private TextMeshProUGUI[] digitTexts = new TextMeshProUGUI[4];

    [Header("UI References")]
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI feedbackText; // 안내 및 오답 알림용 텍스트

    [Header("Settings")]
    [SerializeField] private string correctPassword = "1234";
    [SerializeField] private string nextSceneName = "Scene_06_Hangar"; // 6번 격납고 씬 이름

    private Canvas puzzleCanvas;
    public bool IsPuzzleCleared { get; private set; } = false;

    private void Awake()
    {
        puzzleCanvas = GetComponent<Canvas>();
        if (puzzleCanvas == null) puzzleCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        if (passwordInputField != null)
        {
            // 초기 세팅: 자리 제한 및 숫자만 입력 가능하도록 강제 설정
            passwordInputField.characterLimit = 4;
            passwordInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            
            // 입력창 글자 입력될 때마다 실행될 리스너 연결
            passwordInputField.onValueChanged.AddListener(OnInputChanged);
        }
    }

    private void OnEnable()
    {
        // UI가 활성화될 때마다 초기화 및 포커스 설정
        InitializeUI();
    }

    private void Update()
    {
        // 엔터 키를 눌렀을 때 정답 제출
        if (passwordInputField.isFocused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            OnSubmitPassword(passwordInputField.text);
        }
    }

    private void InitializeUI()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "비밀번호를 입력하세요.";
            feedbackText.color = Color.white;
        }

        // 입력창과 숫자 텍스트 초기화
        passwordInputField.text = "";
        UpdateDigitDisplay("");
        passwordInputField.ActivateInputField(); // 활성화 시 자동으로 입력창에 포커스
    }

    // 입력 도중 호출
    private void OnInputChanged(string currentText)
    {
        // 입력이 다시 시작되면 글자 색상을 화이트로 리셋
        if (feedbackText != null)
        {
            feedbackText.color = Color.white;
            feedbackText.text = "입력 중...";
        }
        UpdateDigitDisplay(currentText);

        // 4자리가 모두 입력되면 자동으로 정답 검증
        if (currentText.Length >= 4)
        {
            OnSubmitPassword(currentText);
        }
    }

    // 엔터키를 누르거나 입력 포커스가 빠졌을 때 판정
    private void OnSubmitPassword(string inputPassword)
    {
        // 4자리를 다 채우지 않았다면 오류 표시
        if (inputPassword.Length < 4)
        {
            ShowError("4자리 숫자를 모두 입력해야 합니다.");
            return;
        }

        // 비밀번호 검증
        if (inputPassword == correctPassword)
        {
            StartCoroutine(SuccessSequence());
        }
        else
        {
            ShowError("인증 실패: 보안 권한이 거부되었습니다.");
        }
    }

    // 오답 시 연출
    private void ShowError(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.color = Color.red; // 빨간색으로 변경
            feedbackText.text = message;
        }
        
        // 입력창 초기화 및 재포커스
        passwordInputField.SetTextWithoutNotify(""); // onValueChanged 리스너를 호출하지 않고 텍스트 초기화
        UpdateDigitDisplay("");
        passwordInputField.ActivateInputField();
    }

    // 입력된 숫자를 밑줄 UI에 표시하는 함수
    private void UpdateDigitDisplay(string currentText)
    {
        for (int i = 0; i < digitTexts.Length; i++)
        {
            if (digitTexts[i] != null)
            {
                digitTexts[i].text = (i < currentText.Length) ? currentText[i].ToString() : "_";
            }
        }
    }

    // 정답 시 연출
    private IEnumerator SuccessSequence()
    {
        if (feedbackText != null)
        {
            IsPuzzleCleared = true;
            feedbackText.color = Color.green;
            feedbackText.text = "[인증 완료: 보안 권한이 승인되었습니다.]";
        }

        passwordInputField.interactable = false;

        // 잠시 연출을 보고 넘어갈 수 있도록 1.5초 대기
        yield return new WaitForSeconds(2.0f);

        // 캔버스 비활성화
        if (puzzleCanvas != null) puzzleCanvas.gameObject.SetActive(false);
    }

}
