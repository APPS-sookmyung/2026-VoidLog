public static class GameProgressData
// 씬 이동에 유지되는 정보(오픈 여부, 퍼즐 클리어 여부 등) - 게임종료 혹은 다시시작 시 초기화됨
{
    // Puzzle 01 - Workshop
    public static bool hasOpenedMap = false; // 지도 오픈 여부
    public static bool hasSolvedCorridorPassword = false; 

    // Puzzle 02 - ControlRoomEntrance
    public static bool hasOpenedControlRoomDoor = false; // 중앙 통제실 도어락 오픈 여부

    // Puzzle 03 - Control
    public static bool hasEnteredRecordsRoom = false;
}