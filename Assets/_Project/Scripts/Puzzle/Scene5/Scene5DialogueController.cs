using UnityEngine;

public class Scene5DialogueController : MonoBehaviour
{
    [Header("씬 대사 CSV 파일 연결")]
    [SerializeField] private TextAsset sceneDialogueCSV; 

    [Header("연결할 퍼즐")]
    [SerializeField] private PuzzleController targetPuzzle; // 퍼즐 컨트롤러 연결

    private void Start()
    {
        // 1. 해당 씬의 CSV 데이터베이스 로드
        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);

        // 2. 맵 진입 대사("Enter") 출력
        DialogueManager.Instance.StartDialogueGroup("Enter");
    }

    // 비밀번호 입력 성공 시 호출
    public void OnPuzzleCleared()
    {
        // 3. 퍼즐 클리어 대사("PuzzleClear") 출력 
        if (targetPuzzle.IsPuzzleCleared)
        {
            DialogueManager.Instance.StartDialogueGroup("PuzzleClear", () =>
            {
                Debug.Log("비상 통로 개방");
                // 4. 퍼즐 클리어 후 다음 방으로 이동 가능하도록 문 오브젝트 활성화
            });
        }
    }
}