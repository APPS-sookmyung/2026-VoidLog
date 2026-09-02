using System.Collections;
using UnityEngine;

namespace VoidLog.UI
{

    public class ShadowHandMover : MonoBehaviour
    {
        [SerializeField] private RectTransform handTransform;

        [Header("떨림 설정 (Step2)")]
        [SerializeField] private float jitterAmount = 4f;
        [SerializeField] private float jitterSpeed = 25f;

        [Header("이동 설정 (Step3)")]
        [SerializeField] private RectTransform ventTargetPoint;
        [SerializeField] private float moveDuration = 2.5f;

        private Vector2 basePosition;
        private Coroutine activeRoutine;
        private bool jittering;

        private void Awake()
        {
            if (handTransform == null)
            {
                handTransform = GetComponent<RectTransform>();
            }
            basePosition = handTransform.anchoredPosition;
        }

        private void Update()
        {
            if (!jittering || handTransform == null) return;

            float offsetX = (Mathf.PerlinNoise(Time.time * jitterSpeed, 0f) - 0.5f) * jitterAmount;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * jitterSpeed) - 0.5f) * jitterAmount;
            handTransform.anchoredPosition = basePosition + new Vector2(offsetX, offsetY);
        }

        public void StartJitter()
        {
            jittering = true;
        }

        public void StopJitter()
        {
            jittering = false;
            if (handTransform != null)
            {
                handTransform.anchoredPosition = basePosition;
            }
        }

        public void MoveToVent()
        {
            if (ventTargetPoint == null || handTransform == null) return;

            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }
            activeRoutine = StartCoroutine(MoveRoutine(ventTargetPoint.anchoredPosition, moveDuration));
        }

        private IEnumerator MoveRoutine(Vector2 target, float duration)
        {
            Vector2 start = handTransform.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                handTransform.anchoredPosition = Vector2.Lerp(start, target, eased);
                yield return null;
            }

            handTransform.anchoredPosition = target;
            activeRoutine = null;
        }
    }
}

