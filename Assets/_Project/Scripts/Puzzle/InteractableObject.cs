using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

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
    private bool isHandlingClose = false; 
    private PlayerMiniMap playerMiniMap;

    [Header("끝난 후 나올 대사 설정")]
    [SerializeField] private Puzzle_01_02_DialogueManager dialogueManager;
    [SerializeField] private DialogueSO dialogueData;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        playerMiniMap = FindObjectOfType<PlayerMiniMap>();
    }
    void Start()
    {
        if (clickText != null) clickText.SetActive(false); 
        if (canvas != null) canvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // E 키 입력 처리
        if (Input.GetKeyDown(KeyCode.E) && isClick)
        {
            if (canvas != null)
            {
                // 캔버스가 비활성화 상태이면 활성화
                if (!canvas.gameObject.activeSelf)
                {
                    playerMiniMap.HideMiniMap();
                    canvas.gameObject.SetActive(true);
                    player.setCanMove(false);
                    onInteract?.Invoke(); // 연동된 외부 이벤트 실행
                    hasInteracted = true;
                }
                // 캔버스가 활성화 상태이면 비활성화 (닫기)
                else
                {
                    CloseInteract();
                }
            }
        }
        if (canvas != null && !canvas.gameObject.activeSelf && hasInteracted && !isHandlingClose)
        {
            StartCoroutine(HandleInteractionClosed());
        }
    }
    private IEnumerator HandleInteractionClosed() // 캔버스가 꺼진 '직후'에 한 번 실행되는 로직
    {
            isHandlingClose = true;
            if (dialogueManager != null && dialogueData != null)
            {
                player.setCanMove(false);
                dialogueData.setHasDialogue(false);
                dialogueManager.DialogueStart(dialogueData);
                // 대사가 끝날 때까지 기다리기
                yield return new WaitUntil(() => dialogueData.getHasDialogue());

            }
            player.setCanMove(true);
            // 한 번 실행 후 다시 실행되지 않도록 플래그를 false로 변경
            hasInteracted = false;
            isHandlingClose = false;
    }

    public void CloseInteract() // 캔버스 닫기 위한 함수 (버튼에 연결)
    {
        if (canvas.gameObject.activeSelf) canvas.gameObject.SetActive(false);
        playerMiniMap.ShowMiniMap();
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