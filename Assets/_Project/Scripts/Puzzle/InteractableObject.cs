using System;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 플레이어와 상호작용하는 오브젝트(편지, 키패드, 모니터화면 등등)에 넣는 스크립트
//콜라이더 구역 안에 플레이어 접근 시 [E] 클릭 텍스트 표시

public class InteractableObject : MonoBehaviour
{
    [Header("[E] 클릭 안내 문구")]
    [Tooltip("화면 하단에 띄울 문구 (ex. [E] 클릭)")]
    [SerializeField] GameObject clickText; // 표시할 텍스트

    [Tooltip("[E] 클릭 후 띄우는 Cavas")]
    [SerializeField] Canvas canvas; // E 클릭 시 띄울 캔버스

    private bool isClick; // 클릭 가능 여부
    private bool hasBeenClick = false;


    [Header("끝난 후 나올 대사 설정")]
    [SerializeField] Puzzle_01_02_Dialogue DialogueManager; // 대사 출력 매니저
    [SerializeField] private DialogueSO dialogueData; // 대사 데이터

    void Start() // 처음에 캔버스 가림
    {
        clickText.gameObject.SetActive(false); 
        canvas.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // E클릭 가능한 범위면서 E클릭시 캔버스 표시
        if (isClick)
        {
            if (Input.GetKey(KeyCode.E))
            {
                canvas.gameObject.SetActive(true); 
                hasBeenClick = true;
            }
        }
        else // 범위 나가면 캔버스 가리기
        {
            canvas.gameObject.SetActive(false);
        }
    }
 
    void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어 접근 시 글자 표시 & 클릭 가능
        if(collision.gameObject.tag == "Player")
        {
            clickText.gameObject.SetActive(true);
            isClick = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        //플레이어 멀어질 시 글자 가림 & E 클릭 불가능
        if(collision.gameObject.tag == "Player")
        {
            clickText.gameObject.SetActive(false);
            isClick = false;
            if (hasBeenClick) // 클릭 했을 시 대사 출력
            {
                DialogueManager.DialogueStart(dialogueData);
                hasBeenClick = false;    
            }
            
        }
        
        
    }


}