using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoHomeButton : MonoBehaviour
{
    [SerializeField] private string dialogueGroupId = "GoHome";
    [SerializeField] private string homeSceneName = "Home";

    [Header("타이핑 완료 감지용 (여기 연결한 Viewer의 IsTyping을 관찰)")]
    [SerializeField] private DialogueViewer viewer;
    [Tooltip("타이핑이 끝난 뒤 자동으로 홈으로 넘어가기까지 읽을 시간(초)")]
    [SerializeField] private float readDelayAfterTyping = 1.5f;

    [Header("대사가 시작될 때 같이 숨길 오브젝트들 (버튼 자신은 자동으로 숨겨짐)")]
    [SerializeField] private GameObject[] additionalObjectsToHide;

    private bool sceneLoadTriggered;

    public void OnClickGoHome()
    {
        
        if (additionalObjectsToHide != null)
        {
            foreach (var obj in additionalObjectsToHide)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        sceneLoadTriggered = false;

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[GoHomeButton] DialogueManager.Instance가 없습니다. 바로 홈으로 이동합니다.");
            LoadHomeSceneOnce();
            return;
        }

        DialogueManager.Instance.StartDialogueGroup(dialogueGroupId, LoadHomeSceneOnce);

        if (viewer != null)
        {
            DialogueManager.Instance.StartCoroutine(AutoGoHomeRoutine());
        }
        else
        {
            Debug.LogWarning("[GoHomeButton] Viewer가 연결되지 않아 자동 전환을 쓸 수 없습니다. 클릭으로만 넘어갑니다.");
        }

        gameObject.SetActive(false);
    }

    private IEnumerator AutoGoHomeRoutine()
    {
        yield return null;

        while (viewer != null && viewer.IsTyping)
        {
            yield return null;
        }

        yield return new WaitForSeconds(readDelayAfterTyping);

        LoadHomeSceneOnce();
    }

    private void LoadHomeSceneOnce()
    {
        if (sceneLoadTriggered) return;
        sceneLoadTriggered = true;
        SceneManager.LoadScene(homeSceneName);
    }
}