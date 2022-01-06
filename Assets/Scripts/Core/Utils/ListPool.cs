using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDLL
{
    public static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, null, Clear);
        static bool Clear(List<T> l)
        {
            l.Clear();
            return true;
        }

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }
}