using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public static SaveManager Instance { get; private set; }

    // 이어하기 위치
    public string sceneName;
    public string spawnPointName;

    // 게임 공통 데이터

    // Puzzle 1
    public bool hasOpenedMap = false; //지도 오픈 여부
   
    // Puzzle 2
    public bool hasOpenedControlRoomDoor = false;

    // 필요한 데이터 계속 추가
}
