using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // 씬 이동/로드에 필요한 네임스페이스

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance { get; private set; }

    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private MazeTimerSystem timerSystem;

    [Header("클리어 저장 시 이어하기 위치 (비우면 기본 위치)")]
    [SerializeField] private string saveSpawnPointName = "";

    [Header("씬 전환 설정 (새로 추가됨)")]
    [Tooltip("미로 성공 시 이동할 다음 씬의 이름")]
    [SerializeField] private string nextSceneName = "";
    [Tooltip("씬이 전환되거나 재시작되기 전 대기 시간 (초)")]
    [SerializeField] private float loadDelayTime = 1.5f;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnSuccess()
    {
        if (isGameOver) return;
        isGameOver = true;
        timerSystem?.StopTimer();
        successPanel.SetActive(true);
        Debug.Log("[GameManager] 성공! 다음 씬으로 이동합니다.");

        // 클리어 여부 저장
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetSceneCleared(6, saveSpawnPointName);
        }

        // 일정 시간 대기 후 다음 씬으로 이동
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    public void OnFail()
    {
        if (isGameOver) return;
        isGameOver = true;
        failPanel.SetActive(true);
        Debug.Log("[GameManager] 실패! 씬을 다시 로드합니다.");

        // 일정 시간 대기 후 현재 씬 재시작
        StartCoroutine(RestartSceneAfterDelay());
    }

    // 일정 시간 대기 후 다음 씬 로드 코루틴
    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelayTime);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("다음 씬 이름(nextSceneName)이 비어있습니다! 인스펙터 창에서 이동할 씬 이름을 입력해주세요.");
        }
    }

    // 일정 시간 대기 후 현재 씬 재시작 코루틴
    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(loadDelayTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}