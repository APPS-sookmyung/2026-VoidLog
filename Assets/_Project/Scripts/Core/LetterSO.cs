using JetBrains.Annotations;
using UnityEngine;
/// <summary>
/// 편지 데이터 생성 스크립트
/// </summary>

[CreateAssetMenu(menuName = "Letter", fileName = "new Letter")]
public class LetterSO : ScriptableObject
{
    [SerializeField] private string letterId = "LETTER_01"; //편지 고유 ID
    [SerializeField] private string senderName ="A"; // 발신자 이름
    [SerializeField] private string letterTitle = "편지제목"; // 편지 제목
    [TextArea(2,10)]
    [SerializeField] private string letterContent = "편지 본문"; // 편지 본문

    private bool letterOpen = false; // 편지 오픈 여부

    public string getContent()
    {
        return letterContent;
    }
    public bool getLetterOpen() // 편지 오픈 여부 반환
    {
        return letterOpen;
    }
    public void setLetterOpen(bool state) // 편지 오픈 여부 변경
    {
        letterOpen = state;
    }



}
