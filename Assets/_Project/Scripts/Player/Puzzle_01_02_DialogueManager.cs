using UnityEngine;
using System.Collections.Generic;
using TMPro;


//대사 출력 매니저 -> 대사 Data를 가져와 출력 및 버튼 클릭 스크립트
public class Puzzle_01_02_DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText; // 대사가 출력될 TMP
    [SerializeField] private GameObject dialoguePanel;     // 대사가 끝나면 꺼질 패널

    private int currentIndex = 0; // 현재 몇 번째 대사인지 기억할 변수
    private DialogueSO currentDialogue; // 현재 대사 기억할 변수
    private PlayerMovement player; // 플레이어 움직임 제어 변수
 

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    void Start()
    {
         dialoguePanel.SetActive(false); // 시작 시에는 대사 출력 X
    }

    // 대화 출력 함수 -> InteractableObject 호출
    public void DialogueStart(DialogueSO dialogue) 
    {
        currentDialogue = dialogue; // 현재 대사 가져오기
        currentIndex = 0; // 대사 줄 초기화
        
        // 대사출력 
       if (currentDialogue.getDialogues().Count > 0)
       {
            dialoguePanel.SetActive(true);
            player.setCanMove(false);

            ShowCurrentDialogue();
        }
        
      
    }

    // 투명 버튼에 연결할 '클릭 시 다음 대사' 함수
    public void OnClickNextDialogue()
    {
        currentIndex++; // 다음 대사 인덱스로 증가

        // 아직 출력할 대사가 남아있다면
        if (currentIndex < currentDialogue.getDialogues().Count)
        {
            ShowCurrentDialogue();
        }
         else // 전부 출력했다면
        {
            currentDialogue.setHasDialogue(true);
            player.setCanMove(true);
            dialoguePanel.SetActive(false);
    }
       
    }

    // 현재 인덱스의 대사를 화면에 출력
    private void ShowCurrentDialogue()
    {
        dialogueText.text = currentDialogue.getDialogues()[currentIndex];
    }
    public int getCurrentIndex()
    {
        return currentIndex;
    } // 현재 몇 번째 대사인지 기억할 변수 반환
  
    
}
