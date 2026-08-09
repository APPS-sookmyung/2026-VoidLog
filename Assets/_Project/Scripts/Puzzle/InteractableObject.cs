using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 프로젝트 전체에서 공용으로 사용되는 범용 상호작용 스크립트
public class InteractableObject : MonoBehaviour
{
    [Header("[E] 클릭 안내 문구")]
    [Tooltip("화면 하단에 띄울 문구 (ex. [E] 클릭)")]
    [SerializeField] private GameObject clickText;

    [Header("클릭 후 뜰 이벤트")]
    [Tooltip("[E] 클릭 후 발생할 이벤트, 끝나고 발생할 이벤트")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onClose;

    private bool isClick;

    [Header("끝난 후 나올 대사 설정")]
    [SerializeField] private Puzzle_01_02_Dialogue DialogueManager;
    [SerializeField] private DialogueSO dialogueData;

    void Start()
    {
        if (clickText != null) clickText.SetActive(false); 
        if (canvas != null) canvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isClick)
        {
            if (canvas != null)
            {
                bool isCanvasActive = canvas.gameObject.activeSelf;
                canvas.gameObject.SetActive(!isCanvasActive);

                // 캔버스가 켜질 때만 연동된 외부 이벤트 실행
                if (!isCanvasActive)
                {
                    onInteract?.Invoke();
                }
            }
        }
    }

    public void CloseInteract()
    {
        if (canvas != null) canvas.gameObject.SetActive(false);
        onClose?.Invoke();

        if (DialogueManager != null && dialogueData != null)
        {
            DialogueManager.DialogueStart(dialogueData);
        }
    }
 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (clickText != null) clickText.SetActive(true);
            isClick = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (clickText != null) clickText.SetActive(false);
            isClick = false;
        }
    }

    public bool getIsClick() { return isClick; }
}