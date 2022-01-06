using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GameDLL
{
    /// <summary>
    /// Unity字典类数据
    /// </summary>
    /// <typeparam name="K">键</typeparam>
    /// <typeparam name="V">值</typeparam>
    public class KVData<K, V> : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        protected List<K> keys = new List<K>();
        [SerializeField]
        protected List<V> values = new List<V>();

        protected Dictionary<K, V> dict = new Dictionary<K, V>();

        public virtual void OnBeforeSerialize()
        {

        }

        public virtual void OnAfterDeserialize()
        {
            dict.Clear();
            int length = Mathf.Min(keys.Count, values.Count);
            for (int i = 0; i < length; i++)
                dict[keys[i]] = values[i];
        }

        public V this[K key]
        {
            get
            {
                V v;
                dict.TryGetValue(key, out v);
                return v;
            }
        }

        public K[] Keys
        {
            get
            {
                return keys.ToArray();
            }
        }

        public V[] Values
        {
            get
            {
                return values.ToArray();
            }
        }

        public int Length
        {
            get
            {
                return keys.Count;
            }
        }

        [Conditional("UNITY_EDITOR")]
        public void SetValue(K key, V value)
        {
            V v;
            if (dict.TryGetValue(key, out v))
            {
                dict[key] = value;
                int index = keys.IndexOf(key);
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
                dict.Add(key, value);
            }
        }
    }
}