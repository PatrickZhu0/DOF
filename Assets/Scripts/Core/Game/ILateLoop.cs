using System;
using UnityEngine;

namespace GameDLL
{
    public abstract class ILateLoop : MonoBehaviour
    {
        protected IGame game;

        private static string objectName = "LateLoop";
        internal static ILateLoop instance;

        public static ILateLoop Instance
        {
            get
            {
                return instance;
            }
        }

        internal static T Create<T>(ILoop loop) where T : ILateLoop
        {
            if (instance == null && loop != null)
            {
                GameObject gameObject = null;
                instance = FindObjectOfType<T>();
                if (instance)
                    gameObject = instance.gameObject;
                else
                {
                    gameObject = new GameObject(objectName);
                    instance = gameObject.AddComponent<T>();
                }
                if(loop.game.gameRoot)
                    gameObject.transform.SetParent(loop.game.gameRoot);
                else
                    DontDestroyOnLoad(gameObject);
                instance.game = loop.game;
            }
            return instance as T;
        }

        void Awake()
        {
            if (instance && instance != this)
                GameObject.Destroy(this);
        }

        void Update()
        {
            if (HandleEscape())
                return;
        }

        void OnDestroy()
        {
            if (game != null)
                game.OnDestroy();
        }

        void OnApplicationQuit()
        {
            game.OnApplicationQuit();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        protected abstract bool HandleEscape();
    }
}
