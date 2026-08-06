using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Puzzle5_DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dialogueText; // 대사가 출력될 TMP
    [SerializeField] private GameObject dialoguePanel;     // 대사가 끝나면 꺼질 패널

    [Header("Puzzle Connection")]
    [SerializeField] private GameObject puzzleObject; // 대화 후 활성화할 퍼즐 오브젝트


    [Header("Dialogue Settings")]
    [TextArea(3, 5)] // 인스펙터 창에서 줄바꿈이 가능하도록 설정
    [SerializeField] private List<string> dialogues = new List<string>(); 

    private int currentIndex = 0; // 현재 몇 번째 대사인지 기억할 변수

    private void Start()
    {
        // 퍼즐은 대화가 끝나기 전까지 비활성화
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(false);
        }

        // 게임 시작 시 대사창 초기화 및 첫 대사 출력
        if (dialogues.Count > 0)
        {
            dialoguePanel.SetActive(true);
            ShowCurrentDialogue();
        }
        else
        {
            // 대사 데이터가 없으면 창을 끕니다.
            // 그리고 바로 퍼즐을 활성화합니다.
            dialoguePanel.SetActive(false);
            if (puzzleObject != null)
            {
                puzzleObject.SetActive(true);
            }
        }
    }

    // 투명 버튼에 연결할 '클릭 시 다음 대사' 함수
    public void OnClickNextDialogue()
    {
        currentIndex++; // 다음 대사 인덱스로 증가

        // 아직 출력할 대사가 남아있다면
        if (currentIndex < dialogues.Count)
        {
            ShowCurrentDialogue();
        }
        else
        {
            // 모든 대사가 끝났을 때의 처리
            EndDialogue();
        }
    }

    // 현재 인덱스의 대사를 화면에 출력
    private void ShowCurrentDialogue()
    {
        dialogueText.text = dialogues[currentIndex];
    }

    // 대사가 종료되었을 때 실행될 함수
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        // 대화가 끝나면 퍼즐 오브젝트를 활성화
        if (puzzleObject != null)
        {
            puzzleObject.SetActive(true);
        }
    }
}