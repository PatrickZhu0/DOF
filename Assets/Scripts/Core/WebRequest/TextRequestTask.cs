using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL.WebRequest
{
    public class TextRequestTask : RequestTask
    {
        /// <summary>
        /// post参数WWWForm
        /// </summary>
        public WWWForm postForm { get; protected set; }

        /// <summary>
        /// post参数Dictionary<string, string>
        /// </summary>
        public Dictionary<string, string> postDic { get; protected set; }

        OnRequestTextComplete onCompleteEvent;

        internal TextRequestTask(MonoBehaviour runner, string url, int timeout, bool disposeHandle, OnRequestTextComplete onComplete) : base(runner, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
        }

        internal TextRequestTask(MonoBehaviour runner, string method, string url, int timeout, bool disposeHandle, OnRequestTextComplete onComplete) : base(runner, method, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
        }

        internal TextRequestTask(MonoBehaviour runner, WWWForm postData, string url, int timeout, bool disposeHandle, OnRequestTextComplete onComplete) : base(runner, UnityWebRequest.kHttpVerbPOST, url, timeout, disposeHandle)
        {
            this.postForm = postData;
            onCompleteEvent = onComplete;
        }

        internal TextRequestTask(MonoBehaviour runner, Dictionary<string, string> postDic, string url, int timeout, bool disposeHandle, OnRequestTextComplete onComplete) : base(runner, UnityWebRequest.kHttpVerbPOST, url, timeout, disposeHandle)
        {
            this.postDic = postDic;
            onCompleteEvent = onComplete;
        }

        protected override void CreateRequest()
        {
            switch (method)
            {
                case UnityWebRequest.kHttpVerbGET:
                    request = UnityWebRequest.Get(url);
                    request.disposeDownloadHandlerOnDispose = disposeHandle;
                    break;
                case UnityWebRequest.kHttpVerbPOST:
                    if (postForm != null)
                        request = UnityWebRequest.Post(url, postForm);
                    else if(postDic != null)
                        request = UnityWebRequest.Post(url, postDic);
                    if (request != null)
                        request.disposeDownloadHandlerOnDispose = disposeHandle;
                    break;
                case UnityWebRequest.kHttpVerbHEAD:
                    request = UnityWebRequest.Head(url);
                    request.disposeDownloadHandlerOnDispose = disposeHandle;
                    break;
            }
        }

        protected override void OnComplete()
        {
            if (onCompleteEvent != null) onCompleteEvent(error, method == UnityWebRequest.kHttpVerbHEAD ? null : request.downloadHandler.text);
            postForm = null;
            postDic = null;
            onCompleteEvent = null;
        }
    }
}
