using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// 플레이어와 상호작용하는 오브젝트(편지, 키패드, 모니터화면 등등)에 넣는 스크립트
//콜라이더 구역 안에 플레이어 접근 시 [E] 클릭 텍스트 표시
public class InteractableObject : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI clickText; // 표시할 텍스트

    private bool isClick; // 클릭 가능 여부

    void Start()
    {
        // 시작할 때는 글자 가림
        clickText.gameObject.SetActive(false); 
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
        //플레이어 멀어질 시 글자 가림 & 클릭 불가능
        if(collision.gameObject.tag == "Player")
        {
            clickText.gameObject.SetActive(false);
            isClick = false;
        }
    }

    public bool getIsClick() // 클릭 가능 여부 반환
    {
        return isClick;
    }
}