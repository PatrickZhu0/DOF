using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDLL
{
    public static class DictionaryPool<K,V>
    {
        private static readonly ObjectPool<Dictionary<K,V>> s_DicPool = new ObjectPool<Dictionary<K, V>>(null, null, Clear);
        static bool Clear(Dictionary<K, V> d)
        {
            d.Clear();
            return true;
        }

        public static Dictionary<K, V> Get()
        {
            return s_DicPool.Get();
        }

        public static void Release(Dictionary<K, V> toRelease)
        {
            s_DicPool.Release(toRelease);
        }
    }
}
