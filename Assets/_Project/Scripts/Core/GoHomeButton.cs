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
        // 버튼 자신을 바로 비활성화하지 않고, 버튼의 Graphic(Image)과 컴포넌트만 끄거나 
        // 혹은 코루틴을 다른 활성화된 오브젝트(예: DialogueManager 등)에서 실행하도록 분리해야 합니다.
        // 여기서는 버튼 오브젝트 자체를 끄는 대신 내부 컴포넌트나 자식 이미지만 숨기는 방식을 추천합니다.
        
        // 만약 꼭 버튼 오브젝트를 꺼야 한다면, 아래처럼 코루틴 실행 주체를 변경해야 합니다.
        
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
            // ★ 수정: 버튼(자기 자신)이 꺼지더라도 코루틴이 끊기지 않도록 
            // 씬에 항상 살아있는 DialogueManager나 다른 활성 오브젝트를 통해 코루틴을 실행합니다.
            DialogueManager.Instance.StartCoroutine(AutoGoHomeRoutine());
        }
        else
        {
            Debug.LogWarning("[GoHomeButton] Viewer가 연결되지 않아 자동 전환을 쓸 수 없습니다. 클릭으로만 넘어갑니다.");
        }

        // 버튼 자신은 코루틴을 예약한 뒤에 끕니다.
        gameObject.SetActive(false);
    }

    private IEnumerator AutoGoHomeRoutine()
    {
        yield return null; // 타이핑이 실제로 시작될 시간을 한 프레임 준다

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