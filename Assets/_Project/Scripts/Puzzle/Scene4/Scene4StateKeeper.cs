using UnityEngine;

// 편지를 읽어서 도어락이 생겼는지를 기억했다가, 씬에 돌아오면 도어락을 다시 켜주는 스크립트.
// 항상 켜져 있는 빈 오브젝트(예: StateKeeper)에 붙이세요.
public class Scene4StateKeeper : MonoBehaviour
{
    [Header("편지를 읽으면 생기는 도어락 오브젝트 (처음엔 꺼져 있는 것)")]
    [SerializeField] private GameObject doorLock;

    void Start()
    {
        // 이전에 편지를 읽었다면 도어락을 바로 켜둠
        if (Scene4Progress.letterRead && doorLock != null)
        {
            doorLock.SetActive(true);
        }
    }

    void Update()
    {
        // 도어락이 켜지는 순간(= 편지를 읽고 닫은 순간)을 기록
        if (!Scene4Progress.letterRead && doorLock != null && doorLock.activeSelf)
        {
            Scene4Progress.letterRead = true;
        }
    }
}