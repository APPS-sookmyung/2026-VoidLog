using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    // 버튼을 눌렀을 때
    public void OnClickClose()
    {
        // 자신(버튼)이 속해 있는 Canvas를 찾는다
        Canvas parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null)
        {
            // 찾은 Canvas 전체를 비활성화 (화면에서 사라짐)
            parentCanvas.gameObject.SetActive(false);
        }
        else
        {
            // 혹시 Canvas를 못 찾았을 때 콘솔에 경고 표시 (디버깅용)
            Debug.LogWarning("부모 Canvas를 찾을 수 없습니다.");
        }
    }
}