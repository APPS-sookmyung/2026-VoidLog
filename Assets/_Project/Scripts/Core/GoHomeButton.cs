using UnityEngine;
using UnityEngine.SceneManagement;

public class GoHomeButton : MonoBehaviour
{
    [SerializeField] private string dialogueGroupId = "Replay";
    [SerializeField] private string homeSceneName = "Home";

    [Header("대사가 시작될 때 같이 숨길 오브젝트들 (버튼 자신은 자동으로 숨겨짐)")]
    [SerializeField] private GameObject[] additionalObjectsToHide;

    public void OnClickGoHome()
    {
        gameObject.SetActive(false);

        if (additionalObjectsToHide != null)
        {
            foreach (var obj in additionalObjectsToHide)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        if (OpeningDialogueManager.Instance == null)
        {
            Debug.LogWarning("[GoHomeButton] OpeningDialogueManager.Instance가 없습니다. 바로 홈으로 이동합니다.");
            LoadHomeScene();
            return;
        }

        OpeningDialogueManager.Instance.StartDialogueGroup(dialogueGroupId, LoadHomeScene);
    }

    private void LoadHomeScene()
    {
        SceneManager.LoadScene(homeSceneName);
    }
}