using System;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    public bool IsPuzzleCleared { get; private set; } = false;

    // 퍼즐이 풀렸을 때 외부에 알리는 이벤트
    public event Action OnPuzzleClearedEvent;

    public void SolvePuzzle()
    {
        if (IsPuzzleCleared) return; // 이미 풀렸다면 중복 실행 방지

        IsPuzzleCleared = true;
        Debug.Log("퍼즐 클리어");

        // 이벤트를 구독하고 있는 대상에게 알림 발송
        OnPuzzleClearedEvent?.Invoke();
    }
}