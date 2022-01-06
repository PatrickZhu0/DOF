using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
//using Object = UnityEngine.Object;

namespace GameDLL.WebRequest
{
    public enum RequestStatus
    {
        Queued,
        Running,
        Completed,
        Stopped,
    }

    public abstract class RequestTask
    {
        public delegate void OnRequestTextComplete(string error, string response);

        public delegate void OnRequestDataComplete(string error, byte[] response);

        public delegate void OnRequestTextureComplete(string error, Texture2D response);

        public delegate void OnRequestAudioComplete(string error, AudioClip response);

        public delegate void OnRequestFileComplete(string error, string saveFilePath);

        /// <summary>
        /// 挂载的脚本
        /// </summary>
        public MonoBehaviour runner { get; protected set; }

        public string method { get; protected set; }

        /// <summary>
        /// 通讯的地址
        /// </summary>
        public string url { get; protected set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public RequestStatus status = RequestStatus.Queued;

        /// <summary>
        /// 当前进度
        /// </summary>
        public float progress { get { return request == null ? 0 : (method == UnityWebRequest.kHttpVerbPUT ? request.uploadProgress : request.downloadProgress); } }

        /// <summary>
        /// 通讯完成
        /// </summary>
        public bool isDone { get { return request == null ? (status == RequestStatus.Completed | status == RequestStatus.Stopped ? true : false) : request.isDone; } }

        public string error { get; protected set; }

        public ulong bytesSize { get; protected set; }

        /// <summary>
        /// 回调进度
        /// </summary>
        internal Action<float> onProgressEvent;

        /// <summary>
        /// 进度用协程
        /// </summary>
        private Coroutine _coroutine;
        public UnityWebRequest request { get; protected set; }

        /// <summary>
        /// 超时强制关闭通讯
        /// </summary>
        protected int timeout;

        protected bool disposeHandle;

        internal RequestTask(MonoBehaviour runner, string method, string url, int timeout, bool disposeHandle)
        {
            this.runner = runner;
            this.method = method.ToUpper();
            this.url = url;
            this.timeout = timeout;
            this.disposeHandle = disposeHandle;
        }

        internal RequestTask(MonoBehaviour runner, string url, int timeout, bool disposeHandle) : this(runner, UnityWebRequest.kHttpVerbGET, url, timeout, disposeHandle)
        {

        }

        /// <summary>
        /// 开始任务
        /// </summary>
        internal void StartTask()
        {
            if (status != RequestStatus.Queued) return;
            runner.StartCoroutine(RunTask());
            status = RequestStatus.Running;
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        internal void StopTask()
        {
            if (request != null)
            {
#if UNITY_2019
                if (request.isHttpError || request.isNetworkError)
#else
                if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
#endif
                {
                    status = RequestStatus.Stopped;                //被切换成stop
                    error = request.error;
                    GLog.ErrorFormat("UnityWebRequest failed!!! {0}", error);
                }
                else
                {
                    status = RequestStatus.Completed;
                    bytesSize = method == UnityWebRequest.kHttpVerbPUT ? request.uploadedBytes : request.downloadedBytes;
                }
            }
            OnComplete();
            WebRequestManager.Instance.OnComplete(this);
            if (_coroutine != null) runner.StopCoroutine(_coroutine);
            if (request != null) request.Dispose();
            request = null;
        }

        public bool Stop()
        {
            if (request != null)
            {
                request.Abort();
                return true;
            }
            return false;
        }

        private IEnumerator UpdateProgress()
        {
            while (!isDone)
            {
                onProgressEvent(progress);
                yield return null;
            }
        }

        private IEnumerator RunTask()
        {
            try
            {
                CreateRequest();
            }
            catch(Exception e)
            {
                //by Rick, 创建时可能会出错，这里需要保护下，保证回调能调到
                error = e.Message;
                StopTask();
                yield break;
            }
            if (request == null)
            {
                status = RequestStatus.Stopped;                //被切换成stop
                error = "Request is null";
                StopTask();
                yield break;
            }

            request.timeout = this.timeout;

            if (onProgressEvent != null)
                _coroutine = runner.StartCoroutine(UpdateProgress());

            yield return request.SendWebRequest();//等待返回请求的信息

            StopTask();
        }

        protected abstract void CreateRequest();
        protected abstract void OnComplete();

        public override string ToString()
        {
            return string.Format("[{0}] url = {1}, method = {2}, runner = {3} status = {4}", GetType(), url, method, runner, status);
        }
    }
}