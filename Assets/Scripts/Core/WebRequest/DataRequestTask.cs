using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL.WebRequest
{
    public class DataRequestTask : RequestTask
    {
        /// <summary>
        /// post参数WWWForm
        /// </summary>
        public WWWForm postForm { get; protected set; }

        /// <summary>
        /// post参数Dictionary<string, string>
        /// </summary>
        public Dictionary<string, string> postDic { get; protected set; }

        OnRequestDataComplete onCompleteEvent;

        internal DataRequestTask(MonoBehaviour runner, string url, int timeout, bool disposeHandle, OnRequestDataComplete onComplete) : base(runner, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
        }

        internal DataRequestTask(MonoBehaviour runner, string method, string url, int timeout, bool disposeHandle, OnRequestDataComplete onComplete) : base(runner, method, url, timeout, disposeHandle)
        {
            onCompleteEvent = onComplete;
        }

        internal DataRequestTask(MonoBehaviour runner, WWWForm postForm, string url, int timeout, bool disposeHandle, OnRequestDataComplete onComplete) : base(runner, UnityWebRequest.kHttpVerbPOST, url, timeout, disposeHandle)
        {
            this.postForm = postForm;
            onCompleteEvent = onComplete;
        }

        internal DataRequestTask(MonoBehaviour runner, Dictionary<string, string> postDic, string url, int timeout, bool disposeHandle, OnRequestDataComplete onComplete) : base(runner, UnityWebRequest.kHttpVerbPOST, url, timeout, disposeHandle)
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
                    else if (postDic != null)
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
            if (onCompleteEvent != null) onCompleteEvent(error, method == UnityWebRequest.kHttpVerbHEAD ? null : request.downloadHandler.data);
            postForm = null;
            postDic = null;
            onCompleteEvent = null;
        }
    }
}
