using UnityEngine;
using UnityEngine.Events;

// 캔버스 없이 [E]만 누르면 이벤트를 실행하는 상호작용 스크립트
public class TriggerInteractable : MonoBehaviour
{
    [Header("[E] 클릭 안내 문구")]
    [SerializeField] private GameObject clickText;

    [Header("E 눌렀을 때 실행할 이벤트")]
    [SerializeField] private UnityEvent onInteract;

    private bool isClick;

    void Start()
    {
        if (clickText != null) clickText.SetActive(false);
    }

    void Update()
    {
        if (!isClick || !Input.GetKeyDown(KeyCode.E)) return;

        // 대사 출력 중이면 상호작용 금지 (InteractableObject와 동일)
        if (DialogueManager.Instance != null && DialogueManager.Instance.getIsDialogueRunning())
            return;

        onInteract?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        isClick = true;
        if (clickText != null) clickText.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        isClick = false;
        if (clickText != null) clickText.SetActive(false);
    }

    void OnDisable()
    {
        isClick = false;
        if (clickText != null) clickText.SetActive(false);
    }
}