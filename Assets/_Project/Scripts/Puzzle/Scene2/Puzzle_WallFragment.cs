using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//퍼즐2 - 벽 퍼즐 (드레그 물체 스크립트)
public class Puzzle_WallFragment : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 처리할 내용 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 캔버스 스케일에 맞춰 마우스 이동량 반영
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 처리할 내용 
    }
}
