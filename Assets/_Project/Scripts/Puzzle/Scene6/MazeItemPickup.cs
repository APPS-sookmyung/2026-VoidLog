using UnityEngine;

/// <summary>
/// 2D 버전 부품 획득. 콜라이더는 Collider2D 계열(Circle Collider 2D 등)에 Is Trigger 체크.
/// </summary>
public class MazeItemPickup : MonoBehaviour
{
    [SerializeField] private string itemId = "Part_01";
    [SerializeField] private GameObject interactPrompt;

    private bool isPlayerNear = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            MazeItemManager.Instance.CollectItem(itemId);
            if (interactPrompt != null) interactPrompt.SetActive(false);
            Destroy(gameObject);
        }
    }
}

/*
 * 플레이어 오브젝트 준비물 (2D)
 * - Tag를 "Player"로 설정
 * - Rigidbody 2D 필수 (Body Type은 Dynamic 또는 Kinematic 아무거나 - 트리거 감지엔 필요)
 * - Collider2D 하나 (Box Collider 2D 등)
 */
