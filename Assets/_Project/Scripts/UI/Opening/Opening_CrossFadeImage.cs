using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidLog.UI
{
    [RequireComponent(typeof(Image))]
    public class CrossfadeImage : MonoBehaviour
    {
        [SerializeField] private Image mainImage;
        [SerializeField] private float defaultCrossfadeDuration = 0.5f;

        [Header("켄 번즈 효과 (정지 상태에서도 천천히 확대/이동)")]
        [Tooltip("확대 폭. 0.03이면 100%~103% 사이를 천천히 오간다")]
        [SerializeField] private float zoomAmplitude = 0.035f;
        [Tooltip("확대/이동이 한 사이클 도는 데 걸리는 대략적인 시간(초). 느릴수록 자연스럽다")]
        [SerializeField] private float motionPeriod = 10f;
        [Tooltip("좌우/상하로 살짝 흔들리는 폭 (픽셀)")]
        [SerializeField] private float panAmplitude = 14f;

        private Coroutine routine;
        private RectTransform mainRect;
        private Vector2 basePosition;
        private float motionSeed;

        private void Awake()
        {
            if (mainImage == null)
            {
                mainImage = GetComponent<Image>();
            }
            mainRect = mainImage.rectTransform;
            basePosition = mainRect.anchoredPosition;
            motionSeed = Random.Range(0f, 100f); // 여러 오브젝트에 같이 써도 서로 다른 움직임이 되도록
        }

        private void Update()
        {
            if (mainRect == null) return;

            float speed = (2f * Mathf.PI) / Mathf.Max(0.1f, motionPeriod);
            float t = Time.time * speed;

            // 확대: 1.0 ~ (1.0 + zoomAmplitude) 사이를 천천히 오간다
            float zoom = 1f + zoomAmplitude * (0.5f + 0.5f * Mathf.Sin(t + motionSeed));
            mainRect.localScale = Vector3.one * zoom;

            // 살짝 팬(이동): x, y 서로 다른 주기로 오가며 자연스러운 표류 느낌
            float panX = Mathf.Sin(t * 0.7f + motionSeed) * panAmplitude;
            float panY = Mathf.Cos(t * 0.5f + motionSeed) * panAmplitude * 0.6f;
            mainRect.anchoredPosition = basePosition + new Vector2(panX, panY);
        }

        public void SetSprite(Sprite newSprite)
        {
            SetSprite(newSprite, defaultCrossfadeDuration);
        }

        public void SetSprite(Sprite newSprite, float duration)
        {
            if (newSprite == null || mainImage == null) return;

            if (routine != null)
            {
                StopCoroutine(routine);
            }
            routine = StartCoroutine(CrossfadeRoutine(newSprite, duration));
        }

        private IEnumerator CrossfadeRoutine(Sprite newSprite, float duration)
        {
            Sprite oldSprite = mainImage.sprite;

            GameObject ghostObj = new GameObject("CrossfadeGhost", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform ghostRect = ghostObj.GetComponent<RectTransform>();
            ghostObj.transform.SetParent(mainImage.transform.parent, false);

            RectTransform mRect = mainImage.rectTransform;
            ghostRect.anchorMin = mRect.anchorMin;
            ghostRect.anchorMax = mRect.anchorMax;
            ghostRect.pivot = mRect.pivot;
            ghostRect.anchoredPosition = mRect.anchoredPosition;
            ghostRect.sizeDelta = mRect.sizeDelta;
            ghostRect.localScale = mRect.localScale; // 지금 확대된 상태 그대로 이어받기
            ghostRect.SetSiblingIndex(mainImage.transform.GetSiblingIndex());

            Image ghostImg = ghostObj.GetComponent<Image>();
            ghostImg.sprite = oldSprite;
            ghostImg.color = mainImage.color;
            ghostImg.raycastTarget = false;

            Color c = mainImage.color;
            mainImage.sprite = newSprite;
            c.a = 0f;
            mainImage.color = c;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / duration);
                mainImage.color = c;

                // 유령 이미지도 같이 확대/이동시켜서 전환 중에도 어긋나 보이지 않게
                ghostRect.localScale = mainRect.localScale;
                ghostRect.anchoredPosition = mainRect.anchoredPosition;

                yield return null;
            }

            c.a = 1f;
            mainImage.color = c;

            Destroy(ghostObj);
            routine = null;
        }
    }
}