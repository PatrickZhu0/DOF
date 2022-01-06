using System;
using UnityEngine;

namespace GameDLL
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

        protected static T instance = null;

        public static T Instance
        {
            get
            {
                Initialize();
                return instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return instance != null;
            }
        }

        protected static void Initialize()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                    instance.InitInstance();
                }
            }
        }

        protected virtual void InitInstance()
        {
            Transform root = null;
            if (IGame.GetInstance() != null)
                root = IGame.GetInstance().gameRoot;
            if (root)
                transform.SetParent(root);
            else
                DontDestroyOnLoad(gameObject);      //对象常驻不被清除
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else if (instance != this)
            {
                GLog.WarningFormat("Duplicate MonoSingleton {0} Instance!", typeof(T).Name);
                Destroy(this);
            }
        }

        protected virtual void Init()
        {

        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }

        public static void Destroy()
        {
            if (instance)
            {
                if (instance is IDisposable) (instance as IDisposable).Dispose();
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}
