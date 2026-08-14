using UnityEngine;

/// <summary>
/// 2D 버전 미로 빌더. wallPrefab은 SpriteRenderer + Box Collider 2D(트리거 아님)가
/// 붙은 프리팹이어야 함.
/// </summary>
public class MazeBuilder : MonoBehaviour
{
    [Header("데이터")]
    [Tooltip("Assets/_Project/Data/MazeData 안의 CSV를 드래그")]
    [SerializeField] private TextAsset mazeCsv;

    [Header("배치 설정")]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Transform wallParent;

    private void Awake()
    {
        BuildMaze();
    }

    private void BuildMaze()
    {
        if (mazeCsv == null)
        {
            Debug.LogError("[MazeBuilder2D] mazeCsv가 비어있습니다.");
            return;
        }

        string[] lines = mazeCsv.text.Split('\n');
        int wallCount = 0;

        for (int y = 0; y < lines.Length; y++)
        {
            if (string.IsNullOrWhiteSpace(lines[y])) continue;
            string[] cells = lines[y].Trim().Split(',');

            for (int x = 0; x < cells.Length; x++)
            {
                if (cells[x] == "1") // 벽
                {
                    // 2D는 보통 X-Y 평면을 씀. 아래로 갈수록 y가 줄어들도록 -y 사용.
                    Vector3 pos = new Vector3(x * cellSize, -y * cellSize, 0f);
                    GameObject wall = Instantiate(wallPrefab, pos, Quaternion.identity, wallParent);
                    wall.isStatic = true; // 스태틱 배칭
                    wallCount++;
                }
            }
        }

        Debug.Log($"[MazeBuilder2D] 벽 {wallCount}개 배치 완료.");
    }
}

/*
 * 벽 프리팹 만드는 법 (2D)
 * 1. Hierarchy 우클릭 → 2D Object → Sprite → Square 선택
 * 2. Inspector에서 Add Component → Box Collider 2D 추가, Is Trigger 체크 해제
 * 3. Sprite Renderer의 색상/이미지를 원하는 벽 모양으로 설정
 * 4. 이 오브젝트를 Project 창으로 드래그해서 프리팹으로 저장
 * 5. cellSize는 이 스프라이트의 실제 크기(Transform Scale 기준 월드 유닛)와 맞추기
 */
