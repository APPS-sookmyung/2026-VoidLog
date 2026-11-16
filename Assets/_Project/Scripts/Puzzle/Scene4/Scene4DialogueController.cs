using System;
using System.Collections;
using UnityEngine;

public class Scene4DialogueController : MonoBehaviour
{
    [Header("씬 대사 CSV 파일 연결")]
    [SerializeField] private TextAsset sceneDialogueCSV;

    private IEnumerator Start()
    {
        yield return null; // 한 프레임 대기 후 시작

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[Scene4Dialogue] DialogueManager.Instance가 없습니다");
            yield break;
        }

        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
        DialogueManager.Instance.StartDialogueGroup("Enter");
    }

    // 스도쿠 클리어 후 SudokuManager가 호출.
    // startDelay초 기다렸다가 PuzzleClear 대사를 재생하고, 대사가 끝나면 onEnd 실행 (다음 씬 이동 등)
    public void PlayClearDialogue(Action onEnd, float startDelay = 0f)
    {
        StartCoroutine(PlayClearDialogueRoutine(onEnd, startDelay));
    }

    private IEnumerator PlayClearDialogueRoutine(Action onEnd, float startDelay)
    {
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("[Scene4Dialogue] DialogueManager.Instance가 없어 대사 없이 넘어갑니다");
            onEnd?.Invoke();
            yield break;
        }

        Debug.Log("[Scene4Dialogue] PuzzleClear 대사 시작");
        DialogueManager.Instance.StartDialogueGroup("PuzzleClear", onEnd);
    }
}