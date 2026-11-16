using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 반드시 필요!

public class SceneChangeInteractable : MonoBehaviour
{
    [Header("[E] 안내 문구 UI")]
    [Tooltip("상호작용 영역에 들어왔을 때 띄울 안내 텍스트 (ex. [E] 이동)")]
    [SerializeField] private GameObject clickText;

    [Header("이동할 씬 설정")]
    [Tooltip("Build Settings에 등록된 이동하고자 하는 씬의 정확한 이름")]
    [SerializeField] private string nextSceneName;

    private bool isPlayerNearby = false;

    private void Start()
    {
        // 시작할 때 안내 문구 가림
        if (clickText != null)
        {
            clickText.SetActive(false);
        }
    }

    private void Update()
    {
        // 플레이어가 콜라이더 안에 있고 E키를 눌렀을 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextScene();
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

    // 콜라이더 감지 (Is Trigger가 체크된 BoxCollider2D 필요)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (clickText != null) clickText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (clickText != null) clickText.SetActive(false);
        }
    }
}