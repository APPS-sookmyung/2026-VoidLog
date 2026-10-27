using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    // 이어하기 위치
    public string sceneName;
    public string spawnPointName;

    // 게임 공통 데이터

    // Puzzle 1
    public bool hasOpenedMap = false; //지도 오픈 여부
   
    // Puzzle 2
    public bool hasOpenedControlRoomDoor = false;

    // Scene 3 ~ 6 클리어 여부
    public bool isScene3Cleared = false; // 중앙 통제실 레버 퍼즐
    public bool isScene4Cleared = false; // 스도쿠
    public bool isScene5Cleared = false; // 기록실 비밀번호
    public bool isScene6Cleared = false; // 격납고 미로

    // 필요한 데이터 계속 추가
}
