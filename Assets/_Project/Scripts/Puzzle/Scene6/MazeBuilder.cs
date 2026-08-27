using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 길(홀수 칸)은 넓게, 벽(짝수 칸)은 얇게 그리는 미로 빌더.
/// wallPrefab은 1×1 유닛 기준 스프라이트여야 함 (프리팹 자체 Transform Scale은
/// (1,1,1)로 초기화해둘 것 - 이 스크립트가 칸마다 알맞은 크기로 Scale을 직접 계산해서 넣어줌).
/// </summary>
public class MazeBuilder : MonoBehaviour
{
    public static MazeBuilder Instance { get; private set; }

    [Header("데이터")]
    [Tooltip("Assets/_Project/Data/MazeData 안의 CSV를 드래그")]
    [SerializeField] private TextAsset mazeCsv;

    [Header("배치 설정")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Transform wallParent;

    [Header("크기 설정")]
    [Tooltip("길(홀수 칸) 폭 - 플레이어가 지나다니는 공간. 크게 잡을수록 시원해 보임")]
    [SerializeField] private float pathSize = 2f;
    [Tooltip("벽(짝수 칸) 두께. 작게 잡을수록 얇아짐")]
    [SerializeField] private float wallThickness = 0.5f;

    private int[,] grid;
    public int Width { get; private set; }
    public int Height { get; private set; }

    private void Awake()
    {
        Instance = this;
        ParseCsv();
        BuildMaze();
    }

    private void ParseCsv()
    {
        if (mazeCsv == null)
        {
            Debug.LogError("[MazeBuilder] mazeCsv가 비어있습니다.");
            return;
        }

        string[] lines = mazeCsv.text.Split('\n');
        List<int[]> rows = new List<int[]>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] values = line.Trim().Split(',');
            int[] rowValues = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
                rowValues[i] = values[i] == "1" ? 1 : 0;
            rows.Add(rowValues);
        }

        Height = rows.Count;
        Width = rows.Count > 0 ? rows[0].Length : 0;
        grid = new int[Height, Width];

        for (int r = 0; r < Height; r++)
            for (int c = 0; c < Width; c++)
                grid[r, c] = rows[r][c];
    }

    private void BuildMaze()
    {
        int wallCount = 0;

        for (int r = 0; r < Height; r++)
        {
            for (int c = 0; c < Width; c++)
            {
                if (grid[r, c] != 1) continue;

                Vector3 pos = GetCellCenter(c, r);
                GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, wallParent);
                wall.transform.localScale = new Vector3(SizeOf(c), SizeOf(r), 1f);
                wall.isStatic = true;
                wallCount++;
            }
        }

        Debug.Log($"[MazeBuilder] 벽 {wallCount}개 배치 완료. (pathSize={pathSize}, wallThickness={wallThickness})");
    }

    /// 짝수 인덱스 = 벽 두께, 홀수 인덱스 = 길 폭
    private float SizeOf(int index) => (index % 2 == 0) ? wallThickness : pathSize;

    /// index 앞쪽까지 누적된 거리 (왼쪽/위쪽 경계 좌표)
    private float EdgeOf(int index)
    {
        int evenCount = (index + 1) / 2; // 0..index-1 구간의 짝수 칸 개수
        int oddCount = index / 2;        // 0..index-1 구간의 홀수 칸 개수
        return evenCount * wallThickness + oddCount * pathSize;
    }

    /// 격자 좌표(col, row)의 월드 중심 좌표
    public Vector3 GetCellCenter(int col, int row)
    {
        float x = EdgeOf(col) + SizeOf(col) / 2f;
        float y = -(EdgeOf(row) + SizeOf(row) / 2f);
        return new Vector3(x, y, 0f);
    }

    /// 미로 전체의 월드 크기 (가로, 세로) - 카메라 사이즈 계산용
    public Vector2 GetTotalWorldSize()
    {
        return new Vector2(EdgeOf(Width), EdgeOf(Height));
    }

    /// 해당 칸이 벽(1)인지 길(0)인지
    public int GetCellValue(int col, int row) => grid[row, col];
}