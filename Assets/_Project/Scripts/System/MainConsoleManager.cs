using UnityEngine;

public class MainConsoleManager : MonoBehaviour
{
    [Header("화면 UI 연결")]
    [SerializeField] private GameObject screenFalse; // 클리어 전 화면
    [SerializeField] private GameObject screenTrue;  // 클리어 후 화면

    [Header("상태")]
    public bool isPuzzleCleared = false;

    // InteractableObject의 onInteract()에 연결할 함수
    public void RefreshConsoleScreen()
    {
        if (isPuzzleCleared)
        {
            if (screenFalse != null) screenFalse.SetActive(false);
            if (screenTrue != null) screenTrue.SetActive(true);
        }
        else
        {
            if (screenFalse != null) screenFalse.SetActive(true);
            if (screenTrue != null) screenTrue.SetActive(false);
        }
    }

    // 퍼즐 클리어 시 호출해줄 메서드
    public void SetPuzzleClear()
    {
        isPuzzleCleared = true;
        RefreshConsoleScreen();
    }
}