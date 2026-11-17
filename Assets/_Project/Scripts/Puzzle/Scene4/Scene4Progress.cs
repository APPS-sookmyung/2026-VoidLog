using UnityEngine;

// Scene_04의 진행 상황을 "씬을 나갔다 돌아와도" 기억해 두는 저장소입니다.
// 오브젝트에 붙이는 스크립트가 아니라, 어디서든 Scene4Progress.변수 로 읽고 쓰는 정적(static) 클래스입니다.
// 주의: 게임을 완전히 종료하면 사라집니다. (씬 이동 사이에서만 유지)
public static class Scene4Progress
{
    // ---- 편지 / 도어락 ----
    public static bool letterRead;            // 편지를 읽고 닫았는지 (= 도어락이 생겼는지)

    // ---- 대사 ----
    public static bool enterDialoguePlayed;   // 씬 시작 대사를 이미 재생했는지
    public static bool letterDialoguePlayed;  // 편지 읽은 뒤 대사를 이미 재생했는지

    // ---- 스도쿠 ----
    public static int[,] board = new int[9, 9]; // 칸마다 플레이어가 입력한 숫자 (0 = 빈칸)
    public static int wrongCount;               // 지금까지 오답 횟수
    public static bool sudokuCleared;           // 스도쿠를 클리어했는지
    public static bool clearHandled;            // 클리어 후 창 닫기 + 대사 + 벽 열기까지 끝났는지

    // 게임오버 시: 스도쿠 입력값과 오답 횟수만 초기화
    public static void ResetSudoku()
    {
        board = new int[9, 9];
        wrongCount = 0;
    }

    // 새 게임 시작 시 호출하면 전부 초기화
    public static void ResetAll()
    {
        letterRead = false;
        enterDialoguePlayed = false;
        letterDialoguePlayed = false;
        sudokuCleared = false;
        clearHandled = false;
        ResetSudoku();
    }

    // 에디터에서 Play를 다시 눌렀을 때 이전 플레이의 기록이 남지 않도록 자동 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void InitOnPlay()
    {
        ResetAll();
    }
}