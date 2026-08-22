using UnityEngine;

public class Scene5DialogueController : MonoBehaviour
{
    [Header("씬 대사 CSV 파일 연결")]
    [SerializeField] private TextAsset sceneDialogueCSV;

    [Header("퍼즐 컨트롤러 연결")]
    [SerializeField] private PuzzleController targetPuzzle;

   private void Start()
{
    // 1. DialogueManager 싱글톤 체크
    if (DialogueManager.Instance == null)
    {
        Debug.LogError("[에러] 씬에 DialogueManager가 존재하지 않습니다. Hierarchy에 DialogueManager를 올려주세요.");
        return;
    }

    // 2. CSV 파일 연결 체크
    if (sceneDialogueCSV == null)
    {
        Debug.LogError("[에러] Scene5DialogueController의 'Scene Dialogue CSV' 슬롯이 비어 있습니다. CSV 파일을 인스펙터에 넣어주세요.");
        return;
    }

    // 3. 정상 실행
    DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
    DialogueManager.Instance.StartDialogueGroup("Enter");
}

    private void OnEnable()
    {
        // 오브젝트 활성화 시 이벤트 구독
        if (targetPuzzle != null)
        {
            targetPuzzle.OnPuzzleClearedEvent += HandlePuzzleCleared;
        }
    }

    private void OnDisable()
    {
        // 오브젝트 비활성화 시 메모리 누수 방지를 위한 구독 해제
        if (targetPuzzle != null)
        {
            targetPuzzle.OnPuzzleClearedEvent -= HandlePuzzleCleared;
        }
    }

    // 퍼즐 클리어 이벤트가 발생하면 자동으로 실행되는 메서드
    private void HandlePuzzleCleared()
    {
        DialogueManager.Instance.StartDialogueGroup("PuzzleClear", () =>
        {
            Debug.Log("비상 통로 개방");
            // 다음 방 이동 문 오브젝트 활성화 로직
        });
    }
}