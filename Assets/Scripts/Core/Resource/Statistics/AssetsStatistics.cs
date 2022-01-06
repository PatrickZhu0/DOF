using System.Collections.Generic;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 负责统计资源计数
    /// </summary>
    public class AssetsStatistics
    {
        /// <summary>
        /// 记录一个资源的名字，从哪个Assetbundle中来的，以及它的ID
        /// </summary>
        struct AssetInfo
        {
            public string bundleName;
            public string assetName;
            public int assetId;
        }

        /// <summary>
        /// 根据ID查找的资源记录
        /// </summary>
        Dictionary<int, AssetInfo> assetIdToInfo = new Dictionary<int, AssetInfo>();

        /// <summary>
        /// 加载/卸载计数，统计性能用
        /// </summary>
        Dictionary<int, int> assetTotallyReferencedCount = new Dictionary<int, int>();
        Dictionary<int, int> assetTotallyDereferencedCount = new Dictionary<int, int>();

        /// <summary>
        /// 资源是否已经被加载，避免重复释放造成引用计数错误
        /// </summary>
        Dictionary<int, bool> assetLoaded = new Dictionary<int, bool>();

        /// <summary>
        /// 资源的引用计数器
        /// </summary>
        Dictionary<int, int> assetRefCountDic = new Dictionary<int, int>();

        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public AssetsStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// 资源被加载时记录
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="bundleName"></param>
        /// <param name="assetId"></param>
        public void OnAssetLoaded(string assetName, string bundleName, int assetId)
        {
            AssetInfo current;
            if (assetIdToInfo.TryGetValue(assetId, out current))
            {
                //有可能的，在不同地方加载同一个资源
                _facade.LogVerboseFormat("OnAssetLoad:{0} duplicated. because it's already loaded ... current=[[ {1} --- {2} ]], new=[[ {3} --- {4} ]]", assetId, current.bundleName, current.assetName, bundleName, assetName);
            }
            else
            {
                current.bundleName = bundleName;
                current.assetName = assetName;
                current.assetId = assetId;
                assetIdToInfo.Add(assetId, current);
                _facade.LogVerboseFormat("OnAssetLoad: {0} success, [[ {1} --- {2} ]]", assetId, bundleName, assetName);
                _facade.AddAssetUsage(bundleName, assetId);
            }
            assetLoaded[assetId] = true;
        }

        /// <summary>
        /// 资源被卸载时记录
        /// </summary>
        /// <param name="assetId"></param>
        public void OnAssetUnloaded(int assetId)
        {
            AssetInfo current;
            if (!assetIdToInfo.TryGetValue(assetId, out current))
            {
                GLog.ErrorFormat("OnAssetUnloaded:{0} failed. because it's not loaded", assetId);
                return;
            }
            if (assetLoaded[assetId])
            {
                assetLoaded[assetId] = false;

                _facade.LogVerboseFormat("OnAssetUnloaded: {0} success, [[ {1} --- {2} ]]", assetId, current.bundleName, current.assetName);
                //通知Bundle卸载
                _facade.ReleaseAssetUsage(current.bundleName, assetId);
            }
            else
            {
                //已经是上一次通知卸载过了，这里不做任何处理
            }
        }

        /// <summary>
        /// 资源释放已经被加载过
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool IsAssetLoadedManually(int assetId)
        {
            if(assetIdToInfo.ContainsKey(assetId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 测试某个资源是否有引用，目前看来这个接口没有用
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool AssetHasReference(int assetId)
        {
            if(assetRefCountDic.ContainsKey(assetId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 资源被引用时调用
        /// </summary>
        /// <param name="assetId"></param>
        public void AssetAddRef(int assetId)
        {
            int oldCount = 0;
            int count = 0;
            if(assetRefCountDic.ContainsKey(assetId))
            {
                oldCount = assetRefCountDic[assetId];
                count = ++assetRefCountDic[assetId];
            }
            else
            {
                assetRefCountDic[assetId] = 1;
                count = 1;
            }
            _facade.LogVerboseFormat("AssetsStatistics.AssetAddRef id={0}, {1} ---> {2}", assetId, oldCount, count);

            if (assetTotallyReferencedCount.ContainsKey(assetId))
            {
                ++assetTotallyReferencedCount[assetId];
            }
            else
            {
                assetTotallyReferencedCount[assetId] = 1;
            }
        }

        /// <summary>
        /// 资源被释放时调用
        /// </summary>
        /// <param name="assetId"></param>
        public void AssetReleaseRef(int assetId)
        {
            if (assetTotallyDereferencedCount.ContainsKey(assetId))
            {
                ++assetTotallyDereferencedCount[assetId];
            }
            else
            {
                assetTotallyDereferencedCount[assetId] = 1;
            }

            int oldCount = 0;
            int count = 0;
            if (assetRefCountDic.ContainsKey(assetId))
            {
                oldCount = assetRefCountDic[assetId];
                count = --assetRefCountDic[assetId];
            }
            else
            {
                AssetInfo current;
                if (assetIdToInfo.TryGetValue(assetId, out current))
                {
                    GLog.ErrorFormat("AssetReleaseRef: {0} failed, because it's not added yet, [[ {1} --- {2} ]]", assetId, current.bundleName, current.assetName);
                }
                else
                {
                    GLog.ErrorFormat("AssetReleaseRef: {0} failed, because it's not added yet, even not loaded", assetId);
                }
            }
            _facade.LogVerboseFormat("AssetsStatistics.AssetReleaseRef id={0}, {1} ---> {2}", assetId, oldCount, count);
        }

        /// <summary>
        /// 尝试是否所有引用计数为0的资源
        /// </summary>
        public void DisposeUnusedAssets()
        {
            foreach(var kv in assetRefCountDic)
            {
                if (kv.Value <= 0)
                {
                    OnAssetUnloaded(kv.Key);
                }
            }
        }

        /// <summary>
        /// 测试某资源是否被加载过
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool IsAssetLoaded(int assetId)
        {
            bool output = false;
            if(assetLoaded.TryGetValue(assetId, out output))
            {
                return output;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查找某个资源的记录
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool FindAssetInfo(int assetId, out string bundleName, out string assetName)
        {
            AssetInfo target;
            if (assetIdToInfo.TryGetValue(assetId, out target))
            {
                bundleName = target.bundleName;
                assetName = target.assetName;
                return true;
            }
            else
            {
                bundleName = string.Empty;
                assetName = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// 打印统计数据
        /// </summary>
        /// <param name="writer"></param>
        public void Dump(System.IO.StreamWriter writer)
        {
            writer.WriteLine(string.Empty);
            writer.WriteLine("----------------------------------------------------------");
            writer.WriteLine("---------------AssetsStatistics Begin---------------------");
            writer.WriteLine(string.Empty);
            writer.WriteLine(string.Empty);

            foreach (var kv in assetIdToInfo)
            {
                writer.WriteLine(string.Format("Begin of asset: {0}", kv.Key));

                writer.WriteLine(string.Format("\tassetName: {0}\tbundleName: {1}", kv.Value.assetName, kv.Value.bundleName));

                int refCount = 0;
                if(assetRefCountDic.TryGetValue(kv.Key, out refCount))
                {
                    writer.WriteLine(string.Format("\trefCount: {0}", refCount));
                }
                else
                {
                    writer.WriteLine("\tno reference");
                }

                int loadCount = 0, unloadCount = 0;
                bool foundInLoad = false, foundInUnload = false;
                if(assetTotallyReferencedCount.TryGetValue(kv.Key, out loadCount))
                {
                    foundInLoad = true;
                }
                if(assetTotallyDereferencedCount.TryGetValue(kv.Key, out unloadCount))
                {
                    foundInUnload = true;
                }
                writer.WriteLine(string.Format("\ttotally referenced: {0}\ttotally unreferenced: {1}", foundInLoad?loadCount:-1, foundInUnload?unloadCount:-1));

                writer.WriteLine(string.Format("End of asset: {0}", kv.Key));

                writer.WriteLine(string.Empty);
            }

            writer.WriteLine(string.Empty);
            writer.WriteLine(string.Empty);
            writer.WriteLine("----------------AssetsStatistics End----------------------");
            writer.WriteLine("----------------------------------------------------------");
            writer.WriteLine(string.Empty);
        }
    }
}
