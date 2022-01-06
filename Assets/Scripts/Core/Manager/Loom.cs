using UnityEngine;
using GameDLL;
using System.Collections.Generic;
using System;
using System.Threading;
//using System.Linq;

namespace GameDLL
{
    /// <summary>
    /// Unity使用多线程可以将纯计算与Unity的对象操作分开
    /// </summary>
    public class Loom : MonoSingleton<Loom>
    {
        //private static string objectName = "Loom";
        //private static Loom instance;
        //private static GameObject gameobject;
        //private static readonly object syslock = new object();

        //public static Loom Instance
        //{
        //    get
        //    {
        //        Initialize();
        //        return instance;
        //    }
        //}

        //static void Initialize()
        //{
        //    if (instance == null)
        //    {
        //        lock (syslock)
        //        {
        //            if (gameobject == null)
        //            {
        //                gameobject = GameObject.Find(objectName);
        //                if (gameobject == null)
        //                {
        //                    gameobject = new GameObject(objectName);
        //                    DontDestroyOnLoad(gameobject);      //对象常驻不被清除
        //                }
        //            }
        //            if (instance == null)
        //            {
        //                instance = gameobject.GetComponent<Loom>();
        //                if (instance == null)
        //                    instance = gameobject.AddComponent<Loom>();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 最大线程数
        /// </summary>
        public const int MaxThreads = 8;

        /// <summary>
        /// 当前线程数
        /// </summary>
        private int _numThreads;

        /// <summary>
        /// 将要执行的主线程函数List
        /// </summary>
        private List<Action> _actions = new List<Action>();

        /// <summary>
        /// 实际执行的主线程函数List
        /// </summary>
        private List<Action> _currentActions = new List<Action>();

        public struct DelayedQueueItem
        {
            public long millisecond;
            public Action action;
        }

        /// <summary>
        /// 将要延迟执行的主线程函数List
        /// </summary>
        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        /// <summary>
        /// 实际延迟执行的主线程函数List
        /// </summary>
        private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        /// <summary>
        /// 添加Unity主线程执行函数队列
        /// </summary>
        public void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0);
        }

        /// <summary>
        /// 添加Unity主线程延迟执行函数队列
        /// </summary>
        public void QueueOnMainThread(Action action, long millisecond)
        {
            if (millisecond != 0)
            {
                lock (_delayed)
                {

                    _delayed.Add(new DelayedQueueItem { millisecond = TimeUtils.GetNowTimeStampMS() + millisecond, action = action });
                }
            }
            else
            {
                lock (_actions)
                {
                    _actions.Add(action);
                }
            }
        }

        /// <summary>
        /// 启动线程执行函数，执行或加入队列
        /// </summary>
        public void RunAsync(Action a)
        {
            //Initialize();
            while (_numThreads >= MaxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref _numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
        }

        /// <summary>
        /// ThreadPool.QueueUserWorkItem的WaitCallback方法
        /// </summary>
        private void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref _numThreads);
            }
        }

        //void Awake()
        //{
        //    if (instance == null)
        //    {
        //        lock (syslock)
        //        {
        //            if (gameobject == null)
        //            {
        //                gameobject = gameObject;
        //                DontDestroyOnLoad(gameObject);      //对象常驻不被清除
        //            }
        //            if (instance == null)
        //                instance = this;
        //        }
        //    }
        //    else if (instance != this)
        //    {
        //        GLog.Warning("Duplicate Singleton Loom Instance!");
        //        Destroy(this);
        //    }
        //}

        //void OnEnable()
        //{
        //    instance = this;
        //}

        //void OnDisable()
        //{
        //    if (instance == this)
        //    {
        //        instance = null;
        //    }
        //}

        void Update()
        {
            lock (_actions)
            {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            foreach (Action a in _currentActions)
            {
                a();
            }
            lock (_delayed)
            {
                long now = TimeUtils.GetNowTimeStampMS();
                _currentDelayed.Clear();
                foreach (DelayedQueueItem d in _delayed)
                {
                    if (d.millisecond <= now)
                        _currentDelayed.Add(d);
                }
                //_currentDelayed.AddRange(_delayed.Where(d => d.millisecond <= now));
                foreach (DelayedQueueItem item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (DelayedQueueItem delayed in _currentDelayed)
            {
                delayed.action();
            }
        }
    }
}