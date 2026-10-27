using UnityEngine;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance { get; private set; }

    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private MazeTimerSystem timerSystem;

    [Header("클리어 저장 시 이어하기 위치 (비우면 기본 위치)")]
    [SerializeField] private string saveSpawnPointName = "";

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
        Debug.Log("[GameManager] 성공!");

        // 클리어 여부 저장
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetSceneCleared(6, saveSpawnPointName);
        }
        // Time.timeScale = 0f; // 필요하면 게임 정지
    }

    public void OnFail()
    {
        if (isGameOver) return;
        isGameOver = true;
        failPanel.SetActive(true);
        Debug.Log("[GameManager] 실패!");
        // Time.timeScale = 0f; // 필요하면 게임 정지
    }
}
