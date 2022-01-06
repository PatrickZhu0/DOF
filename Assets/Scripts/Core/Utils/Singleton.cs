using System;

namespace GameDLL
{
    /// <summary>
    /// 单例模板
    /// </summary>
    public class Singleton<T> where T : new()
    {
        protected static T instance;
        private static readonly object syslock = new object();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
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

        public static void Destroy()
        {
            if (instance is IDisposable) (instance as IDisposable).Dispose();

            instance = default(T);
        }

    }
}