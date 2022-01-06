namespace GameDLL
{
    public class ObjectsPool<T> : Singleton<ObjectsPool<T>> where T : new()
    {
        protected virtual int DefaultCount
        {
            get
            {
                return 32;
            }
        }

        private ObjectPool<T> _objectsPool;

        public ObjectsPool()
        {
            _objectsPool = new ObjectPool<T>(DefaultCount, OnCreate, OnGet, OnRelease);
        }

        protected virtual void OnCreate(T t)
        {
        }

        protected virtual bool OnGet(T t)
        {
            return true;
        }

        protected virtual bool OnRelease(T t)
        {
            return true;
        }

        public static T Get()
        {
            return Instance._objectsPool.Get();
        }

        public static void Release(T toRelease)
        {
            if (toRelease == null || Instance._objectsPool.Contains(toRelease))
                return;
            Instance._objectsPool.Release(toRelease);
        }
    }
}
