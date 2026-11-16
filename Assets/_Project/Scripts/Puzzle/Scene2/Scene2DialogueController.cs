using System;
using UnityEngine;

public class Scene2DialogueController : MonoBehaviour
{

    [SerializeField] private TextAsset sceneDialogueCSV;

   void Start()
    {
        DialogueManager.Instance.LoadDialogueDatabase(sceneDialogueCSV);
    }

    public void FirstDoorDialogue(Action onFinished = null)
    {
        DialogueManager.Instance.StartDialogueGroup(
            "DoorLockLocked",
            () =>
            {
                onFinished?.Invoke();
            }
        );
    }

    public void DoorClearDialogue(Action onFinished = null)
    {
        DialogueManager.Instance.StartDialogueGroup(
            "DoorLClear",
            () =>
            {
                onFinished?.Invoke();
            }
        );
    }
}
