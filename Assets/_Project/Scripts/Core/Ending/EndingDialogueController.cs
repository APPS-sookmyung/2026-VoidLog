using UnityEngine;
using VoidLog.Core;
using VoidLog.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingDialogueController : MonoBehaviour
{
    [Header("엔딩 대사 CSV 연결")]
    [SerializeField] private TextAsset dialogueCSV;

    [Header("그룹 ID 설정")]
    [SerializeField] private string endingGroupId = "Replay";         
    [SerializeField] private string replayGroupId = "ReplayButton";  

    [Header("연출 오브젝트 연결")]
    [SerializeField] private GameObject replayButton; 
    [SerializeField] private FadeController fadeController; 

    private void Awake()
    {
        if (EndingDialogueManager.Instance != null && dialogueCSV != null)
        {
            EndingDialogueManager.Instance.LoadEndingDatabase(dialogueCSV);
        }
        else
        {
            Debug.LogError("[EndingDialogueController] EndingDialogueManager.Instance가 없거나 CSV 파일이 연결되지 않았습니다!");
        }

        if (replayButton != null)
            replayButton.SetActive(false);
    }

    // 랜덤 대사 출력
    public void PlayRandomEndingLine()
    {
        StartCoroutine(PlayEndingSequenceRoutine());
    }

    private IEnumerator PlayEndingSequenceRoutine()
    {
        bool isDialogueDone = false;

        if (EndingDialogueManager.Instance != null)
        {
            EndingDialogueManager.Instance.PlayRandomEndingLine(endingGroupId, () => {
                isDialogueDone = true;
            });
        }
        else
        {
            isDialogueDone = true;
        }

        while (!isDialogueDone)
        {
            yield return null;
        }

        // 대사가 끝난 후 페이드아웃 및 리플레이 버튼 활성화
        if (fadeController != null)
        {
            fadeController.FadeOutEvent(); 
            yield return new WaitForSeconds(1.5f); 
        }

        if (replayButton != null)
        {
            replayButton.SetActive(true);
        }
    }
}