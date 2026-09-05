using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlRoomDoor : MonoBehaviour
{
    [Header("대사")]
    [SerializeField] private Scene2DialogueController dialogueController;

    [Header("도어락")]
    [SerializeField] private Canvas doorLockCanvas;
    [SerializeField] private TextMeshProUGUI escText;

    [Header("Settings")]
    [SerializeField] private string correctPassword = "123456789";
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("이동할 씬 설정")]
    [SerializeField] private string nextSceneName;

    private string passwordInput = "";
    private bool isDialogueRunning;
    private bool hasSeenFirstDialogue;

    private void Start()
    {
        doorLockCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!GameProgressData.hasOpenedControlRoomDoor &&
            doorLockCanvas.gameObject.activeSelf)
        {
            if (passwordInput != "")
            {
                feedbackText.color = Color.white;
                feedbackText.text = "입력 중...";
            }

            if (passwordInput.Length == 9)
            {
                OnSubmitPassword(passwordInput);
            }
        }

        if (doorLockCanvas.gameObject.activeSelf)
        {
            CloseDoor();
        }
    }

    public void DoorCheck()
    {
        if (!GameProgressData.hasOpenedControlRoomDoor)
        {
            passwordInput = "";

            statusText.text = "잠김";
            feedbackText.text = "일반 사원은 접근할 수 없습니다.";
            feedbackText.color = Color.white;

            if (!hasSeenFirstDialogue && !isDialogueRunning)
            {
                ShowFirstDialogue();
            }
        }
        else
        {
            doorLockCanvas.gameObject.SetActive(false);
            LoadNextScene();
        }
    }

    private void ShowFirstDialogue()
    {
        isDialogueRunning = true;
        hasSeenFirstDialogue = true;

        // 도어락 작게
        SetDoorLockDialogueMode(true);

        dialogueController.FirstDoorDialogue(() =>
        {
            // 대사 끝남
            SetDoorLockDialogueMode(false);

            isDialogueRunning = false;
        });
    }
    private void ShowClearDialogue()
    {
        isDialogueRunning = true;

        SetDoorLockDialogueMode(true);

        dialogueController.DoorClearDialogue(() =>
        {
            SetDoorLockDialogueMode(false);

            isDialogueRunning = false;
        });
    }

    private void OnSubmitPassword(string inputPassword)
    {
        if (inputPassword == correctPassword)
        {
            GameProgressData.hasOpenedControlRoomDoor = true;

            feedbackText.color = Color.white;
            feedbackText.text = "인증이 완료되었습니다.";

            statusText.text = "열림";
            statusText.color = Color.blue;

            isDialogueRunning = true;
            escText.gameObject.SetActive(false);

            ShowClearDialogue();
        }
        else
        {
            ShowError("인증 실패: 보안 권한이 거부되었습니다.");
        }
    }

    private void ShowError(string message)
    {
        passwordInput = "";

        feedbackText.color = Color.red;
        feedbackText.text = message;
    }

    public void DeletePasswordNumber()
    {
        if (!GameProgressData.hasOpenedControlRoomDoor)
        {
            passwordInput = "";

            feedbackText.text = "일반 사원은 접근할 수 없습니다.";
            feedbackText.color = Color.white;
        }
    }

    public void KeyPadClickButton(string number)
    {
        if (passwordInput.Length < 9 &&
            !GameProgressData.hasOpenedControlRoomDoor)
        {
            passwordInput += number;
        }
    }

    public void CloseDoor()
    {
        if (isDialogueRunning)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.E))
        {
            doorLockCanvas.gameObject.SetActive(false);

            if (GameProgressData.hasOpenedControlRoomDoor)
            {
                LoadNextScene();
            }
        }
    }

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // 대사중에 도어락 크기 축소
    private void SetDoorLockDialogueMode(bool isDialogueMode)
    {
        RectTransform canvasRect = doorLockCanvas.GetComponent<RectTransform>();

        if (isDialogueMode)
        {
            // 대사 중
            escText.gameObject.SetActive(false);

            doorLockCanvas.transform.localScale =
                new Vector3(0.74f, 0.74f, 0.74f);

            canvasRect.offsetMin = new Vector2(-7f, 168f);
            canvasRect.offsetMax = new Vector2(-7f, 168f);
        }
        else
        {
            // 대사 종료
            escText.gameObject.SetActive(true);

            doorLockCanvas.transform.localScale = Vector3.one;

            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;
        }
    }
}