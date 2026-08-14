using UnityEngine;

public class MazeGameManager : MonoBehaviour
{
    public static MazeGameManager Instance { get; private set; }

    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private MazeTimerSystem timerSystem;

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
