using UnityEngine;

/// <summary>
/// 미로가 만들어진 뒤, 플레이어 시작 위치 / 카메라 위치·사이즈 / 출구 위치를
/// pathSize, wallThickness 값을 기준으로 자동 계산해서 배치한다.
/// 이 스크립트를 쓰면 MazeBuilder의 크기 값을 바꿔도 다시 손으로 좌표를
/// 계산할 필요가 없다.
/// </summary>
public class MazeAutoLayout : MonoBehaviour
{
    [Header("자동 배치 대상")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D exitBlocker;
    [SerializeField] private Collider2D exitSuccessTrigger;

    [Header("카메라 여유 공간 (1 = 딱 맞춤, 1.08 = 8% 여유)")]
    [SerializeField] private float cameraPadding = 1.08f;

    private void Start()
    {
        MazeBuilder maze = MazeBuilder.Instance;
        if (maze == null)
        {
            Debug.LogError("[MazeAutoLayout] MazeBuilder를 찾을 수 없습니다.");
            return;
        }

        PlacePlayer(maze);
        PlaceCamera(maze);
        PlaceExit(maze);
    }

    private void PlacePlayer(MazeBuilder maze)
    {
        if (player == null) return;
        Vector3 pos = maze.GetCellCenter(1, 1); // 미로 알고리즘의 시작 칸
        player.position = new Vector3(pos.x, pos.y, player.position.z);
    }

    private void PlaceCamera(MazeBuilder maze)
    {
        if (mainCamera == null) return;

        Vector2 total = maze.GetTotalWorldSize();
        mainCamera.transform.position = new Vector3(total.x / 2f, -total.y / 2f, mainCamera.transform.position.z);

        float size = Mathf.Max(total.y / 2f, (total.x / 2f) / mainCamera.aspect);
        mainCamera.orthographicSize = size * cameraPadding;
    }

    private void PlaceExit(MazeBuilder maze)
    {
        int exitCol = maze.Width - 1;
        int exitRow = maze.Height - 2;

        if (exitBlocker != null)
        {
            Vector3 pos = maze.GetCellCenter(exitCol, exitRow);
            exitBlocker.transform.position = new Vector3(pos.x, pos.y, exitBlocker.transform.position.z);
        }

        if (exitSuccessTrigger != null)
        {
            // 출구보다 한 칸 더 바깥쪽에 배치
            Vector3 pos = maze.GetCellCenter(exitCol + 1, exitRow);
            exitSuccessTrigger.transform.position = new Vector3(pos.x, pos.y, exitSuccessTrigger.transform.position.z);
        }
    }
}