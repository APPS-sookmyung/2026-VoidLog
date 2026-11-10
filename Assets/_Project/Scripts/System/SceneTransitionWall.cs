using UnityEngine;
using UnityEngine.SceneManagement;

// 앞에서 뒤로 이동할 때 사용할 씬 전환 (단순 벽에 부딪치면 이동)
public class SceneTransitionWall : MonoBehaviour
{
    [Header("이동할 씬")]
    [SerializeField] private string nextSceneName;

    [Header("다음 씬의 시작 위치 이름")]
    [SerializeField] private string spawnPointName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneTransitionData.spawnPointName = spawnPointName;
        SceneManager.LoadScene(nextSceneName);
    }
}