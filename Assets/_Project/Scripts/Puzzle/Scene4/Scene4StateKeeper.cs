using UnityEngine;

// 편지를 읽어서 도어락이 생겼는지를 기억했다가, 씬에 돌아오면 도어락을 다시 켜주는 스크립트.
// - 씬을 나갔다 돌아올 때: Scene4Progress(메모리)로 복구
// - 게임을 껐다 켜고 '이어하기'할 때: SaveManager(세이브 파일)로 복구
// 항상 켜져 있는 빈 오브젝트(예: StateKeeper)에 붙이세요.
public class Scene4StateKeeper : MonoBehaviour
{
    [Header("편지를 읽으면 생기는 도어락 오브젝트 (처음엔 꺼져 있는 것)")]
    [SerializeField] private GameObject doorLock;

    [Header("이어하기 시 플레이어가 나타날 위치 (씬 안에 같은 이름의 빈 오브젝트가 있어야 함)")]
    [SerializeField] private string saveSpawnPointName = "Spawn_Scene4_DoorLock";

    void Start()
    {
        bool savedOnDisk = SaveManager.Instance != null && SaveManager.Instance.Data.isScene4LetterRead;

        // 이전에 편지를 읽었다면 도어락을 바로 켜둠
        if ((Scene4Progress.letterRead || savedOnDisk) && doorLock != null)
        {
            doorLock.SetActive(true);
            Scene4Progress.letterRead = true;
        }
    }

    void Update()
    {
        // 도어락이 처음 켜지는 순간(= 편지를 읽고 닫은 순간)에 기록 + 세이브
        if (!Scene4Progress.letterRead && doorLock != null && doorLock.activeSelf)
        {
            Scene4Progress.letterRead = true;
            SaveToDisk();
        }
    }

    void SaveToDisk()
    {
        if (SaveManager.Instance == null) return;

        SaveManager.Instance.Data.isScene4LetterRead = true; // SaveData.cs에 추가한 변수
        SaveManager.Instance.SaveGame(saveSpawnPointName);
    }
}