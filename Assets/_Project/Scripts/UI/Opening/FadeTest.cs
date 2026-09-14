using UnityEngine;
using VoidLog.UI;

public class FadeTest : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;

    private void Start()
    {
        Invoke(nameof(StartFadeIn), 2f);
    }

    private void StartFadeIn()
    {
        fadeController.FadeIn(3f, () =>
        {
            Debug.Log("Fade In 완료!");
        });
    }
}