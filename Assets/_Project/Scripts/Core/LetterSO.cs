using JetBrains.Annotations;
using UnityEngine;
/// <summary>
/// 편지 데이터 생성 스크립트
/// </summary>

[CreateAssetMenu(menuName = "Letter", fileName = "new Letter")]
public class LetterSO : ScriptableObject
{
    [SerializeField] string letterId = "LETTER_01"; //편지 고유 ID
    [SerializeField] string senderName ="A"; // 발신자 이름
    [SerializeField] string letterTitle = "편지제목"; // 편지 제목
    [TextArea(2,10)]
    [SerializeField] string letterContent = "편지 본문"; // 편지 본문


}
