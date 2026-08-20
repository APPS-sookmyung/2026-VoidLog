using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    // 외부에서는 읽기만 가능하고, 내부에서만 값 변경 가능
    public bool IsPuzzleCleared { get; private set; } = false;

    // 퍼즐 정답을 맞췄을 때 호출하는 메서드
    public void SolvePuzzle()
    {
        IsPuzzleCleared = true;
        Debug.Log("퍼즐 클리어");
    }
}