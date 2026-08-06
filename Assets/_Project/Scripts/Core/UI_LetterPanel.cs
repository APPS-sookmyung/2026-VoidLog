using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//편지 캔버스에 넣는 스크립트
// 플레이어가 접근해서 E 클릭시 편지 오브젝트 화면에 띄움
public class UI_LetterPanel : MonoBehaviour
{
    InteractableObject isClick; // E 클릭 여부 확인
    [SerializeField]LetterSO letter; // 편지 데이터 가져오기

    [SerializeField] Canvas letterCanvas; // 편지 캔버스
    [SerializeField] TextMeshProUGUI content; // 편지 캔버스 속 텍스트 


    void Awake()
    {
        isClick = FindObjectOfType<InteractableObject>();
    }

    void Start()
    {
        // 편지 안보이게 설정
        letterCanvas.gameObject.SetActive(false);
    }
    void Update()
    {
        // E클릭 가능한 범위면서 E클릭시 캔버스 표시
        if (isClick.getIsClick())
        {
            if (Input.GetKey(KeyCode.E))
            {
                DisplayLetter();
            letterCanvas.gameObject.SetActive(true);  
            }
        }
        else // 범위 나가면 캔버스 가리기
        {
            letterCanvas.gameObject.SetActive(false);
        }
    }
    void DisplayLetter() 
    {
        // 편지내용 가져오기
        content.text=letter.getContent();
    }

}
