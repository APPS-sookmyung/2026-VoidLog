using System;
using TMPro;
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
    [SerializeField] private UnityEvent onInteract; // 발생할 캔버스 속 시작 함수
    [SerializeField] private UnityEvent onClose; // 발생할 캔버스 속 끝낼 때 함수

    private bool isClick; // 콜라이더 안에 있는지 확안용
    private bool hasInteracted = false; // 이벤트 열람 여부 확인용
    private PlayerMovement player;

    [Header("끝난 후 나올 대사 설정")]
    [SerializeField] private Puzzle_01_02_DialogueManager dialogueManager;
    [SerializeField] private DialogueSO dialogueData;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }
    void Start()
    {
        if (clickText != null) clickText.SetActive(false); 
        if (canvas != null) canvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isClick && canvas != null && !canvas.gameObject.activeSelf) 
        // E 클릭시 캔버스 활성화 및 움직임 제어
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
                player.setCanMove(false);
                onInteract?.Invoke(); // 연동된 외부 이벤트 실행
                hasInteracted = true;
                
            }
        }
        if( canvas != null &&!canvas.gameObject.activeSelf && hasInteracted) // 캔버스 꺼지면 대사 출력 및 움직임 가능
        {
            if (dialogueManager != null && dialogueData != null)
            {
                dialogueManager.DialogueStart(dialogueData);
            }
            else
            {
                player.setCanMove(true);
            }
            
            hasInteracted = false;
        }
    }

    public void CloseInteract() // 캔버스 닫기 위한 함수 (버튼에 연결)
    {
        if (canvas.gameObject.activeSelf) canvas.gameObject.SetActive(false);
        onClose?.Invoke();
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