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
        DontDestroyOnLoad(gameObject);
         Debug.Log("SaveManager 최초 생성");

        savePath = Path.Combine(Application.persistentDataPath,"save.json");

        Data = new SaveData();

        // 처음에는 저장 UI 숨기기
        if (saveCanvas != null)
        {
            saveCanvas.gameObject.SetActive(false);
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
   


    //저장 UI
    private void ShowSaveUI()
    {
        if (saveCanvas == null)
            return;

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
}