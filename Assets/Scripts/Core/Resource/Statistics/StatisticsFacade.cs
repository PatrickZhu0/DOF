using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// by Rick 统计门面类，避免各个统计接口过于耦合
    /// </summary>
    public class StatisticsFacade
    {
        AssetsStatistics _assetsStatistics = null;
        BundleStatistics _bundleStatistics = null;
        ResourceManager _resourceManager = null;
        SpriteUsageStatistics _spriteUsageStatistics = null;
        TextureUsageStatistics _textureUsageStatistics = null;
        CachedUsageStatistics _cachedUsageStatistics = null;
        AnimationClipUsageStatistics _animationClipUsageStatistics = null;
        HashSet<AbstractUsageStatistics> _abstractUsageStatistics = new HashSet<AbstractUsageStatistics>();
        string builtInCacheGroupId = null;
        const string UNPARENT_ROOT_NAME = "UnparentRoot";
        Transform _unparentRootTransform = null;
        Action _luaTraceStackAction = null;

        static bool logVerboseMode = false;
        static public bool LogVerboseMode
        {
            get { return logVerboseMode; }
            set { logVerboseMode = value; }
        }

        bool releaseMode = false;
        Coroutine waitingDisposeCoroutine = null;

        /// <summary> by Rick
        /// 记录些冗长的资源日志，开发时使用，后期需要关闭甚至删除以保证性能
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void LogVerboseFormat(string message, params object[] args)
        {
            if (logVerboseMode)
            {
                GLog.LogFormat(message, args);
            }
        }

        public void LuaTraceStackAction()
        {
            if(_luaTraceStackAction!=null)
            {
                _luaTraceStackAction();
            }
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Assert(bool condition, string message, params object[] args)
        {
            UnityEngine.Debug.Assert(condition);
            if (!condition)
            {
                GLog.ErrorFormat(message, args);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mono"></param>
        public void InitMono(MonoBehaviour mono, Action luaTraceStackAction)
        {
            if (mono)
            {
                _unparentRootTransform = mono.transform.Find(UNPARENT_ROOT_NAME);
                if (!_unparentRootTransform)
                {
                    GameObject go = new GameObject(UNPARENT_ROOT_NAME);
                    go.transform.SetParent(mono.transform);
                    _unparentRootTransform = go.transform;
                    go.SetActive(false);
                    LogVerboseFormat("StatisticsFacade.InitMono _unparentRootTransform created successfully");
                }
                else
                {
                    LogVerboseFormat("StatisticsFacade.InitMono using existing _unparentRootTransform");
                }
            }
            else
            {
                GLog.Error("InstancePool.InitMono failed, input mono is null");
            }
            _luaTraceStackAction = luaTraceStackAction;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="resourceManager"></param>
        public void AddResourceManager(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="assetsStatistics"></param>
        public void AddAssetStatistics(AssetsStatistics assetsStatistics)
        {
            Assert(_assetsStatistics == null, "AddAssetStatistics failed, _assetsStatistics is not null");
            _assetsStatistics = assetsStatistics;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="bundleStatistics"></param>
        public void AddBundleStatistics(BundleStatistics bundleStatistics)
        {
            Assert(_bundleStatistics == null, "AddBundleStatistics failed, _bundleStatistics is not null");
            _bundleStatistics = bundleStatistics;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="spriteUsageStatistics"></param>
        public void AddSpriteUsageStatistics(SpriteUsageStatistics spriteUsageStatistics)
        {
            Assert(_spriteUsageStatistics == null, "AddSpriteUsageStatistics failed, _spriteUsageStatistics is not null");
            _spriteUsageStatistics = spriteUsageStatistics;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="textureUsageStatistics"></param>
        public void AddTextureUsageStatistics(TextureUsageStatistics textureUsageStatistics)
        {
            Assert(_textureUsageStatistics == null, "AddTextureUsageStatistics failed, _textureUsageStatistics is not null");
            _textureUsageStatistics = textureUsageStatistics;
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="animationClipUsageStatistics"></param>
        public void AddAnimationClipUsageStatistics(AnimationClipUsageStatistics animationClipUsageStatistics)
        {
            Assert(_animationClipUsageStatistics == null, "AddAnimationClipUsageStatistics failed, _animationClipUsageStatistics is not null");
            _animationClipUsageStatistics = animationClipUsageStatistics;
            _abstractUsageStatistics.Add(_animationClipUsageStatistics);
        }

        /// <summary>
        /// 初始化建立关系
        /// </summary>
        /// <param name="cachedUsageStatistics"></param>
        public void AddCachedUsageStatistics(CachedUsageStatistics cachedUsageStatistics)
        {
            _cachedUsageStatistics = cachedUsageStatistics;
        }

        /// <summary>
        /// 增加一个资源引用
        /// 这个被设计成仅被AssetsStatistics调用，因为它知道asset对应的bundle是谁
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="assetId"></param>
        public void AddAssetUsage(string bundleName, int assetId)
        {
            Assert(_bundleStatistics != null, "AddAssetUsage failed, _bundleStatistics is null");
            _bundleStatistics.AddAssetUsage(bundleName, assetId);
        }

        /// <summary>
        /// 当资源被使用时被调用
        /// </summary>
        /// <param name="assetId"></param>
        public void AssetAddRef(int assetId)
        {
            _assetsStatistics.AssetAddRef(assetId);
        }

        /// <summary>
        /// 当资源被释放引用时调用
        /// </summary>
        /// <param name="assetId"></param>
        public void AssetReleaseRef(int assetId, bool protectlyCall)
        {
            bool willAdd = true;
            if (protectlyCall)
            {
                if (!_assetsStatistics.IsAssetLoadedManually(assetId))
                {
                    willAdd = false;
                }
            }
            if (willAdd)
            {
                _assetsStatistics.AssetReleaseRef(assetId);
            }
        }

        /// <summary>
        /// 释放一个资源引用
        /// </summary>
        /// <param name="assetId"></param>
        public void ReleaseAssetUsage(string bundleName, int assetId)
        {
            Assert(_bundleStatistics != null, "ReleaseAssetUsage failed, _bundleStatistics is null");
            _bundleStatistics.ReleaseAssetUsage(bundleName, assetId);
        }

        /// <summary>
        /// 添加一个场景引用
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        public void AddSceneUsage(string bundleName, string sceneName)
        {
            Assert(_bundleStatistics != null, "AddSceneUsage failed, _bundleStatistics is null");
            _bundleStatistics.AddSceneUsage(bundleName, sceneName);
        }

        /// <summary>
        /// 释放一个场景引用
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        public void ReleaseSceneUsage(string bundleName, string sceneName)
        {
            Assert(_bundleStatistics != null, "ReleaseSceneUsage failed, _bundleStatistics is null");
            _bundleStatistics.ReleaseSceneUsage(bundleName, sceneName);
        }

        /// <summary>
        /// 当GameObject被加载时，需要额外处理
        /// </summary>
        /// <param name="gameObject"></param>
        private void OnGameObjectLoaded(GameObject gameObject)
        {
            GameObjectUsageBehaviour usageBehaviour = gameObject.GetComponent<GameObjectUsageBehaviour>();
            if (usageBehaviour == null)
            {
                if (releaseMode)
                {
                    GLog.ErrorFormat("OnGameObjectLoaded {0} failed, gameObject has no GameObjectUsageBehaviour", gameObject.name);
                }
                else
                {
                    usageBehaviour = gameObject.AddComponent<GameObjectUsageBehaviour>();
                }
            }
            usageBehaviour.CacheAssetId(this);
        }

        /// <summary>
        /// 通用的加载完成监视函数
        /// </summary>
        /// <param name="obj"></param>
        public int OnGenericObjectLoaded(string assetName, string bundleName, UnityEngine.Object obj)
        {
            if(obj)
            {
                int assetId = obj.GetInstanceID();
                _assetsStatistics.OnAssetLoaded(assetName, bundleName, assetId);

                if (obj is GameObject)
                {
                    GameObject go = obj as GameObject;
                    //Prefab初始化
                    OnGameObjectLoaded(go);
                }
                if (obj is Sprite)
                {
                    //记录图片精灵
                    //to do...
                    //准备记录图片加载，用于统计加载后没有立即使用的精灵，这种情况下用资源池来解决问题
                }
                return assetId;
            }
            else
            {
                GLog.ErrorFormat("OnGenericObjectLoaded failed, obj is null for asset {0} in bundle {1}", assetName, bundleName);
                return 0;
            }
        }

        /// <summary>
        /// 呼叫统计类释放资源
        /// </summary>
        public void DisposeUnusedAssetsAndBundles()
        {
            LogVerboseFormat("DisposeUnusedAssetsAndBundles begin");
            _assetsStatistics.DisposeUnusedAssets();
            _bundleStatistics.DisposeUnusedBundles();
            LogVerboseFormat("DisposeUnusedAssetsAndBundles end");
        }

        /// <summary>
        /// 门面接口，对BundleStatistics类隐藏ResourceManager
        /// </summary>
        /// <param name="bundleName"></param>
        public void DisposeUnusedBundle(string bundleName)
        {
            LogVerboseFormat("DisposeUnusedBundleSuccess assetbundle: {0}", bundleName);
            _resourceManager.UnloadAssetBundle(bundleName);
        }

        public string FindAssetBundleNameInHashCodeTable(string bundleName)
        {
            return _resourceManager.FindAssetBundleNameInHashCodeTable(bundleName);
        }

        /// <summary>
        /// 场景卸载时的准备
        /// </summary>
        public void PostSceneLoaded(Action postAction)
        {
            if(waitingDisposeCoroutine!=null)
            {
                ILoop.DestroyCoroutine(waitingDisposeCoroutine);
            }
            ILoop.CreateCoroutine(WaitAndDispose(postAction));
        }

        /// <summary>
        /// 延迟释放所有资源和AB
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndDispose(Action postAction)
        {
            yield return new WaitForEndOfFrame();   //经测试，延迟一帧已经足够等待到
            DisposeUnusedAssetsAndBundles();    //需要延后到GameObject的OnDestroy后执行，确保资源都不再使用，这样才能卸载干净。
            postAction.Invoke();
            waitingDisposeCoroutine = null;
        }

        /// <summary>
        /// 门面方法，对GameObject隐藏各种计数器
        /// </summary>
        /// <param name="obj"></param>
        public void PreDestroyObject(UnityEngine.Object obj)
        {
            if (obj is GameObject)
            {
                GameObject go = obj as GameObject;
                //断开
                _preDestroyOrRecycling(go);

                //Texture准备释放
                /* by Rick，2021年9月14日，精简贴图引用，只有个别控件会需要换贴图，对它们单独计数就可以，这样可以降低CPU开销
                _textureUsageStatistics.ReleaseTextureComponents(go);
                */

                //通用的
                foreach (var abstractUsageStatistics in _abstractUsageStatistics)
                {
                    abstractUsageStatistics.OnGameObjectDestroy(go);
                }
            }
        }

        /// <summary>
        /// 回池前的准备工作
        /// </summary>
        /// <param name="gameObject"></param>
        public void PreRecyclingObject(UnityEngine.GameObject gameObject)
        {
            //断开
            _preDestroyOrRecycling(gameObject);
        }

        /// <summary>
        /// 断开父子关系，保证引用正确
        /// </summary>
        /// <param name="gameObject"></param>
        private void _preDestroyOrRecycling(GameObject gameObject)
        {
            if(!releaseMode)
            {
                GameObjectUsageBehaviour[] allBehaviours = gameObject.GetComponentsInChildren<GameObjectUsageBehaviour>();
                foreach(var usage in allBehaviours)
                {
                    if(usage.gameObject == gameObject)
                    {
                        LogVerboseFormat("_preDestroyOrRecycling same as root");
                    }
                    else
                    {
                        GLog.ErrorFormat("_preDestroyOrRecycling should destroy or recylce child first!!!, child={0}, id={1}, root={2}, id={3}", usage.name, usage.gameObject.GetInstanceID(), gameObject.name, gameObject.GetInstanceID());
                        LuaTraceStackAction();
                    }
                }
            }
        }

        /// <summary>
        /// 门面方法，对GameObject隐藏各种计数器
        /// </summary>
        /// <param name="gameObject"></param>
        public void PostGameObjectAwake(UnityEngine.GameObject gameObject)
        {
            /* 
             * by Rick，2021年9月14日，精简贴图引用，只有个别控件会需要换贴图，对它们单独计数就可以，这样可以降低CPU开销
            //Texture初始化
            Assert(_textureUsageStatistics != null, "PostGameObjectAwake faild, _textureUsageStatistics is null");
            _textureUsageStatistics.InitTextureComponents(gameObject);
            */
            //通用的
            foreach (var abstractUsageStatistics in _abstractUsageStatistics)
            {
                abstractUsageStatistics.OnGameObjectAwake(gameObject);
            }
        }

        /// <summary>
        /// 门面方法，对ResourceManager隐藏各种计数器
        /// </summary>
        /// <param name="bundleName"></param>
        public void OnAssetbundleLoaded(string bundleName)
        {
            Assert(_bundleStatistics != null, "OnAssetbundleLoaded faild, _bundleStatistics is null");
            _bundleStatistics.OnAssetbundleLoaded(bundleName);
        }

        /// <summary>
        /// 门面方法，对ResourceManager隐藏各种计数器
        /// </summary>
        /// <param name="bundleName"></param>
        public void OnAssetbundleUnloaded(string bundleName)
        {
            Assert(_bundleStatistics != null, "OnAssetbundleUnloaded faild, _bundleStatistics is null");
            _bundleStatistics.OnAssetbundleUnloaded(bundleName);
        }



        /// <summary>
        /// by Rick
        /// 资源设置的代理方法，接管所有精灵图片设置方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sprite"></param>
        public void AssetProxySetSprite(Image image, Sprite sprite)
        {
            _spriteUsageStatistics.SetSprite(image, sprite);
        }

        public void AssetProxySetCustomSprite<T, S>(T image, S spriteInfo, Func<T, S, Sprite> setSprite) where T : Image
        {
            _spriteUsageStatistics.SetCustomSprite(image, spriteInfo, setSprite);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="material"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public void AssetProxySetMaterialTexture(Material material, string name, Texture texture)
        {
            _textureUsageStatistics.SetMaterialTexture(material, name, texture);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="rawImage"></param>
        /// <param name="texture"></param>
        public void AssetProxySetRawImage(RawImage rawImage, Texture texture)
        {
            _textureUsageStatistics.SetRawImage(rawImage, texture);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="image"></param>
        public void AssetProxyOnImageCreate(Image image)
        {
            Assert(_spriteUsageStatistics != null, "OnImageCreate failed, _spriteUsageStatistics is null");
            _spriteUsageStatistics.OnImageCreate(image);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="image"></param>
        public void AssetProxyOnImageDestroy(Image image)
        {
            Assert(_spriteUsageStatistics != null, "AssetProxyOnImageDestroy failed, _spriteUsageStatistics is null");
            _spriteUsageStatistics.OnImageDestroy(image);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="image"></param>
        public void AssetProxyOnRawImageCreate(RawImage image)
        {
            Assert(_textureUsageStatistics != null, "AssetProxyOnRawImageCreate failed, _textureUsageStatistics is null");
            _textureUsageStatistics.OnRawImageCreate(image);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="image"></param>
        public void AssetProxyOnRawImageDestroy(RawImage image)
        {
            Assert(_textureUsageStatistics != null, "AssetProxyOnRawImageDestroy failed, _textureUsageStatistics is null");
            _textureUsageStatistics.OnRawImageDestroy(image);
        }

        /// <summary>
        /// 门面方法，除RawImage以外的贴图引用
        /// </summary>
        /// <param name="texture"></param>
        public void AssetProxyAddCustomTextureRef(Texture texture)
        {
            Assert(_textureUsageStatistics != null, "AssetProxyAddCustomTextureRef failed, _textureUsageStatistics is null");
            _textureUsageStatistics.AddCustomTextureRef(texture);
        }

        /// <summary>
        /// 门面方法，除RawImage以外的贴图引用
        /// </summary>
        /// <param name="texture"></param>
        public void AssetProxyReleaseCustomTextureRef(Texture texture)
        {
            Assert(_textureUsageStatistics != null, "AssetProxyReleaseCustomTextureRef failed, _textureUsageStatistics is null");
            _textureUsageStatistics.ReleaseCustomTextureRef(texture);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="animatorOverrideController"></param>
        /// <param name="stateName"></param>
        /// <param name="clip"></param>
        public void ReplaceAnimationClip(AnimatorOverrideController animatorOverrideController, string stateName, AnimationClip clip)
        {
            Assert(_animationClipUsageStatistics != null, "ReplaceAnimationClip failed, _animationClipUsageStatistics is null");
            _animationClipUsageStatistics.ReplaceAnimationClip(animatorOverrideController, stateName, clip);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="animator"></param>
        public void InitGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Assert(_animationClipUsageStatistics != null, "InitGlobalAnimatorController failed, _animationClipUsageStatistics is null");
            _animationClipUsageStatistics.InitGlobalAnimatorController(animator);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="animator"></param>
        public void UseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Assert(_animationClipUsageStatistics != null, "UseGlobalAnimatorController failed, _animationClipUsageStatistics is null");
            _animationClipUsageStatistics.UseGlobalAnimatorController(animator);
        }
        
        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="animator"></param>
        public void UnuseGlobalAnimatorController(RuntimeAnimatorController animator)
        {
            Assert(_animationClipUsageStatistics != null, "UnuseGlobalAnimatorController failed, _animationClipUsageStatistics is null");
            _animationClipUsageStatistics.UnuseGlobalAnimatorController(animator);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="assetType"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string AssetProxyCreateCachedGroup(Type assetType, string groupName)
        {
            Assert(_cachedUsageStatistics != null, "AssetProxyCreateCachedGroup faild, _cachedUsageStatistics is null");
            return _cachedUsageStatistics.CreateGroup(assetType, groupName);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="groupUniqueName"></param>
        public void AssetProxyAddUsageToCachedGroup(int assetId, string groupUniqueName)
        {
            Assert(_cachedUsageStatistics != null, "AssetProxyAddUsageToCachedGroup faild, _cachedUsageStatistics is null");
            _cachedUsageStatistics.AddUsageToGroup(assetId, groupUniqueName);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="groupUniqueName"></param>
        public void AssetProxyReleaseCachedGroup(string groupUniqueName)
        {
            Assert(_cachedUsageStatistics != null, "AssetProxyReleaseCachedGroup faild, _cachedUsageStatistics is null");
            _cachedUsageStatistics.ReleaseGroup(groupUniqueName);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool AssetProxyIsAssetLoaded(int assetId)
        {
            Assert(_assetsStatistics != null, "AssetProxyIsAssetLoaded failed, _assetsStatistics is null");
            return _assetsStatistics.IsAssetLoaded(assetId);
        }

        /// <summary>
        /// 门面方法，对应用层隐藏各种计数器
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="bundleName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool FindAssetInfo(int assetId, out string bundleName, out string assetName)
        {
            Assert(_assetsStatistics != null, "FindAssetInfo failed, _assetsStatistics is null");
            return _assetsStatistics.FindAssetInfo(assetId, out bundleName, out assetName);
        }

        /// <summary>
        /// 打印当前资源使用情况
        /// </summary>
        /// <param name="writer"></param>
        public void DumpUsage(System.IO.StreamWriter writer)
        {
            Assert(_bundleStatistics != null, "DumpUsage failed, _bundleStatistics is null");
            _bundleStatistics.Dump(writer);
            writer.WriteLine(string.Empty);
            Assert(_assetsStatistics != null, "DumpUsage failed, _assetsStatistics is null");
            _assetsStatistics.Dump(writer);
        }

        public void CreateBuiltInCachedUsageGroup()
        {
            Assert(_cachedUsageStatistics != null, "CreateBuiltInCachedUsageGroup failed, _cachedUsageStatistics is null");
            builtInCacheGroupId = _cachedUsageStatistics.CreateGroup(typeof(UnityEngine.Object), "BuiltIn");
        }

        public string GetBuiltInCachedUsageGroupId()
        {
            Assert(builtInCacheGroupId != null, "GetBuiltInCachedUsageGroupId failed, builtInCacheGroupId is null");
            return builtInCacheGroupId;
        }

        public void AssetProxyAddUsageToBuiltInCachedGroup(int assetId)
        {
            Assert(builtInCacheGroupId != null, "AssetProxyAddUsageToBuiltInCachedGroup failed, builtInCacheGroupId is null");
            AssetProxyAddUsageToCachedGroup(assetId, builtInCacheGroupId);
        }

        public static bool PrepareGameObjectAsset(GameObject go)
        {
            GameObjectUsageBehaviour usageCom = go.GetComponent<GameObjectUsageBehaviour>();
            if (usageCom != null)
            {

            }
            else
            {
                usageCom = go.AddComponent<GameObjectUsageBehaviour>();
            }
            /*
            bool changed = usageCom.CacheTextureReferences();
            return changed;
            */
            return false;
        }

        public static bool RemoveGameObjectAsset(GameObject go)
        {
            GameObjectUsageBehaviour usageCom = go.GetComponent<GameObjectUsageBehaviour>();
            if(usageCom != null)
            {
                GameObject.DestroyImmediate(usageCom, true);
                return true;
            }
            return false;
        }
    }
}
