using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL.WebRequest
{
    internal class TextureRequestTask : RequestTask
    {
        OnRequestTextureComplete onCompleteEvent;
        /// <summary>
        /// true时可以关闭Texture2D图片的读写功能，节省内存
        /// </summary>
        bool nonReadable = true;

        internal TextureRequestTask(MonoBehaviour runner, string url, int timeout, bool disposeHandle, bool nonReadable, OnRequestTextureComplete onComplete) : base(runner, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
            this.nonReadable = nonReadable;
        }

        protected override void CreateRequest()
        {
            request = UnityWebRequestTexture.GetTexture(url, nonReadable);
            request.disposeDownloadHandlerOnDispose = disposeHandle;
        }

        protected override void OnComplete()
        {
            if (onCompleteEvent != null) onCompleteEvent(error, DownloadHandlerTexture.GetContent(request));
            onCompleteEvent = null;
        }
    }
}
