using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 반드시 필요!
using System.Collections; 
//퍼즐 1 씬 전환 벽
public class Puzzle01SceneGate : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [Header("Scene")]
    [Tooltip("Build Settings에 등록된 이동하고자 하는 씬의 정확한 이름")]
    [SerializeField] private string nextSceneName;
    
    [Header("씬 전환 불가능할 때 출력될 대사")]
    [SerializeField] private Puzzle_01_02_DialogueManager dialogueManager;
    [SerializeField] private DialogueSO dialogueSO;

    void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!collision.CompareTag("Player"))
            return;

        playerMovement.setCanMove(false);

        if (GameProgressData.hasOpenedMap)
        {
            LoadNextScene();
        }
        else
        {
            StartCoroutine(ShowBlockedDialogue());
        }
}
    private IEnumerator ShowBlockedDialogue() // 대사 출력
    {
        playerMovement.setCanMove(false);

        dialogueSO.setHasDialogue(false);
        dialogueManager.DialogueStart(dialogueSO);

        yield return new WaitUntil(() => dialogueSO.getHasDialogue());

        playerMovement.setCanMove(true);
    }
    // 씬 전환 실행 함수
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[SceneChangeInteractable] 이동할 씬 이름(nextSceneName)이 설정되지 않았습니다!");
        }
    }
    
}
