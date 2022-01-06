using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 负责统计AB计数
    /// </summary>
    public class BundleStatistics
    {
        /// <summary>
        /// 单个AB的使用计数
        /// </summary>
        class BundleUsage
        {
            readonly string _bundleName;
            readonly Dictionary<int, int> usedByAsset;
            readonly Dictionary<string, int> usedByScene;

            public BundleUsage(string bundleName)
            {
                _bundleName = bundleName;
                usedByAsset = new Dictionary<int, int>();
                usedByScene = new Dictionary<string, int>();
            }

            public void AddAssetUsage(int assetId)
            {
                if (usedByAsset.ContainsKey(assetId))
                {
                    ++usedByAsset[assetId];
                }
                else
                {
                    usedByAsset[assetId] = 1;
                }
            }

            public void ReleaseAssetUsage(int assetId)
            {
                if (usedByAsset.ContainsKey(assetId))
                {
                    --usedByAsset[assetId];
                    
                }
                else
                {
                    GLog.ErrorFormat("ReleaseAssetUsage {0} by {1} failed, never added", _bundleName, assetId);
                }
            }

            public void AddSceneUsage(string sceneName)
            {
                if (usedByScene.ContainsKey(sceneName))
                {
                    ++usedByScene[sceneName];
                }
                else
                {
                    usedByScene[sceneName] = 1;
                }
            }

            public void ReleaseSceneUsage(string sceneName)
            {
                if (usedByScene.ContainsKey(sceneName))
                {
                    --usedByScene[sceneName];
                }
                else
                {
                    GLog.ErrorFormat("ReleaseSceneUsage {0} by {1} failed, never added", _bundleName, sceneName);
                }
            }

            public bool IsUnused()
            {
                bool inUse = false;
                foreach (int count in usedByAsset.Values)
                {
                    if (count > 0)
                    {
                        inUse = true;
                        break;
                    }
                }
                foreach (int count in usedByScene.Values)
                {
                    if (count > 0)
                    {
                        inUse = true;
                        break;
                    }
                }
                return !inUse;
            }

            public List<KeyValuePair<int, int>> GetAllUsingAssets()
            {
                List<KeyValuePair<int, int>> output = new List<KeyValuePair<int, int>>();
                foreach (var kv in usedByAsset)
                {
                    if (kv.Value > 0)
                    {
                        output.Add(kv);
                    }
                }

                return output;
            }

            public List<KeyValuePair<string, int>> GetAllUsingScenes()
            {
                List<KeyValuePair<string, int>> output = new List<KeyValuePair<string, int>>();
                foreach(var kv in usedByScene)
                {
                    if(kv.Value > 0)
                    {
                        output.Add(kv);
                    }
                }

                return output;
            }
        }

        /// <summary>
        /// 所有AB的使用计数
        /// </summary>
        Dictionary<string, BundleUsage> bundleUsageDic = new Dictionary<string, BundleUsage>();

        /// <summary>
        /// 从Asset到AB的反向查找表，AB从属于哪个
        /// </summary>
        Dictionary<string, HashSet<string>> assetToBundle = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// 记录所有AB是否被加载了
        /// </summary>
        Dictionary<string, bool> bundleLoaded = new Dictionary<string, bool>();

        /// <summary>
        /// 记录AB被加载的次数
        /// </summary>
        Dictionary<string, int> bundleLoadedCount = new Dictionary<string, int>();

        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// AssetBundle清单
        /// </summary>
        private AssetBundleManifest _manifest = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public BundleStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        public void AssignManifest(AssetBundleManifest manifest)
        {
            _manifest = manifest;
        }


        /// <summary>
        /// 内部函数，申请一个计数器
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        private BundleUsage QueryBundleUsage(string bundleName)
        {
            BundleUsage o = null;
            if (bundleUsageDic.TryGetValue(bundleName, out o))
            {
                return o;
            }
            o = new BundleUsage(bundleName);
            bundleUsageDic.Add(bundleName, o);
            return o;
        }

        /// <summary>
        /// 增加一个资源引用
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetId"></param>
        public void AddAssetUsage(string bundleName, int assetId)
        {
            var abname = bundleName.ToLower();
            string bundleNameWithHashCode = abname;
            if (_manifest != null)
            {
                bundleNameWithHashCode = _facade.FindAssetBundleNameInHashCodeTable(abname);
                var depends = _manifest.GetAllDependencies(bundleNameWithHashCode);
                foreach (var d in depends)
                {
                    if (d.Equals(bundleNameWithHashCode))
                    {
                        _facade.LogVerboseFormat("AddAssetUsage, duplicated dependence ab with main {0}", bundleNameWithHashCode);
                    }
                    else
                    {
                        BundleUsage dependingUsage = QueryBundleUsage(d);
                        dependingUsage.AddAssetUsage(assetId);
                        _facade.LogVerboseFormat("AddAssetUsage assetId={0}, bundleName={1}, dependedby={2}", assetId, d, bundleNameWithHashCode);
                    }
                }
            }
            BundleUsage usage = QueryBundleUsage(bundleNameWithHashCode);
            usage.AddAssetUsage(assetId);
            _facade.LogVerboseFormat("AddAssetUsage assetId={0}, bundleName={1} directly", assetId, bundleNameWithHashCode);
        }


        /// <summary>
        /// 释放一个资源引用
        /// </summary>
        /// <param name="assetId"></param>
        public void ReleaseAssetUsage(string bundleName, int assetId)
        {
            var abname = bundleName.ToLower();
            string bundleNameWithHashCode = _facade.FindAssetBundleNameInHashCodeTable(abname);
            BundleUsage usage = QueryBundleUsage(bundleNameWithHashCode);
            usage.ReleaseAssetUsage(assetId);
            _facade.LogVerboseFormat("ReleaseAssetUsage assetId={0}, bundleName={1} directly", assetId, bundleNameWithHashCode);
            if (_manifest != null)
            {
                var depends = _manifest.GetAllDependencies(bundleName);
                foreach (var d in depends)
                {
                    if (d.Equals(bundleNameWithHashCode))
                    {
                        _facade.LogVerboseFormat("ReleaseAssetUsage, duplicated dependence ab with main {0}", bundleNameWithHashCode);
                    }
                    else
                    {
                        BundleUsage dependingUsage = QueryBundleUsage(d);
                        dependingUsage.ReleaseAssetUsage(assetId);
                        _facade.LogVerboseFormat("ReleaseAssetUsage assetId={0}, bundleName={1} directly", assetId, d);
                    }
                }
            }
        }

        /// <summary>
        /// 添加一个场景引用
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        public void AddSceneUsage(string bundleName, string sceneName)
        {
            var abname = bundleName.ToLower();
            string bundleNameWithHashCode = _facade.FindAssetBundleNameInHashCodeTable(abname);
            if (_manifest != null)
            {
                var depends = _manifest.GetAllDependencies(bundleNameWithHashCode);
                foreach (var d in depends)
                {
                    if (d.Equals(bundleNameWithHashCode))
                    {
                        _facade.LogVerboseFormat("AddSceneUsage, duplicated dependence ab with main {0}", bundleNameWithHashCode);
                    }
                    else
                    {
                        BundleUsage dependingUsage = QueryBundleUsage(d);
                        dependingUsage.AddSceneUsage(sceneName);
                        _facade.LogVerboseFormat("AddAssetUsage sceneName={0}, bundleName={1}, dependedby={2}", sceneName, d, bundleNameWithHashCode);
                    }
                }
            }
            BundleUsage usage = QueryBundleUsage(bundleNameWithHashCode);
            usage.AddSceneUsage(sceneName);
            _facade.LogVerboseFormat("AddSceneUsage, sceneName={0}, bundleName={1} directly", sceneName, bundleNameWithHashCode);
        }

        /// <summary>
        /// 释放一个场景引用
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        public void ReleaseSceneUsage(string bundleName, string sceneName)
        {
            var abname = bundleName.ToLower();
            string bundleNameWithHashCode = _facade.FindAssetBundleNameInHashCodeTable(abname);
            BundleUsage usage = QueryBundleUsage(bundleNameWithHashCode);
            usage.ReleaseSceneUsage(sceneName);
            _facade.LogVerboseFormat("ReleaseAssetUsage sceneName={0}, bundleName={1} directly", sceneName, bundleNameWithHashCode);
            if (_manifest != null)
            {
                var depends = _manifest.GetAllDependencies(bundleNameWithHashCode);
                foreach (var d in depends)
                {
                    if (d.Equals(bundleNameWithHashCode))
                    {
                        _facade.LogVerboseFormat("ReleaseSceneUsage, duplicated dependence ab with main {0}", bundleNameWithHashCode);
                    }
                    else
                    {
                        BundleUsage dependingUsage = QueryBundleUsage(d);
                        dependingUsage.ReleaseSceneUsage(sceneName);
                        _facade.LogVerboseFormat("ReleaseSceneUsage sceneName={0}, bundleName={1}, dependedby={2}", sceneName, d, bundleNameWithHashCode);
                    }
                }
            }
        }

        /// <summary>
        /// 尝试释放所有不再使用的AB
        /// </summary>
        public void DisposeUnusedBundles()
        {
            int total = bundleUsageDic.Count;
            int unloaded = 0;
            foreach (var kv in bundleUsageDic)
            {
                var usage = kv.Value;
                if (usage.IsUnused())
                {
                    bool isLoaded = false;
                    //string bundleNameWithHashCode = _facade.FindAssetBundleNameInHashCodeTable(kv.Key);
                    string bundleNameWithHashCode = kv.Key; //现在添加进来的时候已经是带Hash码的了，不用再查找了
                    if (bundleLoaded.TryGetValue(bundleNameWithHashCode, out isLoaded))
                    {
                        if (isLoaded)
                        {
                            _facade.DisposeUnusedBundle(bundleNameWithHashCode);
                            ++unloaded;
                        }
                        else
                        {
                            //已经被卸载的
                            _facade.LogVerboseFormat("DisposeUnusedBundles skip {0}, it's already unloaded last time.", bundleNameWithHashCode);
                        }
                    }
                    else
                    {
                        GLog.ErrorFormat("DisposeUnusedBundles failed, try unloading {0} when it's not registered in bundleLoaded", kv.Key);
                    }
                }
                else if (StatisticsFacade.LogVerboseMode)
                {
                    var usingAssets = usage.GetAllUsingAssets();
                    _facade.LogVerboseFormat("DisposeUnusedBundles begin using assets of ab: {0}", kv.Key);
                    string usingBundleNameByAsset = string.Empty;
                    string usingAssetName = string.Empty;
                    foreach (var kvUsing in usingAssets)
                    {
                        if (_facade.FindAssetInfo(kvUsing.Key, out usingBundleNameByAsset, out usingAssetName))
                        {
                            _facade.LogVerboseFormat("DisposeUnusedBundles bundle:{4} used by assets info: id={0}, count={1}, usingAssetName={2}, usingBundleNameByAsset={3}", kvUsing.Key, kvUsing.Value, usingAssetName, usingBundleNameByAsset, kv.Key);
                        }
                        else
                        {
                            _facade.LogVerboseFormat("DisposeUnusedBundles cannot find using assets info: {0}", kvUsing.Key);
                        }
                    }
                    _facade.LogVerboseFormat("DisposeUnusedBundles end using assets of ab: {0}", kv.Key);

                    var usingScene = usage.GetAllUsingScenes();
                    _facade.LogVerboseFormat("DisposeUnusedBundles begin using scenes of ab: {0}", kv.Key);
                    string usingBundleNameByScene = string.Empty;
                    string usingSceneName = string.Empty;
                    foreach (var kvUsing in usingScene)
                    {
                        _facade.LogVerboseFormat("DisposeUnusedBundles bundle:{2} used by scenes info: scene={0}, count={1}", kvUsing.Key, kvUsing.Value, kv.Key);
                    }
                    _facade.LogVerboseFormat("DisposeUnusedBundles end using scenes of ab: {0}", kv.Key);
                }
            }
            _facade.LogVerboseFormat("DisposeUnusedBundles, unloaded {0} of {1}", unloaded, total);
        }

        /// <summary>
        /// 监视assetbundle加载完毕
        /// </summary>
        /// <param name="bundleName"></param>
        public void OnAssetbundleLoaded(string bundleName)
        {
            if(StatisticsFacade.LogVerboseMode)
            {
                if (bundleLoadedCount.ContainsKey(bundleName))
                {
                    bundleLoadedCount[bundleName]++;
                }
                else
                {
                    bundleLoadedCount[bundleName] = 1;
                }
            }
            bundleLoaded[bundleName] = true;
            _facade.LogVerboseFormat("MonitorAssetbundle +++ OnAssetbundleLoaded {0}", bundleName);
        }

        /// <summary>
        /// 监视assetbundle卸载完毕
        /// </summary>
        /// <param name="bundleName"></param>
        public void OnAssetbundleUnloaded(string bundleName)
        {
            bundleLoaded[bundleName] = false;
            _facade.LogVerboseFormat("MonitorAssetbundle --- OnAssetbundleUnloaded {0}", bundleName);
        }

        /// <summary>
        /// 打印统计数据
        /// </summary>
        /// <param name="writer"></param>
        public void Dump(System.IO.StreamWriter writer)
        {
            writer.WriteLine(string.Empty);
            writer.WriteLine("----------------------------------------------------------");
            writer.WriteLine("---------------BundleStatistics Begin---------------------");
            writer.WriteLine(string.Empty);
            writer.WriteLine(string.Empty);

            foreach (var kv in bundleUsageDic)
            {
                var usage = kv.Value;
                writer.WriteLine(string.Format("Begin of bundle: {0}", kv.Key));
                if(StatisticsFacade.LogVerboseMode)
                {
                    int loadedCount = 0;
                    if (!bundleLoadedCount.TryGetValue(kv.Key, out loadedCount))
                    {
                        writer.WriteLine("Load Count Error");
                    }
                    else
                    {
                        writer.WriteLine(string.Format("Load Count: {0}", loadedCount));
                    }
                }
                if (usage.IsUnused())
                {
                    bool isLoaded = false;
                    if (bundleLoaded.TryGetValue(kv.Key, out isLoaded))
                    {
                        if (isLoaded)
                        {
                            writer.WriteLine("\tWill be Unloaded");
                        }
                        else
                        {
                            //已经被卸载的
                            writer.WriteLine("\tAlready been unloaded");
                        }
                    }
                    else
                    {
                        writer.WriteLine("\tERROR!! Dump failed, it's not registered in bundleLoaded");
                    }
                }
                else
                {
                    var usingAssets = usage.GetAllUsingAssets();
                    string usingBundleNameByAsset = string.Empty;
                    string usingAssetName = string.Empty;
                    foreach (var kvUsing in usingAssets)
                    {
                        if (_facade.FindAssetInfo(kvUsing.Key, out usingBundleNameByAsset, out usingAssetName))
                        {
                            writer.WriteLine(string.Format("\tused by assets info: id={0}, count={1}, usingAssetName={2}, usingBundleNameByAsset={3}", kvUsing.Key, kvUsing.Value, usingAssetName, usingBundleNameByAsset));
                        }
                        else
                        {
                            writer.WriteLine("\tERROR!! Dump failed, cannot find using assets info");
                        }
                    }

                    var usingScenes = usage.GetAllUsingScenes();
                    foreach(var kvUsing in usingScenes)
                    {
                        writer.WriteLine(string.Format("\tused by assets info: scene={0}, count={1}", kvUsing.Key, kvUsing.Value));
                    }
                }

                writer.WriteLine(string.Format("End of bundle: {0}", kv.Key));

                writer.WriteLine(string.Empty);
            }

            writer.WriteLine(string.Empty);
            writer.WriteLine(string.Empty);
            writer.WriteLine("----------------BundleStatistics End----------------------");
            writer.WriteLine("----------------------------------------------------------");
            writer.WriteLine(string.Empty);
        }
    }
}
