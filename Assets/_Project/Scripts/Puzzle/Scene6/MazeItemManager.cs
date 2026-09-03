using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 씬에 빈 오브젝트 하나 만들어서 붙이는 싱글톤.
/// 몇 개를 모았는지, 다 모았는지를 여기서 관리.
/// itemCountText를 연결하면 "0/3" 같은 형태로 화면에 표시.
/// </summary>
public class MazeItemManager : MonoBehaviour
{
    public static MazeItemManager Instance { get; private set; }

    [SerializeField] private int totalItemCount = 3;
    [Tooltip("Canvas 안의 TMP 텍스트를 드래그 (예: '획득한 부품: 0/3' 표시용)")]
    [SerializeField] private TMP_Text itemCountText;

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

    private void Start()
    {
        UpdateUI(); // 시작할 때 "0/3"부터 표시
    }

    public void CollectItem(string itemId)
    {
        if (collectedItems.Add(itemId))
        {
            Debug.Log($"[ItemManager] 획득: {itemId} ({collectedItems.Count}/{totalItemCount})");
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (itemCountText == null) return;
        itemCountText.text = $"{collectedItems.Count}/{totalItemCount}";
    }

    public bool HasAllItems() => collectedItems.Count >= totalItemCount;
    public int CollectedCount => collectedItems.Count;
    public int TotalCount => totalItemCount;
}