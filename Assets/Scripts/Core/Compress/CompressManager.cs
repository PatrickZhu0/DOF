using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDLL.Compress
{
    public class CompressManager : Singleton<CompressManager>, IManager
    {
        public delegate void ProgressCallback(float percent);
        public delegate void DoneCallback(bool success, string message);

        private IGame _igame;
        private Coroutine asyncUpdater = null;
        bool _running = false;

        /// <summary>
        /// 运行的任务，仅允许同时进行一个解压任务，防止多个任务同时写入一个文件，造成写入错误
        /// </summary>
        UnzipTask _runningTask = null;

        /// <summary>
        /// 等待的任务
        /// </summary>
        private Queue<UnzipTask> _taskQueue = new Queue<UnzipTask>();

        public void Dispose()
        {
            if (_runningTask != null)
            {
                _runningTask.Stop();
                _runningTask = null;
            }
            _taskQueue.Clear();
            _running = false;
            if (asyncUpdater != null)
            {
                _igame.StopCoroutine(asyncUpdater);
                asyncUpdater = null;
            }
        }

        public void Init(IGame game)
        {
            _igame = game;
            _running = true;
            asyncUpdater = game.StartCoroutine(ThreadAsyncUpdate());
        }

        public void Reset()
        {
        }


        private void AddTask(UnzipTask task)
        {
            if (_runningTask != null)
                _taskQueue.Enqueue(task);
            else
            {
                _runningTask = task;
                task.Start();
            }
        }

        IEnumerator ThreadAsyncUpdate()
        {
            while (_running)
            {
                if (_runningTask != null)
                {
                    _runningTask.AsyncUpdate();
                    if (!_runningTask.Running)
                    {
                        if (_taskQueue.Count > 0)
                        {
                            _runningTask = _taskQueue.Dequeue();
                            _runningTask.Start();
                        }
                        else
                        {
                            _runningTask = null;
                        }
                    }
                }
                yield return null;
            }
        }

        public void CreateTask(string srcPath, string destDir, ProgressCallback progressCallback, DoneCallback doneCallback)
        {
            UnzipTask task = new UnzipTask(srcPath, destDir, progressCallback, doneCallback);
            AddTask(task);
        }
    }
}
