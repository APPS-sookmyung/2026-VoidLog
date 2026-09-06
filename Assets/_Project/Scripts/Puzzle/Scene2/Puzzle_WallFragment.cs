using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//퍼즐2 - 벽 퍼즐 (드레그 물체 스크립트)
public class Puzzle_WallFragment : MonoBehaviour, IDragHandler
{

    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 캔버스 스케일에 맞춰 마우스 이동량 반영
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

}
