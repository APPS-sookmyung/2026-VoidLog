using UnityEngine;

/// <summary>
/// 이 스크립트는 "SuccessTrigger" 오브젝트 자체에 붙여야 함!
/// (부모인 MazeExitGate가 아니라, 트리거 콜라이더가 실제로 있는 오브젝트에 붙어야
/// OnTriggerEnter2D가 정상적으로 호출됨)
///
/// physicalBlocker는 형제 오브젝트인 Blocker를 드래그해서 연결.
/// </summary>
public class MazeExitGate : MonoBehaviour
{
    [SerializeField] private Collider2D physicalBlocker;

    private void Update()
    {
        if (physicalBlocker != null && physicalBlocker.enabled && MazeItemManager.Instance.HasAllItems())
        {
            physicalBlocker.enabled = false;
            Debug.Log("[MazeExitGate] 출구가 열렸습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[MazeExitGate] 트리거 진입 감지: {other.name}, tag={other.tag}");

        if (!other.CompareTag("Player")) return;

        if (MazeItemManager.Instance.HasAllItems())
        {
            Debug.Log("[MazeExitGate] 성공 조건 충족 - OnSuccess 호출");
            MazeGameManager.Instance.OnSuccess();
        }
        else
        {
            Debug.Log("[MazeExitGate] 아직 부품을 다 못 모음");
        }
    }
}