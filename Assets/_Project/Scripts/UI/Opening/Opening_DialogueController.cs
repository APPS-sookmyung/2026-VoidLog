using UnityEngine;

/// <summary>
/// Scene5DialogueController와 같은 역할, 오프닝용.
///
/// 중요: CSV 로드를 Awake()에서 즉시 처리한다 (Start()에서 프레임 지연 X).
/// Unity는 씬의 모든 오브젝트의 Awake()가 전부 끝난 뒤에야 Start()들을 실행하기
/// 시작하므로, OpeningStepManager의 Start()에서 Step0이 곧바로 PlayGroup()을
/// 호출하더라도 이 시점엔 이미 CSV가 로드되어 있는 게 보장된다.
///
/// 세팅 방법:
/// 1. GameManager 하위 DialogueController 오브젝트에 이 스크립트를 붙인다.
/// 2. Opening Dialogue CSV 필드에 오프닝용 CSV 파일을 연결한다.
/// 3. (권장) Edit > Project Settings > Script Execution Order에서
///    DialogueManager를 Default보다 앞쪽에 두면, DialogueManager.Instance가
///    확실히 먼저 세팅된 뒤 이 스크립트의 Awake가 실행되어 더 안전하다.
/// 4. OpeningStepManager의 각 Step onStepStart에서:
///    DialogueController(오브젝트) 드래그 -> OpeningDialogueController.PlayGroup(string) 선택
///    -> 텍스트 칸에 그 Step에서 보여줄 GroupID 입력.
/// </summary>
public class OpeningDialogueController : MonoBehaviour
{
    [Header("오프닝 대사 CSV 연결")]
    [SerializeField] private TextAsset openingDialogueCSV;

    private void Awake()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[OpeningDialogueController] DialogueManager.Instance가 없습니다. " +
                            "DialogueManager가 붙은 오브젝트가 씬에 있는지, " +
                            "Script Execution Order상 이 스크립트보다 먼저 Awake되는지 확인하세요.");
            return;
        }

        DialogueManager.Instance.LoadDialogueDatabase(openingDialogueCSV);
    }

    /// <summary>
    /// OpeningStepManager의 onStepStart에서 파라미터(GroupID)와 함께 바로 연결 가능.
    /// </summary>
    public void PlayGroup(string groupId)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("[OpeningDialogueController] DialogueManager.Instance가 없습니다.");
            return;
        }
        DialogueManager.Instance.StartDialogueGroup(groupId);
    }
}