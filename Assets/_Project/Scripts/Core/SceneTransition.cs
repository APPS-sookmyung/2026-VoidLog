using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoidLog.UI;

namespace VoidLog.Core
{

    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        [SerializeField] private FadeController fadeController;
        [SerializeField] private float fadeOutDuration = 1.5f;

        public void GoToNextScene()
        {
            LoadScene(nextSceneName);
        }

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("[SceneTransition] 전환할 씬 이름이 비어있습니다. Inspector에서 nextSceneName을 설정하세요.");
                return;
            }

            if (fadeController != null)
            {
                StartCoroutine(FadeThenLoad(sceneName));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        private IEnumerator FadeThenLoad(string sceneName)
        {
            bool fadeDone = false;
            fadeController.FadeOut(fadeOutDuration, () => fadeDone = true);
            
            yield return new WaitUntil(() => fadeDone);
            SceneManager.LoadScene(sceneName);
        }
    }
}