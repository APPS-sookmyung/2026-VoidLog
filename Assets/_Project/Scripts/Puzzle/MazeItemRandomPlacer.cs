using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// maze CSV를 읽어서 "길"(값이 0인 칸) 중에서 랜덤하게 위치를 뽑아
/// items 리스트에 넣은 오브젝트들을 그 자리로 이동시킨다.
/// 시작점/출구 근처, 아이템끼리 너무 가까운 위치는 제외한다.
/// </summary>
public class MazeItemRandomPlacer : MonoBehaviour
{
    [Header("데이터")]
    [Tooltip("MazeBuilder2D에 넣은 것과 같은 CSV 파일")]
    [SerializeField] private TextAsset mazeCsv;
    [SerializeField] private float cellSize = 1.5f;

    [Header("배치할 아이템들")]
    [Tooltip("Part_01, Part_02, Part_03 오브젝트를 순서대로 드래그")]
    [SerializeField] private List<Transform> items;

    [Header("배치 제외 범위 (칸 단위, 맨해튼 거리)")]
    [SerializeField] private int minDistanceFromStart = 5;
    [SerializeField] private int minDistanceFromExit = 5;
    [SerializeField] private int minDistanceBetweenItems = 5;

    private int width;
    private int height;

    private void Awake()
    {
        PlaceItemsRandomly();
    }

    private void PlaceItemsRandomly()
    {
        if (mazeCsv == null || items == null || items.Count == 0)
        {
            Debug.LogWarning("[MazeItemRandomPlacer] mazeCsv 또는 items가 비어있습니다.");
            return;
        }

        List<Vector2Int> openCells = GetOpenCells(); // x = col, y = row

        Vector2Int startCell = new Vector2Int(1, 1);
        Vector2Int exitCell = new Vector2Int(width - 1, height - 2);

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

            // 조건에 맞는 후보가 없으면(맵이 작을 때 등) 거리 조건을 무시하고 남은 칸에서 뽑음
            if (candidates.Count == 0) candidates = openCells;
            if (candidates.Count == 0) break; // 그래도 없으면 포기

            Vector2Int cell = candidates[Random.Range(0, candidates.Count)];
            chosen.Add(cell);
            openCells.Remove(cell); // 같은 칸에 두 아이템이 겹치지 않도록

            item.position = new Vector3(cell.x * cellSize, -cell.y * cellSize, item.position.z);
        }

        Debug.Log($"[MazeItemRandomPlacer] 아이템 {chosen.Count}개 배치 완료: {string.Join(", ", chosen)}");
    }

    private List<Vector2Int> GetOpenCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        string[] lines = mazeCsv.text.Split('\n');

        int y = 0;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] values = line.Trim().Split(',');
            width = values.Length; // 마지막 줄 기준으로 갱신됨 (모든 줄 길이가 같다는 전제)

            for (int x = 0; x < values.Length; x++)
            {
                if (values[x] == "0")
                    cells.Add(new Vector2Int(x, y));
            }
            y++;
        }

        height = y;
        return cells;
    }

    private int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}
