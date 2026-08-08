using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//편지 캔버스에 넣는 스크립트
// 플레이어가 접근해서 E 클릭시 편지 오브젝트 화면에 띄움
public class UI_LetterPanel : MonoBehaviour
{
    [SerializeField] private LetterSO letter; // 편지 데이터 가져오기
    [SerializeField] private TextMeshProUGUI content; // 편지 캔버스 속 텍스트 


    private void Start() {
        Display();
    }
    void Display() 
    {
        // 편지내용 가져오기
        content.text=letter.getContent();
    }

}
