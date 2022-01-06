using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDLL
{
    public static class HashSetPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<HashSet<T>> s_HashSetPool = new ObjectPool<HashSet<T>>(null, null, Clear);
        static bool Clear(HashSet<T> l)
        {
            l.Clear();
            return true;
        }

        public static HashSet<T> Get()
        {
            return s_HashSetPool.Get();
        }

        public static void Release(HashSet<T> toRelease)
        {
            s_HashSetPool.Release(toRelease);
        }
    }
}