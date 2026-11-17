using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // [처음부터] 버튼용
    public void NewGame()
    {
        SaveManager.Instance.NewGame();
    }

    // [이어하기] 버튼용
    public void ContinueGame()
    {
        SaveManager.Instance.ContinueGame();
    }
}