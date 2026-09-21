using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoidLog.Core
{
    public class AudioTrigger : MonoBehaviour
    {
        [System.Serializable]
        public class SoundEntry
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public bool loop = false;
        }

        [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

        [Header("UnityEvent Inspector에서 파라미터 1개로 노출하기 위한 기본 페이드아웃 시간")]
        [SerializeField] private float defaultFadeOutDuration = 1.5f;

        private Dictionary<string, AudioSource> activeSources = new Dictionary<string, AudioSource>();
        private Dictionary<string, Coroutine> lifecycleRoutines = new Dictionary<string, Coroutine>();
        private Dictionary<string, SoundEntry> soundLookup;

        private void Awake()
        {
            soundLookup = new Dictionary<string, SoundEntry>();
            foreach (var entry in sounds)
            {
                if (!soundLookup.ContainsKey(entry.id))
                {
                    soundLookup.Add(entry.id, entry);
                }
                else
                {
                    Debug.LogWarning($"[AudioTrigger] 중복된 사운드 id 발견: {entry.id}");
                }
            }
        }

        public void Play(string id)
        {
            if (!soundLookup.TryGetValue(id, out var entry))
            {
                Debug.LogWarning($"[AudioTrigger] '{id}' 사운드를 찾을 수 없습니다.");
                return;
            }

            if (entry.clip == null)
            {
                Debug.LogWarning($"[AudioTrigger] '{id}' 사운드에 AudioClip이 연결되어 있지 않습니다.");
                return;
            }

            if (activeSources.ContainsKey(id)) return;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = entry.clip;
            source.volume = entry.volume;
            source.loop = entry.loop;
            source.Play();

            activeSources[id] = source;

            if (!entry.loop)
            {
                Coroutine routine = StartCoroutine(CleanupAfterPlay(id, source, entry.clip.length));
                lifecycleRoutines[id] = routine;
            }
        }

        public void Stop(string id)
        {
            CancelLifecycleRoutine(id);

            if (activeSources.TryGetValue(id, out var source))
            {
                if (source != null)
                {
                    source.Stop();
                    Destroy(source);
                }
                activeSources.Remove(id);
            }
        }

        public void FadeOutAndStop(string id)
        {
            FadeOutAndStop(id, defaultFadeOutDuration);
        }

        public void FadeOutAndStop(string id, float duration)
        {
            if (!activeSources.TryGetValue(id, out var source) || source == null)
            {
                return;
            }

            CancelLifecycleRoutine(id);

            Coroutine fadeRoutine = StartCoroutine(FadeOutRoutine(id, source, duration));
            lifecycleRoutines[id] = fadeRoutine;
        }

        private IEnumerator FadeOutRoutine(string id, AudioSource source, float duration)
        {
            float startVolume = source != null ? source.volume : 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (source == null) yield break; // 다른 경로로 이미 파괴된 경우 안전하게 종료

                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
            activeSources.Remove(id);
            lifecycleRoutines.Remove(id);
        }

        private IEnumerator CleanupAfterPlay(string id, AudioSource source, float clipLength)
        {
            yield return new WaitForSeconds(clipLength);

            if (source != null)
            {
                Destroy(source);
            }
            activeSources.Remove(id);
            lifecycleRoutines.Remove(id);
        }

        private void CancelLifecycleRoutine(string id)
        {
            if (lifecycleRoutines.TryGetValue(id, out var routine) && routine != null)
            {
                StopCoroutine(routine);
            }
            lifecycleRoutines.Remove(id);
        }
    }
}