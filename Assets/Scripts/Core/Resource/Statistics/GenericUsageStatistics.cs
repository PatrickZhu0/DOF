using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDLL.Resource.Statistics
{
    public abstract class AbstractUsageStatistics
    {
        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public AbstractUsageStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// 保护只读变量
        /// </summary>
        protected StatisticsFacade Facade
        {
            get
            {
                return _facade;
            }
        }

        /// <summary>
        /// 实例化对象时需要对现存的引用进行初始计数，子类必须改写
        /// </summary>
        /// <param name="gameObject"></param>
        public abstract void OnGameObjectAwake(GameObject gameObject);

        /// <summary>
        /// 摧毁对象时需要对现存的引用进行初始计数，子类必须改写
        /// </summary>
        /// <param name="gameObject"></param>
        public abstract void OnGameObjectDestroy(GameObject gameObject);
    }


    public abstract class GenericUsageStatistics<T> : AbstractUsageStatistics where T:UnityEngine.Object
    {

        /// <summary>
        /// 所有Object的内部计数统计，其实可以不用
        /// </summary>
        Dictionary<int, int> usageDic = new Dictionary<int, int>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public GenericUsageStatistics(StatisticsFacade facade) : base(facade)
        {
        }

        /// <summary>
        /// 添加引用计数
        /// </summary>
        /// <param name="obj"></param>
        protected void AddRef(T obj)
        {
            int instId = obj.GetHashCode();
            int currentCount = 0;
            if (usageDic.TryGetValue(instId, out currentCount))
            {
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("GenericUsageStatistics.AddRef<{0}>, Sprite count error: count < 0 on AddRef: {1}, {2}", typeof(T).ToString(), obj.name, currentCount);
                }
            }
            int nextCount = currentCount + 1;
            usageDic[instId] = nextCount;
            Facade.LogVerboseFormat("GenericUsageStatistics.AddRef<0>, {1}, id={2}, count {3} -> {4}", typeof(T).ToString(), obj.name, instId, currentCount, nextCount);
            //呼叫真正的计数
            Facade.AssetAddRef(instId);

        }

        /// <summary>
        /// 释放引用
        /// </summary>
        /// <param name="texture"></param>
        protected void ReleaseRef(T obj)
        {
            int instId = obj.GetHashCode();
            if (usageDic.ContainsKey(instId))
            {
                int oldCount = usageDic[instId];
                int currentCount = --usageDic[instId];
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("GenericUsageStatistics<{0}> count error: count < 0 on ReleaseRef: {1}, {2}, instId={3}", typeof(T).ToString(), obj.name, currentCount, instId);
                }
                Facade.LogVerboseFormat("GenericUsageStatistics.ReleaseRef<{0}>, {1}, id={2}, count {3} -> {4}", typeof(T).ToString(), obj.name, instId, oldCount, currentCount);
            }
            else
            {
                GLog.ErrorFormat("GenericUsageStatistics.ReleaseRef<{0}> error: {1} not exist in usageDic, instId={2}", typeof(T).ToString(), obj.name, instId);
            }
            //呼叫真正的计数
            Facade.AssetReleaseRef(instId, true);
        }
    }
}
