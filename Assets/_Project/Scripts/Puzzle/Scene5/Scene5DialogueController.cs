using UnityEngine;
using System.Collections;

public class Scene5DialogueController : MonoBehaviour
{
     [Header("씬 대사 CSV 파일 연결")]
    [SerializeField] private TextAsset sceneDialogueCSV;

    [Header("퍼즐 컨트롤러 연결")]
    [SerializeField] private PuzzleController targetPuzzle;

    [Header("퍼즐 UI 연결")]
    [SerializeField] private ArchiveRoomPuzzle archiveRoomPuzzle;

    [Header("클리어 후 열릴 문")]
    [SerializeField] private GameObject nextRoomDoor;

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

     private void HandlePuzzleCleared()
    {
        // 퍼즐이 클리어 되었다는 로그를 먼저 확인
        Debug.Log("[Scene5] 퍼즐 클리어");
        StartCoroutine(WaitAndStartDialogue());
    }

    private IEnumerator WaitAndStartDialogue()
    {
        if (archiveRoomPuzzle != null)
        {
            while (archiveRoomPuzzle.gameObject.activeInHierarchy)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("[Scene5] ArchiveRoomPuzzle이 슬롯에 연결되지 않았습니다!");
        }

        
        // 2. 대사 시작
        DialogueManager.Instance.StartDialogueGroup("PuzzleClear", () =>
        {
            Debug.Log("[Scene5] 대사 종료 및 통로 개방");
            if (nextRoomDoor != null) nextRoomDoor.SetActive(true);
        });
    }
}