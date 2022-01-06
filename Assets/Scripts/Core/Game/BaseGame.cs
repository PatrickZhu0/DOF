using System;

namespace GameDLL
{
    public abstract class BaseGame<T> : IGame where T : IGame, new()
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    baseGameInstatnce = instance;
                }
                return instance;
            }
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                if (instance is IDisposable) (instance as IDisposable).Dispose();

                baseGameInstatnce = instance = default(T);

            }
        }
    }
}
