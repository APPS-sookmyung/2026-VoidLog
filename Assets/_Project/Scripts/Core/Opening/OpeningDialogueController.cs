using UnityEngine;
using VoidLog.Core;

// 오프닝 전용 대사 컨트롤러.

public class OpeningDialogueController : MonoBehaviour
{
    [Header("오프닝 대사 CSV 연결")]
    [SerializeField] private TextAsset openingDialogueCSV;

    [Header("대사가 끝나면 자동으로 다음 Step으로 넘어가기 위한 연결")]
    [SerializeField] private OpeningStepManager stepManager;

    private void Awake()
    {
        if (OpeningDialogueManager.Instance == null)
        {
            Debug.LogError("[OpeningDialogueController] OpeningDialogueManager.Instance가 없습니다.");
            return;
        }

        OpeningDialogueManager.Instance.LoadDialogueDatabase(openingDialogueCSV);
    }

    public void PlayGroup(string groupId)
    {
        if (OpeningDialogueManager.Instance == null)
        {
            Debug.LogWarning("[OpeningDialogueController] OpeningDialogueManager.Instance가 없습니다.");
            return;
        }

        OpeningDialogueManager.Instance.StartDialogueGroup(groupId, OnGroupFinished);
    }

    private void OnGroupFinished()
    {
        if (stepManager != null)
        {
            stepManager.SkipToNextStep();
        }
        else
        {
            Debug.LogWarning("[OpeningDialogueController] Step Manager가 연결되지 않아 자동 진행을 못 합니다. Duration 타이머로만 넘어갑니다.");
        }
    }
}
