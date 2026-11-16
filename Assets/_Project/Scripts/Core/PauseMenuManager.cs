using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuManager : MonoBehaviour
{

    public static PauseMenuManager Instance { get; private set; }
    [SerializeField] private  GameObject pausePanel;

    private bool isPaused = false;

    // 어떤 씬에서 시작해도 자동으로 PauseManager 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("PauseMenuManager");

        if (prefab != null)
        {
            Instantiate(prefab);
        }
        else
        {
            Debug.LogError(
                "[PauseManager] Resources/PauseMenuManager 프리팹을 찾을 수 없습니다."
            );
        }
    }
      private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) // 열려 있으면 닫기
            {
                ResumeGame();
            }
            else
            {
                OpenPauseMenu();
            }
        }   
    }

    public void OpenPauseMenu()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void GoHome()
    {
        isPaused = false;
        
        if (pausePanel != null)
        pausePanel.SetActive(false);
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("Home");
    }
}
