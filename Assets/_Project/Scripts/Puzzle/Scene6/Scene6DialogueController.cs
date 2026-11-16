using System;
using System.Collections;
using UnityEngine;

public class Scene6DialogueController : MonoBehaviour
{
    [Header("씬 대사 CSV 파일 연결 (Scene6Dialogue)")]
    [SerializeField] private TextAsset sceneDialogueCSV;

    private IEnumerator Start()
    {
        yield return null; // 한 프레임 대기 후 시작 (DialogueManager 초기화 대기)

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[Scene6Dialogue] DialogueManager.Instance가 없습니다");
            yield break;
        }

        if (sceneDialogueCSV == null)
        {
            Debug.LogError("[Scene6Dialogue] sceneDialogueCSV가 연결되지 않았습니다");
            yield break;
        }

        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
        DialogueManager.Instance.StartDialogueGroup("Enter");
    }

    // 다른 스크립트(편지 상호작용, 미로 클리어 등)에서 원하는 그룹을 재생하고 싶을 때 사용.
    // 예) sceneController.PlayGroup("GroupID", () => { /* 대사 끝난 뒤 할 일 */ });
    // 현재 Scene6Dialogue.csv에는 "Enter" 그룹만 있으니, 다른 그룹을 쓰려면 CSV에 먼저 추가하세요.
    public void PlayGroup(string groupId, Action onEnd = null)
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[Scene6Dialogue] DialogueManager.Instance가 없어 대사 없이 넘어갑니다");
            onEnd?.Invoke();
            return;
        }

        DialogueManager.Instance.StartDialogueGroup(groupId, onEnd);
    }
}