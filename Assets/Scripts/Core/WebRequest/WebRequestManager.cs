using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace GameDLL.WebRequest
{
    public class WebRequestManager : Singleton<WebRequestManager>, IManager
    {
        private IGame _igame;

        /// <summary>
        /// 允许同时运行的任务数量
        /// </summary>
        [Range(1, 10)]
        private int _maxTask = 3;

        /// <summary>
        /// 允许同时运行的任务数量
        /// </summary>
        public int MaxTask
        {
            get
            {
                return _maxTask;
            }
            set
            {
                if (value > _maxTask)
                {
                    _maxTask = value;
                    while (NextTask()) ;        //直接启动等待的任务
                }
                else if (value < _maxTask)
                    _maxTask = value;
            }
        }

        /// <summary>
        /// 运行的任务
        /// </summary>
        private List<RequestTask> _taskList = new List<RequestTask>();

        /// <summary>
        /// 等待的任务
        /// </summary>
        private Queue<RequestTask> _taskQueue = new Queue<RequestTask>();

        /// <summary>
        /// 默认任务超时时间15秒
        /// </summary>
        private const int TakeTimeOut = 15;

        public static RequestTask GetString(string url, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            TextRequestTask task = new TextRequestTask(IGame.GetInstance().GetMainLoop(), url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void GetStringNoReturn(string url, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            GetString(url, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask GetData(string url, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            DataRequestTask task = new DataRequestTask(IGame.GetInstance().GetMainLoop(), url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void GetDataNoReturn(string url, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            GetData(url, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask GetTexture(string url, RequestTask.OnRequestTextureComplete onComplete = null, int timeout = TakeTimeOut, bool nonReadable = true, bool disposeHandle = true, Action<float> onProgress = null)
        {
            TextureRequestTask task = new TextureRequestTask(IGame.GetInstance().GetMainLoop(), url, timeout, disposeHandle, nonReadable, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void GetTextureNoReturn(string url, RequestTask.OnRequestTextureComplete onComplete = null, int timeout = TakeTimeOut, bool nonReadable = true, bool disposeHandle = true, Action<float> onProgress = null)
        {
            GetTexture(url, onComplete, timeout, nonReadable, disposeHandle, onProgress);
        }

        public static RequestTask GetAudioClip(string url, RequestTask.OnRequestAudioComplete onComplete = null, int timeout = TakeTimeOut, AudioType audioType = AudioType.UNKNOWN, bool disposeHandle = true, Action<float> onProgress = null)
        {
            AudioClipRequestTask task = new AudioClipRequestTask(IGame.GetInstance().GetMainLoop(), url, timeout, disposeHandle, audioType, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void GetAudioClipNoReturn(string url, RequestTask.OnRequestAudioComplete onComplete = null, int timeout = TakeTimeOut, AudioType audioType = AudioType.UNKNOWN, bool disposeHandle = true, Action<float> onProgress = null)
        {
            GetAudioClip(url, onComplete, timeout, audioType, disposeHandle, onProgress);
        }

        public static RequestTask GetFile(string url, string saveFilePath, RequestTask.OnRequestFileComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            FileRequestTask task = new FileRequestTask(IGame.GetInstance().GetMainLoop(), url, saveFilePath, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void GetFileNoReturn(string url, string saveFilePath, RequestTask.OnRequestFileComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            GetFile(url, saveFilePath, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask PostString(string url, Dictionary<string, string> postDic, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            TextRequestTask task = new TextRequestTask(IGame.GetInstance().GetMainLoop(), postDic, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void PostStringNoReturn(string url, Dictionary<string, string> postDic, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PostString(url, postDic, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask PostString(string url, WWWForm postData, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            TextRequestTask task = new TextRequestTask(IGame.GetInstance().GetMainLoop(), postData, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void PostStringNoReturn(string url, WWWForm postData, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PostString(url, postData, onComplete, timeout, disposeHandle,onProgress);
        }

        public static RequestTask PostData(string url, Dictionary<string, string> postDic, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            DataRequestTask task = new DataRequestTask(IGame.GetInstance().GetMainLoop(), postDic, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void PostDataNoReturn(string url, Dictionary<string, string> postDic, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PostData(url, postDic, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask PostData(string url, WWWForm postData, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            DataRequestTask task = new DataRequestTask(IGame.GetInstance().GetMainLoop(), postData, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void PostDataNoReturn(string url, WWWForm postData, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PostData(url, postData, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask HeadString(string url, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            TextRequestTask task = new TextRequestTask(IGame.GetInstance().GetMainLoop(), UnityWebRequest.kHttpVerbHEAD, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void HeadStringNoReturn(string url, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            HeadString(url, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask HeadData(string url, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            DataRequestTask task = new DataRequestTask(IGame.GetInstance().GetMainLoop(), UnityWebRequest.kHttpVerbHEAD, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void HeadDataNoReturn(string url, RequestTask.OnRequestDataComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            HeadData(url, onComplete, timeout, disposeHandle, onProgress);
        }

        public static RequestTask PutData(string url, byte[] putData, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PutRequestTask task = new PutRequestTask(IGame.GetInstance().GetMainLoop(), putData, url, timeout, disposeHandle, onComplete);
            task.onProgressEvent = onProgress;
            Instance.AddTask(task);
            return task;
        }

        public static void PutDataNoReturn(string url, byte[] putData, RequestTask.OnRequestTextComplete onComplete = null, int timeout = TakeTimeOut, bool disposeHandle = true, Action<float> onProgress = null)
        {
            PutData(url, putData, onComplete, timeout, disposeHandle, onProgress);
        }

        private void AddTask(RequestTask task)
        {
            if (_taskList.Count >= _maxTask)
                _taskQueue.Enqueue(task);
            else
            {
                _taskList.Add(task);
                task.StartTask();
            }
        }

        /// <summary>
        /// 执行下一个任务
        /// </summary>
        private bool NextTask()
        {
            if (_taskQueue.Count <= 0 || _taskList.Count >= _maxTask) return false;
            RequestTask task = _taskQueue.Dequeue();
            _taskList.Add(task);
            task.StartTask();
            return true;
        }

        /// <summary>
        /// 完成当前任务，并检测下一个任务
        /// </summary>
        internal void OnComplete(RequestTask task)
        {
            //GLog.LogFormat("Task over-> {0}", task.url);
            _taskList.Remove(task);
            NextTask();
        }

        #region 管理器接口函数
        public void Init(IGame game)
        {
            _igame = game;
        }

        public void Reset()
        {

        }

        public void Dispose()
        {
            _taskQueue.Clear();
            for(int i = _taskList.Count - 1; i >= 0; i--)
                _taskList[i].StopTask();
            _taskList.Clear();
        }
        #endregion
    }
}