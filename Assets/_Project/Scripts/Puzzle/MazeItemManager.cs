using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬에 빈 오브젝트 하나 만들어서 붙이는 싱글톤.
/// 몇 개를 모았는지, 다 모았는지를 여기서 관리.
/// </summary>
public class MazeItemManager : MonoBehaviour
{
    public static MazeItemManager Instance { get; private set; }

    [SerializeField] private int totalItemCount = 3;
    private readonly HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void CollectItem(string itemId)
    {
        if (collectedItems.Add(itemId))
        {
            Debug.Log($"[ItemManager] 획득: {itemId} ({collectedItems.Count}/{totalItemCount})");
        }
    }

    public bool HasAllItems() => collectedItems.Count >= totalItemCount;
    public int CollectedCount => collectedItems.Count;
    public int TotalCount => totalItemCount;
}
