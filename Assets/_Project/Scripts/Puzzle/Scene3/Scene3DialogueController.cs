using UnityEngine;
using System.Collections;

public class Scene3DialogueController : MonoBehaviour
{
    [Header("씬 대사 CSV 파일 연결")]
    [SerializeField] private TextAsset sceneDialogueCSV;

    [Header("레버(동시 토글) 퍼즐 컨트롤러 연결")]
    [SerializeField] private ToggleMatrix targetPuzzle;

    [Header("퍼즐 UI 연결 (닫히는 애니메이션을 기다릴 오브젝트, 없으면 비워둬도 됨)")]
    [SerializeField] private GameObject puzzleUI;

    [Header("메인 콘솔 매니저 연결")]
    [SerializeField] private MainConsoleManager mainConsoleManager;

    [Header("클리어 후 열릴 문 (중앙 통제실 -> 메인 기록실)")]
    [SerializeField] private GameObject nextRoomDoor;

    // PuzzleLocked 연출은 최초 1회만 재생
    private bool hasShownLockedDialogue = false;

    private IEnumerator Start()
    {
        yield return null; // 한 프레임 대기 후 시작

        if (DialogueManager.Instance == null) yield break;
        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
        DialogueManager.Instance.StartDialogueGroup("Enter");
    }

    private void OnEnable()
    {
        if (targetPuzzle != null) targetPuzzle.OnPuzzleClearedEvent += HandlePuzzleCleared;
    }

    private void OnDisable()
    {
        if (targetPuzzle != null) targetPuzzle.OnPuzzleClearedEvent -= HandlePuzzleCleared;
    }

    // 메인 콘솔을 인터랙트했을 때 호출.
    public void OnMainConsoleInteract()
    {
        // 이미 퍼즐을 클리어한 상태면 가스라이팅 연출 없이 바로 정상 콘솔 화면을 보여줌
        if (mainConsoleManager != null && mainConsoleManager.isPuzzleCleared)
        {
            mainConsoleManager.RefreshConsoleScreen();
            return;
        }

        // 아직 클리어 전이고, 잠금 경고 연출을 아직 보여준 적 없다면 1회 재생
        if (!hasShownLockedDialogue && DialogueManager.Instance != null)
        {
            hasShownLockedDialogue = true;
            DialogueManager.Instance.StartDialogueGroup("PuzzleLocked");
        }
    }

    private void HandlePuzzleCleared()
    {
        Debug.Log("[Scene3] 레버 퍼즐 클리어");

        StartCoroutine(WaitAndStartDialogue());
    }

    private IEnumerator WaitAndStartDialogue()
    {
        if (puzzleUI != null)
        {
            while (puzzleUI.activeInHierarchy)
            {
                yield return null;
            }
        }

        // 콘솔 화면을 클리어 상태로 전환
        if (mainConsoleManager != null)
        {
            mainConsoleManager.RefreshConsoleScreen();
        }

        DialogueManager.Instance.StartDialogueGroup("PuzzleClear", () =>
        {
            Debug.Log("[Scene3] 대사 종료 및 메인 기록실 통로 개방");
            if (nextRoomDoor != null) nextRoomDoor.SetActive(true);
        });
    }
}