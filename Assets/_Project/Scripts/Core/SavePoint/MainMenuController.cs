using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // 처음부터
    public void NewGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.NewGame();
        }
    }

    // 이어하기
    public void ContinueGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ContinueGame();
        }
    }
}