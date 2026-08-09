using UnityEngine;

/// <summary>
/// 2D 버전 출구. physicalBlocker는 Is Trigger 해제된 Collider2D(실제로 막는 벽),
/// successTrigger는 Is Trigger 체크된 Collider2D(성공 판정용).
/// </summary>
public class MazeExitGate : MonoBehaviour
{
    [SerializeField] private Collider2D physicalBlocker;
    [SerializeField] private Collider2D successTrigger;

    private void Update()
    {
        if (physicalBlocker != null && physicalBlocker.enabled && MazeItemManager.Instance.HasAllItems())
        {
            physicalBlocker.enabled = false;
            Debug.Log("[ExitGate2D] 출구가 열렸습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (MazeItemManager.Instance.HasAllItems())
        {
            MazeGameManager.Instance.OnSuccess();
        }
    }
}
