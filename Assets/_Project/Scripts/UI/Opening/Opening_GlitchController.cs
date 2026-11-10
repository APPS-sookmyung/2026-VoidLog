using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoidLog.UI
{
    [RequireComponent(typeof(Image))]
    public class GlitchController : MonoBehaviour
    {
        [SerializeField] private Image glitchImage;
        [SerializeField] private Material glitchMaterialTemplate;

        [Header("기본 트리거 설정 (UnityEvent로 파라미터 없이 호출할 때 사용)")]
        [SerializeField] private float defaultDuration = 0.25f;
        [SerializeField] private float defaultIntensity = 1f;

        [Header("떨림의 불규칙함 정도 (0이면 항상 매끈한 삼각파)")]
        [Range(0f, 1f)]
        [SerializeField] private float flickerRandomness = 0.4f;

        private Material glitchMaterialInstance;
        private Coroutine glitchRoutine;

        private static readonly int NoiseIntensityID = Shader.PropertyToID("_NoiseIntensity");
        private static readonly int RGBSplitID = Shader.PropertyToID("_RGBSplit");
        private static readonly int LineJitterID = Shader.PropertyToID("_LineJitter");
        private static readonly int SeedID = Shader.PropertyToID("_Seed");

        private void Awake()
        {
            if (glitchImage == null)
            {
                glitchImage = GetComponent<Image>();
            }

            SetImageAlpha(0f);

            if (glitchMaterialTemplate == null)
            {
                Debug.LogWarning("[GlitchController] glitchMaterialTemplate이 연결되지 않았습니다. Inspector에서 UI_Glitch 머티리얼을 연결하세요.");
                return;
            }

            glitchMaterialInstance = new Material(glitchMaterialTemplate);
            glitchImage.material = glitchMaterialInstance;
            glitchImage.raycastTarget = false;

            SetIntensity(0f);
        }

        

        public void TriggerGlitchEvent()
        {
            TriggerGlitch(defaultDuration, defaultIntensity);
        }

        public void TriggerGlitch(float duration, float intensity = 1f)
        {
            if (glitchMaterialInstance == null) return;

            if (glitchRoutine != null)
            {
                StopCoroutine(glitchRoutine);
            }
            glitchRoutine = StartCoroutine(GlitchRoutine(duration, intensity));
        }

        public void StopGlitch()
        {
            if (glitchRoutine != null)
            {
                StopCoroutine(glitchRoutine);
                glitchRoutine = null;
            }
            SetIntensity(0f);
        }

        private IEnumerator GlitchRoutine(float duration, float intensity)
        {
            SetImageAlpha(1f);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float envelope = 1f - Mathf.Abs(t * 2f - 1f);
                
                float flicker = Mathf.Lerp(1f, Random.Range(0.5f, 1f), flickerRandomness);

                SetIntensity(intensity * envelope * flicker);

                if (glitchMaterialInstance != null)
                {
                    glitchMaterialInstance.SetFloat(SeedID, Random.value * 100f);
                }

                yield return null;
            
            }

            SetIntensity(0f);
            SetImageAlpha(0f);
            glitchRoutine = null;
        }

        private void SetIntensity(float value)
        {
            if (glitchMaterialInstance == null) return;

            glitchMaterialInstance.SetFloat(NoiseIntensityID, value);
            glitchMaterialInstance.SetFloat(RGBSplitID, value * 0.02f);
            glitchMaterialInstance.SetFloat(LineJitterID, value);
        }

        private void SetImageAlpha(float alpha)
        {
            if (glitchImage == null) return;
            Color c = glitchImage.color;
            c.a = alpha;
            glitchImage.color = c;
        }
    }
}
