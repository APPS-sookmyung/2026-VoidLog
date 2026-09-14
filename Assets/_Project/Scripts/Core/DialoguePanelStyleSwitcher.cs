using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialoguePanelStyleSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class FrameStyle
    {
        public string speakerKey;
        public GameObject frameVisual;
    }

    [SerializeField] private TMP_Text speakerTextRef;
    [SerializeField] private List<FrameStyle> frameStyles = new List<FrameStyle>();

    private string lastSpeaker;

    private void Awake()
    {
        foreach (var f in frameStyles)
        {
            if (f.frameVisual != null)
            {
                f.frameVisual.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (speakerTextRef == null)
        {
            Debug.LogWarning("[DialoguePanelStyleSwitcher] Speaker Text Ref가 연결되지 않았습니다.");
            enabled = false; // 매 프레임 경고 도배 방지
            return;
        }

        string current = speakerTextRef.text;
        if (current == lastSpeaker) return;

        lastSpeaker = current;
        Debug.Log($"[DialoguePanelStyleSwitcher] Speaker 감지: '{current}'");
        ApplyStyle(current);
    }

    private void ApplyStyle(string speaker)
    {
        FrameStyle matched = frameStyles.Find(f => f.speakerKey == speaker);

        if (matched == null)
        {
            Debug.LogWarning($"[DialoguePanelStyleSwitcher] '{speaker}'와 일치하는 speakerKey가 Frame Styles 리스트에 없습니다.");
        }
        else
        {
            Debug.Log($"[DialoguePanelStyleSwitcher] '{speaker}' 매칭됨 -> {matched.frameVisual?.name} 켜짐");
        }

        foreach (var f in frameStyles)
        {
            if (f.frameVisual != null)
            {
                f.frameVisual.SetActive(f == matched);
            }
        }
    }
}