using UnityEngine;

// 퍼즐2 - 벽 퍼즐
public class Puzzle_Wall : MonoBehaviour
{
    [SerializeField] private Canvas wall;

    [SerializeField] private RectTransform movableImage;
    [SerializeField] private RectTransform answerPosition;
    [SerializeField] private GameObject correctImage;
    [SerializeField] private float clearDistance = 20f;

    private bool isCleared = false;

  void Start()
{
    wall.gameObject.SetActive(false);

    if (correctImage != null)
        correctImage.SetActive(false);

    if (SaveManager.Instance != null &&
        SaveManager.Instance.Data.hasSolvedWallPuzzle)
    {
        isCleared = true;

        movableImage.anchoredPosition =
            answerPosition.anchoredPosition;

        if (correctImage != null)
            correctImage.SetActive(true);
    }
}

    public void WallOpen()
    {
        wall.gameObject.SetActive(true);
    }

    // 드래그가 끝났을 때 호출
    public void CheckPuzzleClear()
    {
        CheckClear();
    }

    private void CheckClear()
    {
        if (isCleared) return;

        float distance = Vector2.Distance(
            movableImage.anchoredPosition,
            answerPosition.anchoredPosition
        );

        if (distance <= clearDistance)
        {
            ClearPuzzle();
        }
    }

    private void ClearPuzzle()
    {
        isCleared = true;

        // 움직이는 이미지를 정답 위치로 딱 맞춤
        movableImage.anchoredPosition =answerPosition.anchoredPosition;

        // 완성된 정답 이미지 표시
        if (correctImage != null)
        {
            correctImage.SetActive(true);
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.Data.hasSolvedWallPuzzle = true;
            SaveManager.Instance.SaveGame("FromControlRoom");
        }

        Debug.Log("[Puzzle_Wall] 퍼즐 클리어!");
    }
}