using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL.WebRequest
{
    internal class AudioClipRequestTask : RequestTask
    {
        OnRequestAudioComplete onCompleteEvent;

        AudioType audioType;

        internal AudioClipRequestTask(MonoBehaviour runner, string url, int timeout, bool disposeHandle, AudioType audioType, OnRequestAudioComplete onComplete) : base(runner, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
            this.audioType = audioType;
        }

        protected override void CreateRequest()
        {
            UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            request.disposeDownloadHandlerOnDispose = disposeHandle;
        }

        protected override void OnComplete()
        {
            if (onCompleteEvent != null) onCompleteEvent(error, DownloadHandlerAudioClip.GetContent(request));
            onCompleteEvent = null;
        }
    }
}
