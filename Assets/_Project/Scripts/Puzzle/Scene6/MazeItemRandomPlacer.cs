using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// MazeBuilder.Instance의 격자 정보를 그대로 사용해서 아이템을 랜덤 배치.
/// (더 이상 CSV를 직접 읽지 않음 - MazeBuilder가 이미 읽어둔 데이터를 가져다 씀)
/// MazeBuilder가 먼저 실행되어야 하므로 Awake가 아니라 Start에서 실행함
/// (모든 오브젝트의 Awake가 끝난 뒤 Start가 실행되므로 순서 걱정 없음).
/// </summary>
public class MazeItemRandomPlacer : MonoBehaviour
{
    [Header("배치할 아이템들")]
    [Tooltip("Part_01, Part_02, Part_03 오브젝트를 순서대로 드래그")]
    [SerializeField] private List<Transform> items;

    [Header("배치 제외 범위 (칸 단위, 맨해튼 거리)")]
    [SerializeField] private int minDistanceFromStart = 5;
    [SerializeField] private int minDistanceFromExit = 5;
    [SerializeField] private int minDistanceBetweenItems = 5;

    private void Start()
    {
        PlaceItemsRandomly();
    }

    private void PlaceItemsRandomly()
    {
        MazeBuilder maze = MazeBuilder.Instance;
        if (maze == null || items == null || items.Count == 0)
        {
            Debug.LogWarning("[MazeItemRandomPlacer] MazeBuilder 또는 items가 비어있습니다.");
            return;
        }

        List<Vector2Int> openCells = new List<Vector2Int>();
        for (int r = 0; r < maze.Height; r++)
            for (int c = 0; c < maze.Width; c++)
                if (maze.GetCellValue(c, r) == 0)
                    openCells.Add(new Vector2Int(c, r));

        Vector2Int startCell = new Vector2Int(1, 1);
        Vector2Int exitCell = new Vector2Int(maze.Width - 1, maze.Height - 2);

        openCells.RemoveAll(cell =>
            ManhattanDistance(cell, startCell) < minDistanceFromStart ||
            ManhattanDistance(cell, exitCell) < minDistanceFromExit);

        List<Vector2Int> chosen = new List<Vector2Int>();

        foreach (Transform item in items)
        {
            if (item == null) continue;

            List<Vector2Int> candidates = openCells
                .Where(c => chosen.All(picked => ManhattanDistance(c, picked) >= minDistanceBetweenItems))
                .ToList();

            if (candidates.Count == 0) candidates = openCells;
            if (candidates.Count == 0) break;

            Vector2Int cell = candidates[Random.Range(0, candidates.Count)];
            chosen.Add(cell);
            openCells.Remove(cell);

            Vector3 pos = maze.GetCellCenter(cell.x, cell.y);
            item.position = new Vector3(pos.x, pos.y, item.position.z);
        }

        Debug.Log($"[MazeItemRandomPlacer] 아이템 {chosen.Count}개 배치 완료: {string.Join(", ", chosen)}");
    }

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}