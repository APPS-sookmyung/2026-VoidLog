using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoniterFile : MonoBehaviour
{
    [Header("파일 Image")]
    [SerializeField] private Image trueFile; // 금기사항 파일 
    [SerializeField] private List<Image> falseFiles= new List<Image>(); // 손상된 파일 목록
    
    [Header("파일 내용")]
    [SerializeField] private Image fileContent; // 파일 열린 내용
    [SerializeField] private TextMeshProUGUI fileContentText; // 파일 내용 속 텍스트

    [Header("파일 내용 문구")]
    [SerializeField] // 금기사항 목록 내용
    private string tabooFileText =
        "금기사항 목록(1) \n" +
        "1. 하루 활동은 3회로 제한.\n" +
        "2. 전력이 50 아래로 떨어지지 않도록 주의할 것.\n" +
        "3. 일주일에 15건 이상의 편지를 검열할 것.\n" +
        "4. 외부인과의 접근은 엄격히 금함.\n\n" +
        "※ ▒▒▒ #%)))))@# [로그 데이터가 유실되었습니다] ▒▒▒";

    [SerializeField] private string corruptedFileText = "해당 파일은 손상되었습니다."; // 손상파일 내용

    [Header("모니터 Canvas")]
    [SerializeField] private Canvas moniter; // 전체 모니터 화면
    PlayerMovement playerMovement;

   
    private bool isShowFileContent; // 파일 내용 출력 여부
    private bool isShowMoniter = true; // 모니터 출력 여부


    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    void Start()
    {
        isShowFileContent = false; //처음에는 파일 내용 출력X
    }
    void Update()
    {
        if (isShowMoniter) // 모니터 화면 보일 때
        {
            // 파일 클릭 -> 파일 내용 보임 , 파일 밖에 클릭 -> 파일 내용 꺼짐
            fileContent.gameObject.SetActive(isShowFileContent);
            playerMovement.setCanMove(false); // 못 움직임
        }
        else
        {            
            moniter.gameObject.SetActive(isShowMoniter); // 모니터 화면 비활성화
            playerMovement.setCanMove(true); // 움직이기 가능
            isShowMoniter = true; // 다시 모니터 열람시 오류 방지

        }
        
    }

    
    public void trueFileClick() // 금기사항 파일 클릭시
    {
        isShowFileContent = true;
        fileContentText.text = tabooFileText;
    }

       public void falseFileClick()  // 손상파일 클릭시

    {
        isShowFileContent = true;
        fileContentText.text = corruptedFileText;
    }

    public void mainClick() // 파일 보이는 상태에서 메인 모니터 클릭시 파일 내용 꺼지기
    {
        if (isShowFileContent)
        {
            isShowFileContent = false;
        }
    }
    public void closeClick() // X버튼 클릭시 창 내리기
    {
        if (!isShowFileContent)
        {
            isShowMoniter = false;
        }
    }

    


    


}
