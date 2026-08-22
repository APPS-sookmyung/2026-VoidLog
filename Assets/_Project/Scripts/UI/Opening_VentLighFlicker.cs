using UnityEngine;
using UnityEngine.UI;

namespace VoidLog.UI
{
    [RequireComponent(typeof(Image))]
    public class VentLightFlicker : MonoBehaviour
    {
        [SerializeField] private Image lightImage;
        [SerializeField] private float minAlpha = 0.15f;
        [SerializeField] private float maxAlpha = 0.6f;
        [SerializeField] private float flickerSpeed = 3f;

        private bool flickering;

        private void Awake()
        {
            if (lightImage == null)
            {
                lightImage = GetComponent<Image>();
            }
            SetAlpha(0f);
        }

        public void StartFlicker()
        {
            flickering = true;
        }

        public void StopFlicker()
        {
            flickering = false;
            SetAlpha(0f);
        }

        private void Update()
        {
            if (!flickering) return;

            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.5f);
            SetAlpha(Mathf.Lerp(minAlpha, maxAlpha, noise));
        }

        private void SetAlpha(float alpha)
        {
            if (lightImage == null) return;
            Color c = lightImage.color;
            c.a = alpha;
            lightImage.color = c;
        }
    }
}