using UnityEngine;
using UnityEngine.SceneManagement;

// 퍼즐1 씬 전환 벽
public class Puzzle01SceneGate : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    [Header("씬 전환 불가능할 때 출력될 대사")]
    [SerializeField] private Scene1DialogueController scene1DialogueController;

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (GameProgressData.hasOpenedMap)
        {
            LoadNextScene();
        }
        else
        {
            playerMovement.setCanMove(false);

            scene1DialogueController.MapRequiredDialogue(() =>
            {
                playerMovement.setCanMove(true);
            });
        }
    }

    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("이동할 씬 이름이 설정되지 않았습니다.");
        }
    }
}