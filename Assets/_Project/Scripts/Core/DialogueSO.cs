using UnityEngine;
using System.Collections.Generic;
using TMPro;

//대사 저장 데이터 생성 스크립트 오브젝트
[CreateAssetMenu(menuName = "Dialogue", fileName = "new Dialogue")]
public class DialogueSO : ScriptableObject
{
    [Header("Dialogue Settings")]
    [TextArea(3, 5)] // 인스펙터 창에서 줄바꿈이 가능하도록 설정
    [SerializeField] private List<string> dialogues = new List<string>(); 

    private bool hasDialogue = false; // 대사 출력 여부 

    public List<string> getDialogues() // 대사 반환
    {
        return dialogues;
    }

    public void setHasDialogue(bool state)  // 대사 출력 여부 상태 변경
    {
        hasDialogue = state;
    }
    public bool getHasDialogue() // 대사 출력 여부
    {
        return hasDialogue;
    }
    

}
