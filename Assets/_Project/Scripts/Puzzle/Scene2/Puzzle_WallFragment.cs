using UnityEngine;
using UnityEngine.EventSystems;

// 퍼즐2 - 벽 퍼즐 (드래그 물체 스크립트)
public class Puzzle_WallFragment : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    [SerializeField] private Puzzle_Wall puzzleWall;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 캔버스 스케일에 맞춰 마우스 이동량 반영
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 마우스를 놓았을 때 정답 위치인지 검사
        if (puzzleWall != null)
        {
            puzzleWall.CheckPuzzleClear();
        }
    }
}