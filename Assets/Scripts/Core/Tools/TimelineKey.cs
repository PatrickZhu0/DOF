using System;
using UnityEngine.Timeline;

namespace GameDLL.Timeline
{
    [Serializable]
    public struct TimelineKey : IComparable
    {
        public float time;
        public int key;
        /// <summary>
        /// 用于type不同枚举的设置
        /// </summary>
        public int mode;
        public int type;
        public TimelineAsset asset;
        public string param;

        public int CompareTo(object obj)
        {
            TimelineKey k = (TimelineKey)obj;
            if (time > k.time)
                return 1;
            if (time < k.time)
                return -1;
            return 0;
        }
    }
}
