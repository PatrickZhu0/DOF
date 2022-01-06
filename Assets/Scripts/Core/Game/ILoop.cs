using System.Collections;
using UnityEngine;

namespace GameDLL
{
    /// <summary>
    /// 游戏主循环
    /// </summary>
    public abstract class ILoop : MonoBehaviour
    {
        private static string objectName = "Loop";
        internal static ILoop instance;

        internal protected IGame game;

        public static ILoop Instance
        {
            get
            {
                return instance;
            }
        }

        public static T Create<T, U>(IGame game) where T : ILoop where U : ILateLoop
        {
            if (instance == null && game != null)
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
                if (game.gameRoot)
                    gameObject.transform.SetParent(game.gameRoot);
                else
                    DontDestroyOnLoad(gameObject);
                instance.game = game;
                ILateLoop.Create<U>(instance);
            }
            return instance as T;
        }

        public static Coroutine CreateCoroutine(IEnumerator routine)
        {
            //by Rick, 添加了返回值，便于中断协程
            if (instance != null)
            {
                return instance.StartCoroutine(routine);
            }
            else
            {
                GLog.ErrorFormat("CreateCoroutine failed, ILoop has no instance.");
                return null;
            }
        }

        public static void DestroyCoroutine(Coroutine coroutine)
        {
            if (instance != null)
            {
                instance.StopCoroutine(coroutine);
            }
            else
            {
                GLog.ErrorFormat("InterruptCoroutine failed, ILoop has no instance.");
            }
        }

        void Awake()
        {
            if (instance && instance != this)
                GameObject.Destroy(this);
        }

        void Start()
        {
            game.Start();
        }

        void Update()
        {
            game.Update();
        }

        void LateUpdate()
        {
            game.LateUpdate();
        }

        void FixedUpdate()
        {
            game.FixedUpdate();
        }

        //void OnDestroy()
        //{
        //    game.OnDestroy();
        //}

        //void OnApplicationQuit()
        //{
        //    game.OnApplicationQuit();
        //}

    }
}