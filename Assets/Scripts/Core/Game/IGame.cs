using GameDLL.WebRequest;
using System;
using System.Collections;
using UnityEngine;

namespace GameDLL
{
    public abstract class IGame
    {
        /// <summary>
        /// 挂载在Unity场景主循环
        /// </summary>
        protected ILoop mainLoop;

        /// <summary>
        /// 最后执行的主循环
        /// </summary>
        protected ILateLoop lateLoop;

        /// <summary>
        /// 挂载管理脚本的根节点
        /// </summary>
        public Transform gameRoot { get; protected set; }

        protected bool applicationRunning;
        public virtual bool IsApplicationRunning
        {
            get
            {
                return applicationRunning;
            }
        }

        protected Action fixedUpdateAction;
        protected Action updateAction;
        protected Action lateUpdateAction;

        /// <summary>
        /// 启动时备份下来的mono
        /// </summary>
        public MonoBehaviour mono
        {
            get; protected set;
        }

        protected float tScale = 1;

        public virtual float timeScale
        {
            get
            {
                return tScale;
            }
            set
            {
                if (tScale != value)
                {
                    tScale = value;
                    Time.timeScale = tScale;
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IGame()
        {
            tScale = 1;
            //by Rick 标记进程开始
            GameTools.ApplicationRunning(true);
            applicationRunning = true;
            //~by Rick
        }

        public virtual void Init(MonoBehaviour mono)
        {
            this.mono = mono;
            InitRoot(mono);
            InitManager(mono);
        }

        protected virtual void InitRoot(MonoBehaviour mono)
        {
            GameObject rootObj;
            if (mono)
            {
                rootObj = mono.gameObject;
                gameRoot = mono.transform.Find(Const.GameBaseNodeName);
                if (!gameRoot)
                {
                    GameObject go = new GameObject(Const.GameBaseNodeName);
                    go.SetParent(mono.transform);
                    this.gameRoot = go.transform;
                }
            }
            else
            {
                rootObj = new GameObject("Game");
                GameObject go = new GameObject(Const.GameBaseNodeName);
                go.SetParent(rootObj.transform);
                gameRoot = go.transform;
            }
            if (Application.isPlaying)
                GameObject.DontDestroyOnLoad(rootObj);
        }

        /// <summary>
        /// 重置游戏参数
        /// </summary>
        public virtual void OnReset()
        {
            fixedUpdateAction = null;
            updateAction = null;
            lateUpdateAction = null;
        }

        public virtual void ExitGame(bool forceQuit)
        {
            Application.Quit();
        }

        #region Unity MonoBehaviour调用函数
        public virtual void Start()
        {
            //do nothing
        }

        public void AddUpdate(Action action)
        {
            if (action == null)
                return;
            if (updateAction != null)
                updateAction -= action;
            updateAction += action;
        }

        public void RemoveUpdate(Action action)
        {
            if (action == null)
                return;
            if (updateAction != null)
                updateAction -= action;
        }

        public virtual void Update()
        {
            if (updateAction != null)
                updateAction();
        }

        public void AddLateUpdate(Action action)
        {
            if (action == null)
                return;
            if (lateUpdateAction != null)
                lateUpdateAction -= action;
            lateUpdateAction += action;
        }

        public void RemoveLateUpdate(Action action)
        {
            if (action == null)
                return;
            if (lateUpdateAction != null)
                lateUpdateAction -= action;
        }

        public virtual void LateUpdate()
        {
            if (lateUpdateAction != null)
                lateUpdateAction();
        }

        public void AddFixedUpdate(Action action)
        {
            if (action == null)
                return;
            if (fixedUpdateAction != null)
                fixedUpdateAction -= action;
            fixedUpdateAction += action;
        }

        public void RemoveFixedUpdate(Action action)
        {
            if (action == null)
                return;
            if (fixedUpdateAction != null)
                fixedUpdateAction -= action;
        }

        /// <summary>
        /// 定时帧主循环
        /// </summary>
        public virtual void FixedUpdate()
        {
            if (fixedUpdateAction != null)
                fixedUpdateAction();
        }

        /// <summary>
        /// 初始化各种管理器
        /// </summary>
        protected abstract void InitManager(MonoBehaviour mono);

        /// <summary>
        /// 清除对象
        /// </summary>
        public abstract void OnDestroy();

        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        /// <returns></returns>
        public abstract bool IsInitiated();

        /// <summary>
        /// 退出应用
        /// </summary>
        public virtual void OnApplicationQuit()
        {
            //by Rick 标记进程退出
            applicationRunning = false;
            GameTools.ApplicationRunning(false);
            //~by Rick
        }
        #endregion

        /// <summary>
        /// 启动协程
        /// </summary>
        public virtual Coroutine StartCoroutine(IEnumerator coroutine)
        {
            if (mainLoop)
                return mainLoop.StartCoroutine(coroutine);
            return null;
        }

        /// <summary>
        /// 启动协程
        /// </summary>
        public virtual Coroutine StartLateCoroutine(IEnumerator coroutine)
        {
            if (lateLoop)
                return lateLoop.StartCoroutine(coroutine);
            return null;
        }

        public virtual bool StopCoroutine(Coroutine coroutine)
        {
            if(mainLoop)
            {
                mainLoop.StopCoroutine(coroutine);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual ILoop GetMainLoop()
        {
            return mainLoop;
        }

        public virtual ILateLoop GetLateLoop()
        {
            return lateLoop;
        }

        protected static IGame baseGameInstatnce;
        public static IGame GetInstance()
        {
            return baseGameInstatnce;
        }
    }
}
