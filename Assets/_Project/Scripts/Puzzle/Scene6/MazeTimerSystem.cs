using UnityEngine;
using TMPro;

/// <summary>
/// 게임 시작과 함께 카운트다운. 0초가 되면 GameManager.OnFail() 호출.
/// 화면에 시간 표시하려면 TextMeshProUGUI 연결.
/// </summary>
public class MazeTimerSystem : MonoBehaviour
{
    [SerializeField] private float timeLimit = 180f;
    [SerializeField] private TMP_Text timerText;

    private float remaining;
    private bool isRunning = true;

    private void Start()
    {
        remaining = timeLimit;
        UpdateUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        remaining -= Time.deltaTime;

        if (remaining <= 0f)
        {
            remaining = 0f;
            isRunning = false;
            UpdateUI();
            MazeGameManager.Instance.OnFail();
            return;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    /// 게임 성공 시 타이머 멈추고 싶을 때 GameManager에서 호출
    public void StopTimer() => isRunning = false;
}
