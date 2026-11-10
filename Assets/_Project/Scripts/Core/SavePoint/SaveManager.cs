using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData Data { get; private set; }

    private string savePath;


    [Header("자동 저장 UI")]
    [SerializeField] private Canvas saveCanvas;
    [SerializeField] private float saveUIShowTime = 2f;

    private Coroutine saveUICoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("중복 SaveManager 삭제");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 혹시 다른 오브젝트의 자식으로 생성됐으면 루트로 빼기
        transform.SetParent(null);
        
        DontDestroyOnLoad(gameObject);

         Debug.Log("SaveManager 최초 생성");

        savePath = Path.Combine(Application.persistentDataPath,"save.json");

        Data = new SaveData();
        
        // 자식에 있는 SaveCanvas 자동 탐색
        if (saveCanvas == null)
        {
            saveCanvas = GetComponentInChildren<Canvas>(true);
        }
        // 처음에는 저장 UI 숨기기
        if (saveCanvas != null)
        {
            saveCanvas.gameObject.SetActive(false);
        }else
        {
            Debug.LogError("SaveCanvas를 찾을 수 없습니다.");
        }

        
    }

    //저장
    public void SaveGame(string spawnPointName)
    {
        Data.sceneName = SceneManager.GetActiveScene().name;
        Data.spawnPointName = spawnPointName;

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("게임 저장 완료 : " + savePath);

        // 저장 완료 UI 출력
        ShowSaveUI();
        Debug.Log("TimeScale : " + Time.timeScale);
    }

    //불러오기
    public bool LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("저장 파일이 없습니다.");

            Data = new SaveData();
            return false;
        }

        string json = File.ReadAllText(savePath);

        Data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("게임 데이터 불러오기 완료");

        return true;
    }

    // 이어하기
    public void ContinueGame()
    {
        if (!LoadGame())
            return;

        SceneTransitionData.spawnPointName = Data.spawnPointName;

        SceneManager.LoadScene(Data.sceneName);
    }


    // 처음부터 / 초기화

    public void NewGame()
    {
        ResetData();

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        SceneTransitionData.spawnPointName = "";

        SceneManager.LoadScene("Scene_01_Workshop");
    }

    public void ResetData()
    {
        Data = new SaveData();

        Debug.Log("게임 진행 데이터 초기화");
    }

    // 씬 클리어 여부 (Scene 3 ~ 6)

    // 씬 클리어 여부 확인
    public bool IsSceneCleared(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 3: return Data.isScene3Cleared;
            case 4: return Data.isScene4Cleared;
            case 5: return Data.isScene5Cleared;
            case 6: return Data.isScene6Cleared;
            default:
                Debug.LogWarning("클리어 여부가 없는 씬 번호입니다 : " + sceneNumber);
                return false;
        }
    }

    // 씬 클리어 처리 후 바로 저장
    public void SetSceneCleared(int sceneNumber, string spawnPointName = "")
    {
        switch (sceneNumber)
        {
            case 3: Data.isScene3Cleared = true; break;
            case 4: Data.isScene4Cleared = true; break;
            case 5: Data.isScene5Cleared = true; break;
            case 6: Data.isScene6Cleared = true; break;
            default:
                Debug.LogWarning("클리어 여부가 없는 씬 번호입니다 : " + sceneNumber);
                return;
        }

        Debug.Log("Scene " + sceneNumber + " 클리어 저장");
        SaveGame(spawnPointName);
    }
   


    //저장 UI
    private void ShowSaveUI()
    {
        Debug.Log("저장 UI 실행");

        if (saveCanvas == null)
        {
            Debug.LogError("SaveCanvas 없음");
            return;
        }    

        // 저장이 연속으로 발생했을 경우 기존 타이머 취소
        if (saveUICoroutine != null)
        {
            StopCoroutine(saveUICoroutine);
        }

        saveUICoroutine = StartCoroutine(SaveUICoroutine());
    }

    private IEnumerator SaveUICoroutine()
    {
        if (saveCanvas == null) yield break;

        saveCanvas.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(saveUIShowTime);

        // 기다리는 동안 Canvas가 사라졌을 수도 있으므로 다시 확인
        if (saveCanvas != null)
        {
            saveCanvas.gameObject.SetActive(false);
        }

        saveUICoroutine = null;
    }

    //저장 파일 확인
    public bool HasSaveData()
    {
        return File.Exists(savePath);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateSaveManager()
    {
        // 이미 존재하면 생성 안 함
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>("SaveManager");

        if (prefab == null)
        {
            Debug.LogError("SaveManager 프리팹을 찾을 수 없습니다.");
            return;
        }

        Instantiate(prefab);
    }
}