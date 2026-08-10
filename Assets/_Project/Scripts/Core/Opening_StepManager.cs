using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VoidLog.Core
{
    public class OpeningStepManager : MonoBehaviour
    {
        [System.Serializable]
        public class OpeningStep
        {
            public string stepName;
            public float duration = 3f;
            public UnityEvent onStepStart;
        }

        [SerializeField] private List<OpeningStep> steps = new List<OpeningStep>();
        [SerializeField] private UnityEvent onSequenceComplete;
        [SerializeField] private bool playOnStart = true;

        private int currentStepIndex = -1;
        private Coroutine sequenceRoutine;

        public bool IsPlaying { get; private set; }

        private void Start()
        {
            if (playOnStart)
            {
                StartSequence();
            }
        }

        public void StartSequence()
        {
            if (sequenceRoutine != null)
            {
                StopCoroutine(sequenceRoutine);
            }
            currentStepIndex = -1;
            sequenceRoutine = StartCoroutine(RunSequence());
        }

        public void SkipToNextStep()
        {
            if (!IsPlaying) return;

            if (sequenceRoutine != null)
            {
                StopCoroutine(sequenceRoutine);
            }
            sequenceRoutine = StartCoroutine(RunSequence());
        }

        private IEnumerator RunSequence()
        {
            IsPlaying = true;
            currentStepIndex++;

            while (currentStepIndex < steps.Count)
            {
                OpeningStep step = steps[currentStepIndex];

                Debug.Log($"[OpeningStepManager] 시작: {step.stepName} (지속시간 {step.duration}초)");
                step.onStepStart?.Invoke();

                yield return new WaitForSeconds(step.duration);

                currentStepIndex++;
            }

            IsPlaying = false;
            Debug.Log("[OpeningStepManager] 전체 시퀀스 완료");
            onSequenceComplete?.Invoke();
        }
    }
}
