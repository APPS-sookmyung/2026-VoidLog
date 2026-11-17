using UnityEngine;
using UnityEngine.Video;

namespace VoidLog.Core
{
    /// <summary>
    /// WebGL에서는 VideoClip 재생이 안 되므로 StreamingAssets의 mp4를 URL로 재생한다.
    /// 같은 오브젝트의 VideoPlayer에 URL을 넣고 미리 Prepare 해 둔다.
    /// 재생(Play)/정지(Stop)는 기존처럼 OpeningStepManager 이벤트에서 호출.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class ReplayVideoUrlSetter : MonoBehaviour
    {
        [Tooltip("StreamingAssets 기준 상대 경로")]
        [SerializeField] private string relativePath = "Video/Replay_Test.mp4";
        [SerializeField] private bool prepareOnAwake = true;

        private VideoPlayer videoPlayer;

        private void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
            videoPlayer.source = VideoSource.Url;
            videoPlayer.playOnAwake = false;

            // 소리 출력: Direct 모드로 첫 번째 오디오 트랙 재생
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetDirectAudioMute(0, false);
            videoPlayer.SetDirectAudioVolume(0, 1f);

            // WebGL에서 streamingAssetsPath는 http(s) URL이므로 Path.Combine 대신 "/"로 연결
            string url = Application.streamingAssetsPath + "/" + relativePath;
            videoPlayer.url = url;

            videoPlayer.errorReceived += OnError;

            Debug.Log($"[ReplayVideoUrlSetter] URL = {url}");

            if (prepareOnAwake)
                videoPlayer.Prepare();
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
                videoPlayer.errorReceived -= OnError;
        }

        private void OnError(VideoPlayer source, string message)
        {
            Debug.LogError($"[ReplayVideoUrlSetter] 영상 재생 오류: {message} (url: {source.url})");
        }
    }
}
