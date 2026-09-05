using UnityEngine;

public class Scene1DialogueController : MonoBehaviour
{
    [SerializeField] private TextAsset sceneDialogueCSV;

   void Start()
    {
        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
    }

    // 편지 열람 후
    public void LetterDialogue()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.StartDialogueGroup(
            "FirstLetterOpen",
            () =>
            {
                Debug.Log("[Scene1] 편지 대사 종료");
            }
        );
    }

    // 모니터 열람 후
    public void MonitorDialogue()
    {
        if (DialogueManager.Instance == null) return;

        DialogueManager.Instance.StartDialogueGroup(
            "MoniterOpen",
            () =>
            {
                Debug.Log("[Scene1] 모니터 대사 종료");
            }
        );
    }

    // 지도를 안 본 상태에서 다음 맵 이동 시도

    public void MapRequiredDialogue(System.Action onFinished = null)
    {
        if (DialogueManager.Instance == null)
        {
            onFinished?.Invoke();
            return;
        }

        DialogueManager.Instance.StartDialogueGroup(
            "MapLocked",
            () =>
            {
                Debug.Log("[Scene1] 지도 확인 필요 대사 종료");
                onFinished?.Invoke();
            }
        );
    }
}