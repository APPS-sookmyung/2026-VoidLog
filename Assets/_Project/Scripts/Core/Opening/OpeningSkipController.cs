using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSkipController : MonoBehaviour
{
    private const string SeenKey = "Opening_Seen";

    [Header("스킵 시 이동할 씬 이름")]
    [SerializeField] private string nextSceneName = "Home";

    private void Awake()
    {
        bool hasSeenBefore = PlayerPrefs.GetInt(SeenKey, 0) == 1;

        gameObject.SetActive(hasSeenBefore);
    }

    // 버튼의 On Click()에서 연결
    public void OnClickSkip()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // OpeningStepManager의 onSequenceComplete에서 연결 
    public void MarkOpeningSeen()
    {
        PlayerPrefs.SetInt(SeenKey, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }

    // 테스트/디버그용
    public void ResetSeenFlag_DebugOnly()
    {
        PlayerPrefs.DeleteKey(SeenKey);
        PlayerPrefs.Save();
    }
}
