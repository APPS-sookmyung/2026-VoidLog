using System;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 전환을 위해 반드시 필요!

//퍼즐2 도어락 스크립트
public class ControlRoomDoor : MonoBehaviour
{
    [Header("대사")]
    [SerializeField] private Puzzle_01_02_DialogueManager dialogueManager;
    [SerializeField] private DialogueSO firstDialogue;
    [SerializeField] private DialogueSO lastDialogue;


    [Header("도어락")]
    [SerializeField] Canvas doorLockCanvas; // 도어락 캔버스 
    [SerializeField] TextMeshProUGUI escText; // ESC 창닫기 글자

    [Header("Settings")]
    [SerializeField] private string correctPassword = "123456789";
    
    [SerializeField] private TextMeshProUGUI feedbackText; // 안내 및 오답 알림용 텍스트
    [SerializeField] private TextMeshProUGUI statusText; // 상태 확인용 텍스트
    [Header("이동할 씬 설정")]
    [Tooltip("Build Settings에 등록된 이동하고자 하는 씬의 정확한 이름")]
    [SerializeField] private string nextSceneName;

    private string passwordInput; // 플레이어 정답 저장 변수
    private bool isDialogueRunning; // 중복 생성 방지

    void Start()
    {
        doorLockCanvas.gameObject.SetActive(false);
        lastDialogue.setHasDialogue(false);
    }
    void Update()
    {
        if (!GameProgressData.hasOpenedControlRoomDoor && doorLockCanvas.gameObject.activeSelf) 
        {   // 오픈 못한 상태에서 안내용 텍스트
            if (feedbackText != null)
            {
                if(passwordInput != "")
                {
                    feedbackText.color = Color.white;
                    feedbackText.text = "입력 중...";
                }
            }
            if(passwordInput.Length == 9)
            {
                OnSubmitPassword(passwordInput);
            }

        }
        if (!doorLockCanvas.gameObject.activeSelf)
        {
            CloseDoor(); // ESC -> 창 닫기    
        }
    }

    // 처음 도어락 열었을 때 - 비밀번호 초기화 및 상태와 안내 문구, 대사 출력
    public void DoorCheck() 
    {
        if (!GameProgressData.hasOpenedControlRoomDoor)
        {
            passwordInput = ""; //비밀번호 입력 초기화
            statusText.text = "잠김";
            feedbackText.text = "일반 사원은 접근할 수 없습니다.";
            feedbackText.color = Color.white;  

            if (!isDialogueRunning  && !firstDialogue.getHasDialogue())
            {
                StartCoroutine(DoorDialogue());
            }

        }
        else
        {
            doorLockCanvas.gameObject.SetActive(false);
            LoadNextScene();
        }
    }

    // 대사 출력 + 도어락 사이즈 및 위치 변경 
    private IEnumerator DoorDialogue()
    {
        isDialogueRunning = true;
        RectTransform canvasRect = doorLockCanvas.GetComponent<RectTransform>();

        // 도어락 UI 작게 + ESC 글자 안보이게
        escText.gameObject.SetActive(false);
        doorLockCanvas.transform.localScale = new Vector3(0.74f, 0.74f, 0.74f);
        canvasRect.offsetMin = new Vector2(-7, 168);
        canvasRect.offsetMax = new Vector2(-7, 168);

        // 대사 시작
        dialogueManager.DialogueStart(firstDialogue);

        // 대사가 끝날 때까지 기다리기
        yield return new WaitUntil(() => firstDialogue.getHasDialogue());

        // 대사 끝나면 원래 크기로 + ESC 글자 보이게
        escText.gameObject.SetActive(true);
        doorLockCanvas.transform.localScale = Vector3.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
        isDialogueRunning = false;
    }
    public void CloseDoor() // ESC 창 닫기
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKey(KeyCode.E))
        {
            doorLockCanvas.gameObject.SetActive(false);
            if (GameProgressData.hasOpenedControlRoomDoor && !lastDialogue.getHasDialogue())
            {
                dialogueManager.DialogueStart(lastDialogue);
            }
        }
    }

    // 정답 판정
    private void OnSubmitPassword(string inputPassword)
    {
        // 비밀번호 검증
        if (inputPassword == correctPassword)
        {
            GameProgressData.hasOpenedControlRoomDoor = true;
            feedbackText.color = Color.white;
            feedbackText.text = "인증이 완료되었습니다.";
            statusText.text ="열림";
            statusText.color = Color.blue;
        }
        else
        {
            ShowError("인증 실패: 보안 권한이 거부되었습니다.");
        }
    }

    // 오답 시 연출
    private void ShowError(string message)
    {
        // onValueChanged를 실행시키지 않고 입력창만 비우기
        passwordInput = "";
        if (feedbackText != null)
        {
            feedbackText.color = Color.red; // 빨간색으로 변경
            feedbackText.text = message;
        }
    }


    // 초기화 버튼
    public void DeletePasswordNumber() 
    {
        if (passwordInput.Length >= 0 && !GameProgressData.hasOpenedControlRoomDoor)
        {
            passwordInput = ""; // 입력 초기화
            feedbackText.text = "일반 사원은 접근할 수 없습니다.";
            feedbackText.color = Color.white;  
        }
    }
    
    // 키패드 버튼 연결
    public void KeyPadClickButton(string number)
    {
        if (passwordInput.Length < 10)
        {
            passwordInput += number;
        }
    }
  

    // 씬 전환 실행 함수
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[SceneChangeInteractable] 이동할 씬 이름(nextSceneName)이 설정되지 않았습니다!");
        }
    }
    
}
