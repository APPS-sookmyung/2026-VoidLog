using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;


// 퍼즐1 복도 지도키패드 기능 스크립트

public class Puzzle_CorridorPassword : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TextMeshProUGUI feedbackText; // 안내 및 오답 알림용 텍스트
    
    [Header("MapKeyPad")]
    [SerializeField] Canvas MapKeyPad; // 키패드 캔버스
    [SerializeField] Image Map; // 지도

    [Header("Settings")]
    [SerializeField] private string correctPassword = "35015";

    private bool hasBeenOpen = false; // 오픈 여부 (get O)

    void Start()
    {
        MapKeyPad.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);
        passwordInputField.onValueChanged.AddListener(OnInputChanged); // 입력창 글자 입력될 때마다 실행될 리스너 연결
    }

    public void OpenKeyPad()
    {
        Map.gameObject.SetActive(false); // 지도 화면 안뜨게
        passwordInputField.text = ""; //비밀번호 입력 초기화
        if (passwordInputField != null)
        {
            // 초기 세팅: 자리 제한 및 숫자만 입력 가능하도록 강제 설정
            passwordInputField.characterLimit = 5;
            passwordInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        }

        if (feedbackText != null)
        {
            feedbackText.text = "비밀번호를 입력하세요.";
            feedbackText.color = Color.white;
        }
    }
    // 입력 도중 호출
    private void OnInputChanged(string currentText)
    {
        // 입력이 다시 시작되면 글자 색상을 화이트로 리셋
        if (feedbackText != null)
        {
            feedbackText.color = Color.white;
            feedbackText.text = "입력 중...";
        }
    }

    // 확인키를 누르거나 입력 포커스가 빠졌을 때 판정
    private void OnSubmitPassword(string inputPassword)
    {
        // 5자리를 다 채우지 않았다면 리턴
        if (inputPassword.Length < 5)
        {
            ShowError("5자리 숫자를 모두 입력해야 합니다.");
            return;
        }

        // 비밀번호 검증
        if (inputPassword == correctPassword)
        {
            Map.gameObject.SetActive(true);
            hasBeenOpen = true;
        }
        else
        {
            ShowError("인증 실패: 보안 권한이 거부되었습니다.");
        }
    }

    // 오답 시 연출
    private void ShowError(string message)
    {
        // onValueChanged를 실행시키지 않고 입력창만 비우기
        passwordInputField.SetTextWithoutNotify("");
        if (feedbackText != null)
        {
            feedbackText.color = Color.red; // 빨간색으로 변경
            feedbackText.text = message;
        }
        
        // 재포커스
        passwordInputField.ActivateInputField();
    }


    // 취소 버튼
    public void DeletePasswordNumber() 
    {
        if (passwordInputField.text.Length > 0)
        {
            passwordInputField.text =
                passwordInputField.text.Substring(
                    0,
                    passwordInputField.text.Length - 1
                );
        }
    }

    //클릭 버튼
    public void SubmitPassword()
    {
       OnSubmitPassword(passwordInputField.text);
    }
    
    // 키패드 버튼 연결
    public void KeyPadClickButton(string number)
    {
        if (passwordInputField.text.Length < 5)
        {
            passwordInputField.text += number;
        }
    }
  
    public bool getHasBeenOpen()
    {
        return hasBeenOpen;
    }





}
