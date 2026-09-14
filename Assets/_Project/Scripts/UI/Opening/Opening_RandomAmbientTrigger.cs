using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoidLog.Core
{
    public class RandomAmbientTrigger : MonoBehaviour
    {
        [SerializeField] private AudioTrigger audioTrigger;
        [SerializeField] private VoidLog.UI.GlitchController glitchController;

        [Tooltip("AudioTrigger에 등록된 사운드 id 중 랜덤으로 재생할 목록 (발소리, 안내방송 등)")]
        [SerializeField] private List<string> soundIds = new List<string>();

        [SerializeField] private float minInterval = 3f;
        [SerializeField] private float maxInterval = 8f;
        [SerializeField] private float glitchDuration = 0.25f;

        private Coroutine loopRoutine;

        public void StartLoop()
        {
            if (loopRoutine != null) StopCoroutine(loopRoutine);
            loopRoutine = StartCoroutine(LoopRoutine());
        }

        public void StopLoop()
        {
            if (loopRoutine != null)
            {
                StopCoroutine(loopRoutine);
                loopRoutine = null;
            }
        }

        private IEnumerator LoopRoutine()
        {
            while (true)
            {
                float wait = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(wait);

                if (soundIds.Count > 0 && audioTrigger != null)
                {
                    string id = soundIds[Random.Range(0, soundIds.Count)];
                    audioTrigger.Play(id);
                }

                if (glitchController != null)
                {
                    glitchController.TriggerGlitch(glitchDuration);
                }
            }
        }
    }
}