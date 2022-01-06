using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDLL.WebRequest
{
    public class PutRequestTask : RequestTask
    {
        OnRequestTextComplete onCompleteEvent;
        /// <summary>
        /// put参数
        /// </summary>
        public byte[] putData { get; protected set; }

        internal PutRequestTask(MonoBehaviour runner, byte[] putData, string url, int timeout, bool disposeHandle, OnRequestTextComplete onComplete) : base(runner, UnityWebRequest.kHttpVerbPUT, url, timeout, disposeHandle)
        {
            this.putData = putData;
            this.onCompleteEvent = onComplete;
        }

        protected override void CreateRequest()
        {
            request = UnityWebRequest.Put(url, putData);
            request.disposeDownloadHandlerOnDispose = disposeHandle;
            request.disposeUploadHandlerOnDispose = disposeHandle;
        }

        protected override void OnComplete()
        {
            if (onCompleteEvent != null) onCompleteEvent(error, request.downloadHandler.text);
        }
    }
}
