using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// "이어하기" 버튼에 붙여서, 세이브 파일이 없으면 버튼을 비활성화한다.
/// 기존 MainMenuController는 전혀 건드리지 않는 독립 스크립트.
///
/// 세팅:
/// - 이어하기 버튼 오브젝트(Button 컴포넌트가 있는 그 오브젝트)에 이 스크립트를 붙인다.
/// - 버튼 On Click은 기존대로 MainMenuController.ContinueGame()을 그대로 둔다.
/// </summary>
[RequireComponent(typeof(Button))]
public class ContinueButtonGuard : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // 씬 시작 시, 그리고 이 오브젝트가 다시 켜질 때마다 세이브 상태를 확인한다.
    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (button == null) button = GetComponent<Button>();

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[ContinueButtonGuard] SaveManager.Instance가 없어 이어하기 버튼을 비활성화합니다. " +
                             "Resources 폴더에 SaveManager 프리팹이 있는지 확인하세요.");
            button.interactable = false;
            return;
        }

        button.interactable = SaveManager.Instance.HasSaveData();
    }
}
