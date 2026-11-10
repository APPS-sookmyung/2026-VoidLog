using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//퍼즐 1 모니터 실행 스크립트
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
    
    private bool isShowFileContent; // 파일 내용 출력 여부



    void Start()
    {
        isShowFileContent = false; //처음에는 파일 내용 출력X
    }

    public void OpenMonitor()
    {
        // 파일 클릭 -> 파일 내용 보임 , 파일 밖에 클릭 -> 파일 내용 꺼짐
        fileContent.gameObject.SetActive(isShowFileContent);   
    }
    public void CloseMonitor() // X버튼 클릭시 창 내리기
    {
    }
    
    public void trueFileClick() // 금기사항 파일 클릭시
    {
        isShowFileContent = true;
        fileContentText.text = tabooFileText;
        OpenMonitor();
    }

       public void falseFileClick()  // 손상파일 클릭시

    {
        isShowFileContent = true;
        fileContentText.text = corruptedFileText;
        OpenMonitor();
    }

    public void mainClick() // 파일 보이는 상태에서 메인 모니터 클릭시 파일 내용 꺼지기
    {
        if (isShowFileContent)
        {
            isShowFileContent = false;
        }
        OpenMonitor();
    }
   

    


    


}
