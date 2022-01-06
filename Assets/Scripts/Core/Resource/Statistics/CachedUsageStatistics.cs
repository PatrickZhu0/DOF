using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 保持缓存的资源计数器
    /// </summary>
    public class CachedUsageStatistics
    {
        /// <summary>
        /// 承担计数的组，资源在组一级登记一次引用+1，组释放时对其所有资源引用-1
        /// </summary>
        class Group
        {
            readonly CachedUsageStatistics _owner;
            readonly string _uniqueName;
            HashSet<int> holdingAssets = new HashSet<int>();

            public Group(CachedUsageStatistics owner, string uniqueName)
            {
                _owner = owner;
                _uniqueName = uniqueName;
            }

            public void AddUsage(int assetId)
            {
                if(holdingAssets.Contains(assetId))
                {
                    //已经在组里，不做任何操作
                }
                else
                {
                    //新资源，使引用计数加+1
                    _owner.AddRef(assetId);
                    holdingAssets.Add(assetId);
                }
            }

            public void ReleaseAllAssets()
            {
                foreach(int assetId in holdingAssets)
                {
                    _owner.ReleaseRef(assetId);
                }
            }
        }

        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// 所有缓存组的查找表
        /// </summary>
        Dictionary<string, Group> groupDic = new Dictionary<string, Group>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public CachedUsageStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// 查找分组
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        Group FindGroup(string uniqueName)
        {
            Group output = null;
            if(groupDic.TryGetValue(uniqueName, out output))
            {
                return output;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 创建一个新组
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string CreateGroup(Type assetType, string groupName)
        {
            string timeStamp = DateTime.Now.ToString();
            string uniqueName = string.Format("CGUID_{0}_{1}_{2}", assetType.ToString(), groupName, timeStamp);
            groupDic.Add(uniqueName, new Group(this, uniqueName));
            _facade.LogVerboseFormat("CachedUsageStatistics.CreateGroup success, groupUniqueName={0}", uniqueName);
            return uniqueName;
        }

        /// <summary>
        /// 给某个分组添加一个引用
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="groupUniqueName"></param>
        public void AddUsageToGroup(int assetId, string groupUniqueName)
        {
            Group group = FindGroup(groupUniqueName);
            _facade.Assert(group != null, "CachedUsageStatistics.CachedUsageStatistics.AddToGroup failed, can not find group named: {0}", groupUniqueName);
            group.AddUsage(assetId);
            _facade.LogVerboseFormat("CachedUsageStatistics.AddUsageToGroup success, groupUniqueName={0}", groupUniqueName);
        }

        /// <summary>
        /// 释放某个缓存分组
        /// </summary>
        /// <param name="groupUniqueName"></param>
        public void ReleaseGroup(string groupUniqueName)
        {
            Group group = FindGroup(groupUniqueName);
            _facade.Assert(group != null, "CachedUsageStatistics.CachedUsageStatistics.ReleaseGroup failed, can not find group named: {0}", groupUniqueName);
            group.ReleaseAllAssets();
            groupDic.Remove(groupUniqueName);
            _facade.LogVerboseFormat("CachedUsageStatistics.ReleaseGroup success, groupUniqueName={0}", groupUniqueName);
        }

        /// <summary>
        /// 被分组调用的代理函数，对分组隐藏门面接口
        /// </summary>
        /// <param name="assetId"></param>
        public void AddRef(int assetId)
        {
            _facade.AssetAddRef(assetId);
        }

        /// <summary>
        /// 被分组调用的代理函数，对分组隐藏门面接口
        /// </summary>
        /// <param name="assetId"></param>
        public void ReleaseRef(int assetId)
        {
            _facade.AssetReleaseRef(assetId, false);
        }
    }
}
