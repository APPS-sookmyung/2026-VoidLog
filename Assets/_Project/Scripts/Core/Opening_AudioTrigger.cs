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

        private Dictionary<string, AudioSource> activeSources = new Dictionary<string, AudioSource>();
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

            if (activeSources.ContainsKey(id)) return;

            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = entry.clip;
            source.volume = entry.volume;
            source.loop = entry.loop;
            source.Play();

            activeSources[id] = source;

            if (!entry.loop)
            {
                StartCoroutine(CleanupAfterPlay(id, entry.clip.length));
            }
        }

        public void Stop(string id)
        {
            if (activeSources.TryGetValue(id, out var source))
            {
                source.Stop();
                Destroy(source);
                activeSources.Remove(id);
            }
        }

        public void FadeOutAndStop(string id, float duration)
        {
            if (activeSources.TryGetValue(id, out var source))
            {
                StartCoroutine(FadeOutRoutine(id, source, duration));
            }
        }

        private IEnumerator FadeOutRoutine(string id, AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            source.Stop();
            Destroy(source);
            activeSources.Remove(id);
        }

        private IEnumerator CleanupAfterPlay(string id, float clipLength)
        {
            yield return new WaitForSeconds(clipLength);

            if (activeSources.TryGetValue(id, out var source))
            {
                Destroy(source);
                activeSources.Remove(id);
            }
        }
    }
}