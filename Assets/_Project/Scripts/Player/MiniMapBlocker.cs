using UnityEngine;

// 이 오브젝트가 켜져 있는 동안 플레이어 미니맵/월드맵을 숨긴다.
// 꺼지거나(SetActive(false)) 씬이 바뀌어 파괴되면 미니맵이 다시 켜진다.
// 예: Scene6 미로 루트(MazeRoot) - HangerFlowManager.StartMaze()에서 자동으로 붙음
public class MiniMapBlocker : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerMiniMap.AddBlocker();
    }

    private void OnDisable()
    {
        PlayerMiniMap.RemoveBlocker();
    }
}
