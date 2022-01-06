using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using GameDLL.Resource.Statistics;
using Object = UnityEngine.Object;

namespace GameDLL
{
    /// <summary>
    /// 异步加载AssetBundle返回资源
    /// </summary>
    public class AssetBundleData
    {
        public Type type;        //需要的类型
        public object asset;            //返回的数据
    }

    /// <summary>
    /// 异步加载文件返回资源
    /// </summary>
    public class FileData
    {
        public string type;        //需要的类型
        public object asset;            //返回的数据
        public FileData(string type)
        {
            this.type = type;
        }
    }

    public class ResourceManager : Singleton<ResourceManager>, IManager
    {
        private IGame _igame;

        /// <summary>
        /// AssetBundle详细依赖列表
        /// </summary>
        private AssetBundleManifest _manifest;

        /// <summary>
        /// AssetBundle信息缓存
        /// </summary>
        private Dictionary<string, AssetBundleInfo> _bundleDic = new Dictionary<string, AssetBundleInfo>();

        /// <summary>
        /// 正在异步加载的资源
        /// </summary>
        private List<string> isLoadingAssetBundleInfo = new List<string>();

        /// <summary>
        /// by Rick, 加载的场景名，按分层登记表
        /// </summary>
        private Dictionary<int, KeyValuePair<string, string>> sceneNameByLayer = new Dictionary<int, KeyValuePair<string, string>>();

        public static IEditorTools editorTools;

        static bool developMode;

        static public bool DevelopMode
        {
            get
            {
                return developMode;
            }
        }

        /// <summary>
        /// by Rick, 重定向过的manifest路径
        /// </summary>
        private string _redirectedManifestDirectory = null;

        /// <summary>
        /// by Rick, 重定向过的assetbundle路径
        /// </summary>
        private string _redirectedAssetbundleDirectory = null;

        /// <summary>
        /// by Rick, 资源门面类实例
        /// </summary>
        readonly private StatisticsFacade statisticsFacade = new StatisticsFacade();
        bool _statisticsInitiated = false;

        public StatisticsFacade StatisticsFacade
        {
            get { return statisticsFacade; }
        }

        /// <summary>
        /// by Rick, 资源计数器
        /// </summary>
        private AssetsStatistics assetsStatistics;

        /// <summary>
        /// by Rick, AB包计数器
        /// </summary>
        private BundleStatistics bundleStatistics;

        /// <summary>
        /// by Rick, 精灵计数器
        /// </summary>
        private SpriteUsageStatistics spriteStatistics;

        /// <summary>
        /// by Rick, 贴图计数器
        /// </summary>
        private TextureUsageStatistics textureUsageStatistics;

        /// <summary>
        /// by Rick, 动画片段计数器
        /// </summary>
        private AnimationClipUsageStatistics animationClipUsageStatistics;

        /// <summary>
        /// by Rick, 通用的缓存计数器
        /// </summary>
        private CachedUsageStatistics cachedUsageStatistics;

        /// <summary>
        /// 加上MD5码的AB文件名查找表
        /// </summary>
        private Dictionary<string, string> _abNameWithHashCodeLookupTable = new Dictionary<string, string>();

        /// <summary>
        /// MD5码的文件名分隔符
        /// </summary>
        const char _hashCodeSeperator = '_';
        const char _surfixSeperator = '.';

        /// <summary>
        /// 异步加载asset bundle回调函数
        /// </summary>
        public delegate void LoadAsyncComplete(object asset);

        /// <summary>
        /// by Rick, 异步加载asset bundle的统计回调
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="bundleName"></param>
        /// <param name="asset"></param>
        public delegate void LoadAsyncPreComplete(string assetName, string bundleName, object asset);

        #region 管理器接口函数
        public void Init(IGame game)
        {
            //by Rick，添加统计对象
            if (!_statisticsInitiated)
            {
                _statisticsInitiated = true;
                statisticsFacade.AddResourceManager(this);
                assetsStatistics = new AssetsStatistics(statisticsFacade);
                statisticsFacade.AddAssetStatistics(assetsStatistics);
                bundleStatistics = new BundleStatistics(statisticsFacade);
                statisticsFacade.AddBundleStatistics(bundleStatistics);
                spriteStatistics = new SpriteUsageStatistics(statisticsFacade);
                statisticsFacade.AddSpriteUsageStatistics(spriteStatistics);
                textureUsageStatistics = new TextureUsageStatistics(statisticsFacade);
                statisticsFacade.AddTextureUsageStatistics(textureUsageStatistics);
                cachedUsageStatistics = new CachedUsageStatistics(statisticsFacade);
                statisticsFacade.AddCachedUsageStatistics(cachedUsageStatistics);
                statisticsFacade.CreateBuiltInCachedUsageGroup();
                animationClipUsageStatistics = new AnimationClipUsageStatistics(statisticsFacade);
                statisticsFacade.AddAnimationClipUsageStatistics(animationClipUsageStatistics);
            }
            //~by Rick

            _igame = game;
            UpdateDevelopMode();
            LoadAssetBundleManifest();
        }

        public void Reset()
        {

        }

        public void Dispose()
        {
            _manifest = null;
            foreach (var abi in _bundleDic)
                abi.Value.UnloadAll(true);
            _bundleDic.Clear();
        }
        #endregion

        #region AssetBundle函数

        /// <summary>
        /// 从AssetBundle加载资源依赖列表,文件名默认为ABManifest
        /// </summary>
        public void LoadAssetBundleManifest(string fileName = Const.AssetBundleManifestName)
        {
            if (!developMode && _manifest == null)
            {
                AssetBundle assetbundle = LoadManifestAssetBundleFromFile(fileName);    //by Rick, 使用版本分开存储方案的AB位置
                if (assetbundle != null)
                {
                    _manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    if (_manifest == null)
                        GLog.ErrorFormat("can not load AssetBundleManifest");

                    //by Rick, 多版本共存模式下，AB多个版本由文件名的HashCode部分做共存，在这里构建查找表
                    _abNameWithHashCodeLookupTable.Clear();
                    string[] allAssetBundles = _manifest.GetAllAssetBundles();
                    foreach (var fullAbName in allAssetBundles)
                    {
                        //所有打出的AB文件名必须符合 "XXXX_XXX.XX"的规则
                        int splitHashIndex = fullAbName.LastIndexOf(_hashCodeSeperator);
                        if (splitHashIndex > 0)
                        {
                            string secondName = fullAbName.Substring(splitHashIndex + 1);
                            string firstName = fullAbName.Substring(0, splitHashIndex);

                            int splitSurfixIndex = secondName.LastIndexOf(_surfixSeperator);
                            string combined = firstName;
                            if (splitSurfixIndex > 0)
                            {
                                string surfix = secondName.Substring(splitSurfixIndex);
                                combined = firstName + surfix;
                            }
                            else
                            {
                                //GLog.WarningFormat("building _abNameWithHashCodeLookupTable not surfix found: {0}", fullAbName);
                                statisticsFacade.LogVerboseFormat("building _abNameWithHashCodeLookupTable not surfix found: {0}", fullAbName); //因为__auto类型的ab不能带后缀，所以这个是合法的，不需要再报告
                            }
                            if (_abNameWithHashCodeLookupTable.ContainsKey(combined))
                            {
                                GLog.ErrorFormat("building _abNameWithHashCodeLookupTable failed, firstName duplicated: {0}", combined);
                            }
                            else
                            {
                                _abNameWithHashCodeLookupTable.Add(combined, fullAbName);
                            }
                        }
                        else
                        {
                            GLog.ErrorFormat("building _abNameWithHashCodeLookupTable failed, ab name cannot be splitted: {0}", fullAbName);
                        }
                    }

                    assetbundle.Unload(false);
                    assetbundle = null;
                }
                else
                    GLog.ErrorFormat("AssetBundle {0} is not exist", fileName);

                //by Rick
                //关联清单
                bundleStatistics.AssignManifest(_manifest);
                //~by Rick
            }
        }

        /// <summary>
        /// 读取AssetBundle文件
        /// </summary>
        AssetBundle LoadAssetBundle(string abname)
        {
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);
            if (abInfo != null)
                return abInfo.bundle;
            return null;
        }

        /// <summary>
        /// 优先从持久化路径获取文件
        /// 持久化路径找不到文件从包内获取文件
        /// </summary>
        AssetBundle LoadAssetBundleFromFile(string fileName, bool addHashcode)
        {
            string filePath = ResourceUtils.GetAssetsPath(fileName);
            return AssetBundle.LoadFromFile(filePath);//可以使用带offset的方法进行简单加密
        }

        /// <summary>
        /// by Rick, 从带MD5码的查找表里找到对应文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string FindAssetBundleNameInHashCodeTable(string fileName)
        {
            if (DevelopMode)
            {
                return fileName;
            }
            string lookupWithHashCode = null;
            if (_abNameWithHashCodeLookupTable.TryGetValue(fileName, out lookupWithHashCode))
            {

            }
            else
            {
                GLog.ErrorFormat("FindAssetBundleNameInHashCodeTable failed, cannot find in _abNameWithHashCodeLookupTable: {0}", fileName);
            }
            return lookupWithHashCode;
        }

        AssetBundle LoadManifestAssetBundleFromFile(string fileName)
        {
            string filePath = null;
            if (_redirectedManifestDirectory != null)
            {
                filePath = Path.Combine(_redirectedManifestDirectory, fileName);
            }
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                filePath = ResourceUtils.GetStreamingAssetsFilePath(fileName);
            return AssetBundle.LoadFromFile(filePath);
        }

        /// <summary>
        /// 加载依赖包
        /// </summary>
        void LoadDependencies(string abname)
        {
            LoadAssetBundleManifest();
            string[] dependencies = _manifest.GetAllDependencies(abname);
            int len = dependencies.Length;
            if (len == 0)
                return;
            for (int i = 0; i < len; i++)
            {
                if (dependencies[i] == abname)
                {
                    statisticsFacade.LogVerboseFormat("LoadDependencies same with abname {0}", abname);
                }
                else
                {
                    LoadAssetBundleInfo(dependencies[i], false);
                }
            }
        }

        /// <summary>
        /// 加载AssetBundle信息文件
        /// </summary>
        AssetBundleInfo LoadAssetBundleInfo(string abname, bool recursive)
        {
            AssetBundleInfo abInfo = null;
            try
            {
                abname = abname.ToLower();
                string withHashcode = abname;
                if (recursive)
                {
                    withHashcode = FindAssetBundleNameInHashCodeTable(abname);
                }
                if (!_bundleDic.TryGetValue(withHashcode, out abInfo))
                {
                    if (recursive)
                    {
                        LoadDependencies(withHashcode);       //加载依赖资源
                    }

                    if (isLoadingAssetBundleInfo.Contains(withHashcode))
                        isLoadingAssetBundleInfo.Remove(withHashcode);        //清除异步加载到一半的资源

                    AssetBundle bundle = LoadAssetBundleFromFile(withHashcode, recursive);   //recuisive为false说明是在根据Manifest获取出来的依赖包，它是自带HashCode的，也不是从应用层发起加载的
                    if (bundle != null)
                    {
                        abInfo = new AssetBundleInfo(withHashcode, bundle);
                        _bundleDic.Add(withHashcode, abInfo);
                        statisticsFacade.OnAssetbundleLoaded(withHashcode);   //by Rick, 加入监视代码
                    }
                }
                return abInfo;
            }
            catch (Exception e)
            {
                GLog.ErrorFormat("LoadAssetBundle Failed! {0}", e.ToString());
                GLog.Exception(e);
            }
            return null;
        }

        /// <summary>
        /// 从AssetBundle读取资源
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname, bool isCacheAsset = false) where T : Object
        {
            if (string.IsNullOrEmpty(abname) || string.IsNullOrEmpty(assetname))
                return null;
            if (developMode)
            {
                // by Rick
                // 记录Asset和Bundle的关系
                T assetByEditor = editorTools.LoadAsset<T>(abname, assetname);
                if (Application.isPlaying)
                {
                    statisticsFacade.OnAssetbundleLoaded(abname);
                    int assetIdByEditor = statisticsFacade.OnGenericObjectLoaded(assetname, abname, assetByEditor);
                    if (isCacheAsset)
                    {
                        statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetIdByEditor);
                    }
                }
                // ~by Rick
                return assetByEditor;
            }
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);
            if (null == abInfo) return default(T);
            // by Rick
            // 记录Asset和Bundle的关系
            Type t = typeof(T);
            T asset = (T)abInfo.LoadAsset(t, assetname, isCacheAsset);
            //by Rick
            int assetId = statisticsFacade.OnGenericObjectLoaded(assetname, abname, asset);
            if (isCacheAsset)
            {
                statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetId);
            }
            // ~by Rick
            return asset;
        }

        /// <summary>
        /// 通过Type类型从AssetBundle读取资源
        /// </summary>
        public Object LoadAsset(Type type, string abname, string assetname, bool isCacheAsset = false)
        {
            if (string.IsNullOrEmpty(abname) || string.IsNullOrEmpty(assetname))
                return null;
            if (developMode)
            {
                // by Rick
                // 记录Asset和Bundle的关系
                Object assetByEditor = editorTools.LoadAsset(type, abname, assetname);
                if (Application.isPlaying)
                {
                    statisticsFacade.OnAssetbundleLoaded(abname);
                    int assetIdByEditor = statisticsFacade.OnGenericObjectLoaded(assetname, abname, assetByEditor);
                    if (isCacheAsset)
                    {
                        statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetIdByEditor);
                    }
                }
                // ~by Rick
                return assetByEditor;
            }
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);
            if (null == abInfo) return null;
            // by Rick
            // 记录Asset和Bundle的关系
            Object asset = abInfo.LoadAsset(type, assetname, isCacheAsset);
            int assetId = statisticsFacade.OnGenericObjectLoaded(assetname, abname, asset);
            if (isCacheAsset)
            {
                statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetId);
            }
            // ~by Rick
            return asset;
        }

        /// <summary>
        /// XLua从AssetBundle读取泛型资源
        /// </summary>
        public T XLuaLoadAsset<T>(T t, string abname, string assetname, bool isCacheAsset = false) where T : Object
        {
            return LoadAsset<T>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle获取预设资源
        /// </summary>
        public GameObject LoadGameObject(string abname, string assetname, bool isCacheAsset = false)
        {
            GameObject output = LoadAsset<GameObject>(abname, assetname, isCacheAsset);
            return output;
        }

        /// <summary>
        /// 从AssetBundle获取材质球资源
        /// </summary>
        public Material LoadMaterial(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<Material>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle获取贴图资源
        /// </summary>
        public Texture LoadTexture(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<Texture>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle获取Sprite资源
        /// </summary>
        public Sprite LoadSprite(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<Sprite>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle获取声音资源
        /// </summary>
        public AudioClip LoadAudioClip(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<AudioClip>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle获取文件资源
        /// </summary>
        public TextAsset LoadTextAsset(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<TextAsset>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 从AssetBundle文件资源获取string
        /// </summary>
        public string LoadText(string abname, string assetname, bool isCacheAsset = false)
        {
            TextAsset asset = LoadTextAsset(abname, assetname, isCacheAsset);
            if (asset)
                return asset.text;
            return null;
        }

        /// <summary>
        /// 从AssetBundle文件资源获取二进制数组
        /// </summary>
        public byte[] LoadBytes(string abname, string assetname, bool isCacheAsset = false)
        {
            TextAsset asset = LoadTextAsset(abname, assetname, isCacheAsset);
            if (asset)
                return asset.bytes;
            return null;
        }

        /// <summary>
        /// 从AssetBundle文件资源获取ShaderVariantCollection
        /// </summary>
        public ShaderVariantCollection LoadShaderVariantCollection(string abname, string assetname, bool isCacheAsset = false)
        {
            return LoadAsset<ShaderVariantCollection>(abname, assetname, isCacheAsset);
        }

        /// <summary>
        /// 优先从持久化路径获取文件
        /// 持久化路径找不到文件从包内获取文件
        /// </summary>
        AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string fileName)
        {
            string filePath = ResourceUtils.GetAssetsPath(fileName);
            return AssetBundle.LoadFromFileAsync(filePath);
        }

        /// <summary>
        /// 异步加载AssetBundle信息文件
        /// </summary>
        IEnumerator LoadAssetBundleInfoAsync(string abname, AssetBundleData assetIn)
        {
            AssetBundleInfo abInfo = null;
            abname = abname.ToLower();
            string abWithHashcode = FindAssetBundleNameInHashCodeTable(abname);
            abname = abWithHashcode;
            if (_bundleDic.TryGetValue(abname, out abInfo))
            {
                if (assetIn != null)
                    assetIn.asset = abInfo;
            }
            else
            {
                if (isLoadingAssetBundleInfo.Contains(abname))
                {
                    while (isLoadingAssetBundleInfo.Contains(abname))       //等待其他协程加载完成
                        yield return null;
                }
                else
                {
                    AssetBundleCreateRequest request;
                    //异步加载依赖包
                    isLoadingAssetBundleInfo.Add(abname);
                    string[] dependencies = _manifest.GetAllDependencies(abname);
                    string dependency;
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        dependency = dependencies[i];
                        if (!_bundleDic.ContainsKey(dependency))
                        {
                            if (isLoadingAssetBundleInfo.Contains(dependency))
                            {
                                while (isLoadingAssetBundleInfo.Contains(dependency))       //等待其他协程加载完成
                                    yield return null;
                            }
                            else
                            {
                                isLoadingAssetBundleInfo.Add(dependency);

                                request = LoadAssetBundleFromFileAsync(dependency);
                                if (request != null)
                                {
                                    while (!request.isDone)
                                        yield return null;
                                    if (request.assetBundle != null && isLoadingAssetBundleInfo.Contains(dependency))
                                    {
                                        _bundleDic.Add(dependency, new AssetBundleInfo(dependency, request.assetBundle));       //如果没有被同步加载函数清除则加入缓存
                                        statisticsFacade.OnAssetbundleLoaded(dependency);   //by Rick, 加入监视代码
                                    }
                                }

                                isLoadingAssetBundleInfo.Remove(dependency);
                            }
                        }
                    }

                    //异步加载自己需要的AssetBundle
                    request = LoadAssetBundleFromFileAsync(abname);

                    if (request != null)
                    {
                        while (!request.isDone)
                            yield return null;
                        if (request.assetBundle != null && isLoadingAssetBundleInfo.Contains(abname))
                        {
                            abInfo = new AssetBundleInfo(abname, request.assetBundle);
                            _bundleDic.Add(abname, abInfo);                        //如果没有被同步加载函数清除则加入缓存
                            statisticsFacade.OnAssetbundleLoaded(abname);   //by Rick, 加入监视代码
                        }
                    }
                    isLoadingAssetBundleInfo.Remove(abname);
                }

                if (_bundleDic.TryGetValue(abname, out abInfo))
                {
                    if (assetIn != null)
                        assetIn.asset = abInfo;
                }
            }
        }

        /// <summary>
        /// 从AssetBundle异步读取资源
        /// </summary>
        public IEnumerator LoadAssetAsync(string abname, string assetname, AssetBundleData assetIn, bool isCacheAsset = false, LoadAsyncComplete complete = null)
        {
            if (assetIn == null || assetIn.type == null)
            {
                if (complete != null)
                    complete(null);
                yield break;
            }
            if (developMode)
            {
                yield return editorTools.LoadAssetAsync(abname, assetname, assetIn, complete, LoadAsyncPreCompleteFn);
                //by Rick
                //记录资源使用
                Object unityObject = assetIn.asset as Object;
                if (Application.isPlaying)
                    statisticsFacade.OnAssetbundleLoaded(abname);
                if (unityObject)
                {
                    if (Application.isPlaying)
                    {
                        int assetId = statisticsFacade.OnGenericObjectLoaded(assetname, abname, unityObject);
                        if (isCacheAsset)
                        {
                            statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetId);
                        }
                    }
                }
                else
                {
                    GLog.ErrorFormat("LoadAssetAsync failed, object is not an UnityObject, [[ {0} --- {1} ]]", abname, assetname);
                }
                //~by Rick
                yield break;
            }

            //by Rick，2021年8月19日，所有依赖资源都单独打AB之后，同步/异步加载AB会引发重复加载的冲突，这里就全部改成同步加载AB，只是资源异步加载
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);
            if (abInfo != null)
            {
                yield return abInfo.LoadAssetAsync(assetname, abname, assetIn, false, (obj) =>
                {
                    //by Rick
                    //记录资源使用
                    Object unityObject = assetIn.asset as Object;
                    if (unityObject)
                    {
                        int assetId = statisticsFacade.OnGenericObjectLoaded(assetname, abname, unityObject);
                        if (isCacheAsset)
                        {
                            statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetId);
                        }
                    }
                    else
                    {
                        //GLog.ErrorFormat("LoadAssetAsync failed, object is not an UnityObject, [[ {0} --- {1} ]]", abname, assetname);
                    }
                    //~by Rick
                    if (complete != null)
                    {
                        complete(obj);
                    }
                });
            }
        }

        /// <summary>
        /// 从AssetBundle异步读取资源
        /// </summary>
        public IEnumerator LoadAssetAsync(Type type, string abname, string assetname, AssetBundleData assetIn, bool isCacheAsset = false, LoadAsyncComplete complete = null)
        {
            if (type == null)
            {
                if (complete != null)
                    complete(null);
                yield break;
            }

            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = type;

            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步读取预设资源
        /// </summary>
        public IEnumerator LoadGameObjectAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(GameObject);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取材质球资源
        /// </summary>
        public IEnumerator LoadMaterialAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(Material);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取贴图资源
        /// </summary>
        public IEnumerator LoadTextureAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(Texture);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取Sprite资源
        /// </summary>
        public IEnumerator LoadSpriteAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(Sprite);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取声音资源
        /// </summary>
        public IEnumerator LoadAudioClipAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(AudioClip);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取文件资源
        /// </summary>
        public IEnumerator LoadTextAssetAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(TextAsset);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }

        /// <summary>
        /// 从AssetBundle异步获取Shaderv
        /// </summary>
        public IEnumerator LoadShaderVariantCollectionAsync(string abname, string assetname, LoadAsyncComplete complete, AssetBundleData assetIn = null, bool isCacheAsset = false)
        {
            if (assetIn == null)
                assetIn = new AssetBundleData();
            assetIn.type = typeof(ShaderVariantCollection);
            yield return LoadAssetAsync(abname, assetname, assetIn, isCacheAsset, complete);
        }


        /// <summary>
        /// 释放AssetBundle资源
        /// </summary>
        public void UnloadAssetBundle(string abname, bool unloadAllLoadedObjects = true)
        {
            if (developMode)
            {
                //by Rick, 加入监视代码
                statisticsFacade.OnAssetbundleUnloaded(abname);
                return;
            }
            if (isLoadingAssetBundleInfo.Contains(abname))
            {
                GLog.ErrorFormat("UnloadAssetBundle:{0} failed. because this ab is loading...", abname);
                return;
            }

            AssetBundleInfo bundleInfo;
            abname = abname.ToLower();

            if (_bundleDic.TryGetValue(abname, out bundleInfo))
            {
                bundleInfo.UnloadAll(unloadAllLoadedObjects);
                _bundleDic.Remove(abname);
                statisticsFacade.OnAssetbundleUnloaded(abname); //by Rick，加入监视代码
            }
            else
                GLog.ErrorFormat("UnloadAssetBundle:{0}", abname);
        }

        /// <summary>
        /// 释放AssetBundle资源及所有依赖资源
        /// </summary>
        public void UnloadAssetBundleAndDependencies(string abname, List<string> except = null, bool unloadAllLoadedObjects = true)
        {
            if (developMode)
                return;
            string[] dependencies = _manifest.GetAllDependencies(abname);       //获取所有的依赖资源
            for (int i = 0; i < dependencies.Length; i++)
            {
                if (except == null || !except.Contains(dependencies[i]))
                    UnloadAssetBundle(dependencies[i], unloadAllLoadedObjects);
            }
            UnloadAssetBundle(abname, unloadAllLoadedObjects);
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void UnloadAllAssetBundles(bool unloadAllLoadedObjects = true)
        {
            if (developMode)
                return;
            Dictionary<string, AssetBundleInfo>.Enumerator e = _bundleDic.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    KeyValuePair<string, AssetBundleInfo> kv = e.Current;
                    kv.Value.UnloadAll(unloadAllLoadedObjects);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (e as IDisposable)) != null)
                    disposable.Dispose();
            }
            _bundleDic.Clear();

        }
        #endregion

        public static void UpdateDevelopMode()
        {
            developMode = Application.isEditor && editorTools != null && editorTools.DevelopMode();
        }

        /// <summary>
        /// 通过AssetDataInfo信息加载特定资源
        /// </summary>
        public Object GetAssetObject(AssetDataInfo info, Type type = null, bool cacheAsset = false, bool editor = false)
        {
            if (type == null)
                type = Type.GetType(info.assetType);
            //从场景获取资源
            if (info.assetInScene)
            {
                Transform trans = ResourceUtils.GetTransformFromPath(info.assetPath);
                if (trans)
                {
                    if (type == typeof(GameObject))
                        return trans.gameObject;
                    if (type.IsSubclassOf(typeof(Component)))
                        return trans.GetComponent(type);
                    if (Application.isPlaying && SceneManager.GetActiveScene().name != "BattleFieldSimulator")
                        GLog.ErrorFormat("ResourceManager.GetAssetObject(AssetDataInfo, Type, bool) Unknown Type:{0}", type);
                    return null;
                }

                if (string.IsNullOrEmpty(info.assetInfo))
                {
                    if (Application.isPlaying && SceneManager.GetActiveScene().name != "BattleFieldSimulator")
                        GLog.ErrorFormat("ResourceManager.GetAssetObject(AssetDataInfo, Type, bool) Unknown Path:{0}", info.assetPath);
                    return null;
                }
            }

            string path = info.assetInfo;
            string guid;
            string assetName;
            path = ResourceUtils.GetAssetDataInfo(path, out guid, out assetName);

            Object asset = null;

            if (string.IsNullOrEmpty(assetName))
                asset = Resources.Load(path, type);
            else if (editorTools == null || editorTools.SimulatorRunningCheck(editor))
                asset = LoadAsset(type, path, assetName, cacheAsset);


            if (!asset && editorTools != null)
            {
                asset = editorTools.EditorLoadObject(editor, info, guid, assetName, type);
                if (editorTools.CheckRunningOrSimulator(editor))
                {
                    statisticsFacade.OnAssetbundleLoaded(path);
                    int assetIdByEditor = statisticsFacade.OnGenericObjectLoaded(assetName, path, asset);
                    if (cacheAsset)
                    {
                        statisticsFacade.AssetProxyAddUsageToBuiltInCachedGroup(assetIdByEditor);
                    }
                }
            }

            return asset;
        }


        /// <summary>
        /// 通过AssetDataInfo信息加载特定资源
        /// </summary>
        public T GetAssetObject<T>(AssetDataInfo info, bool cacheAsset = false, bool editor = false) where T : Object
        {
            return GetAssetObject(info, typeof(T), cacheAsset, editor) as T;
        }

        /// <summary>
        /// 通过AssetDataInfo信息异步加载特定资源
        /// </summary>
        public IEnumerator GetObjectFromAssetPathAsync(string assetPath, Type assetType, bool isCacheAsset, LoadAsyncComplete complete)
        {
            string guid;
            string assetName;
            assetPath = ResourceUtils.GetAssetDataInfo(assetPath, out guid, out assetName);

            AssetBundleData assetIn = new AssetBundleData();
            assetIn.type = assetType;
            yield return LoadAssetAsync(assetPath, assetName, assetIn, isCacheAsset);

            object obj = assetIn.asset;
            if (obj == null)
            {
                ResourceRequest request = null;
                if (string.IsNullOrEmpty(assetName))
                    request = Resources.LoadAsync(assetPath, assetType);
                else
                    request = Resources.LoadAsync(assetPath + "/" + assetName, assetType);
                if (request != null)
                {
                    yield return request;
                    while (!request.isDone)
                        yield return request;
                    obj = request.asset;
                }
            }
            if (obj == null && editorTools != null)
                obj = editorTools.EditorLoadObjectFromGUID(guid, assetType);

            if (complete != null)
                complete(obj);
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public void GC()
        {
            System.GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 异步从路径加载文本
        /// </summary>
        public IEnumerator WebRequestLoadText(string fileName, LoadAsyncComplete complete)
        {
            FileData data = new FileData("string");
            string filePath = ResourceUtils.GetAssetsPathFromRequest(fileName);
            UnityWebRequest request = new UnityWebRequest(filePath);
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return 0;

            if (string.IsNullOrEmpty(request.error))
                data.asset = request.downloadHandler.text;
            else
                GLog.ErrorFormat(request.error);

            request.Dispose();

            if (complete != null)
                complete(data.asset);
        }

        /// <summary>
        /// 异步从路径加载二进制数组
        /// </summary>
        public IEnumerator WebRequestLoadBytes(string fileName, LoadAsyncComplete complete)
        {
            FileData data = new FileData("byte[]");
            string filePath = ResourceUtils.GetAssetsPathFromRequest(fileName);
            UnityWebRequest request = new UnityWebRequest(filePath);
            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return 0;

            if (string.IsNullOrEmpty(request.error))
                data.asset = request.downloadHandler.data;
            else
                GLog.ErrorFormat(request.error);

            request.Dispose();

            if (complete != null)
                complete(data.asset);
        }

        /// <summary>
        /// 异步从路径加载AssetBundle
        /// </summary>
        public IEnumerator WebRequestLoadAssetBundle(string fileName, LoadAsyncComplete complete)
        {
            FileData data = new FileData("AssetBundle");
            string filePath = ResourceUtils.GetAssetsPathFromRequest(fileName);
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(filePath);

            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return 0;

            if (string.IsNullOrEmpty(request.error))
                data.asset = DownloadHandlerAssetBundle.GetContent(request);
            else
                GLog.ErrorFormat(request.error);
            request.Dispose();

            if (complete != null)
                complete(data.asset);
        }

        /// <summary>
        /// 异步从路径加载贴图
        /// </summary>
        public IEnumerator WebRequestLoadTexture(string fileName, LoadAsyncComplete complete)
        {
            FileData data = new FileData("Texture");
            string filePath = ResourceUtils.GetAssetsPathFromRequest(fileName);
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(filePath);

            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return 0;

            if (string.IsNullOrEmpty(request.error))
                data.asset = DownloadHandlerTexture.GetContent(request);
            else
                GLog.ErrorFormat(request.error);
            request.Dispose();

            if (complete != null)
                complete(data.asset);
        }

        /// <summary>
        /// 异步从路径加载音效
        /// </summary>
        public IEnumerator WebRequestLoadAudioClip(string fileName, AudioType audioType, LoadAsyncComplete complete)
        {
            FileData data = new FileData("AudioClip");
            string filePath = ResourceUtils.GetAssetsPathFromRequest(fileName);
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(filePath, audioType);

            yield return request.SendWebRequest();
            while (!request.isDone)
                yield return 0;

            if (string.IsNullOrEmpty(request.error))
                data.asset = DownloadHandlerAudioClip.GetContent(request);
            else
                GLog.ErrorFormat(request.error);
            request.Dispose();

            if (complete != null)
                complete(data.asset);
        }

        /// <summary>
        /// 同步读取persistentDataPath路径下文本
        /// </summary>
        public string LoadFileText(string fileName)
        {
            string filePath = ResourceUtils.GetPersistentDataFilePath(fileName);
            if (File.Exists(filePath))
                return File.ReadAllText(filePath);
            return null;
        }

        /// <summary>
        /// 同步读取persistentDataPath路径下二进制数组
        /// </summary>
        public byte[] LoadFileBytes(string fileName)
        {
            string filePath = ResourceUtils.GetPersistentDataFilePath(fileName);
            if (File.Exists(filePath))
                return File.ReadAllBytes(filePath);
            return null;
        }

        /// <summary>
        /// by Rick
        /// 资源异步加载完毕前的回调，做一些辅助工作
        /// </summary>
        /// <param name="asset"></param>
        void LoadAsyncPreCompleteFn(string assetName, string bundleName, object asset)
        {
            statisticsFacade.OnGenericObjectLoaded(assetName, bundleName, asset as UnityEngine.Object);
        }


        /// <summary>
        /// by Rick
        /// 额外登记一部分重新实例化的对象
        /// 重新实例化出来的对象，它的贴图需要再次登记。因为主物体在实例化的时候没有检测到它的Renderer，贴图没有登记
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject InstantiatePrefab(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            statisticsFacade.PostGameObjectAwake(prefab);
            return obj;
        }

        /// <summary>
        /// 场景加载完毕，标记可以卸载资源
        /// </summary>
        public void PostSceneLoaded()
        {
            statisticsFacade.PostSceneLoaded(GC);
        }

        public void FreeMemory()
        {
            statisticsFacade.DisposeUnusedAssetsAndBundles();
            GC();
        }

        /// <summary>
        /// 摧毁某个对象前，执行一些清理方法
        /// </summary>
        public void PreDestroyObject(Object o)
        {
            statisticsFacade.PreDestroyObject(o);
        }

        /// <summary>
        /// 获取当前游戏是否还在运行，有可能已经被强杀了
        /// </summary>
        /// <returns></returns>
        public bool IsApplicationRunning()
        {
            if (_igame != null)
            {
                return _igame.IsApplicationRunning;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 卸载当前场景
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="scenename"></param>
        private void UnloadCurrentScene(int layer, string scenename, bool byLoading)
        {
            if (string.IsNullOrEmpty(scenename))
            {
                if (layer == 0)
                {
                    //0层表示默认层，作为整体加载删除的层，可以不带名字
                }
                else if (!byLoading)
                {
                    GLog.ErrorFormat("UnloadCurrentScene failed, scenename is empty but layer is {0}", layer);
                    return;
                }
            }
            KeyValuePair<string, string> previousScene;
            if (sceneNameByLayer.TryGetValue(layer, out previousScene))
            {
                if (!string.IsNullOrEmpty(previousScene.Value))
                {
                    if (layer == 0)
                    {
                        //如果是整体层，需要全部释放
                        foreach (var kv in sceneNameByLayer)
                        {
                            statisticsFacade.ReleaseSceneUsage(kv.Value.Key, kv.Value.Value);
                        }
                    }
                    else
                    {
                        statisticsFacade.ReleaseSceneUsage(previousScene.Key, previousScene.Value);
                    }
                }
                statisticsFacade.LogVerboseFormat("ResourceScene UnloadCurrentScene layer={0}, previoursSceneName={1}", layer, previousScene.Value);
                sceneNameByLayer.Remove(layer);
            }
            else
            {
                statisticsFacade.LogVerboseFormat("ResourceScene UnloadCurrentScene layer {0} not exist", layer);
            }
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="scenename"></param>
        /// <returns></returns>
        public AsyncOperation UnloadSceneAsync(int layer, string scenename)
        {
            UnloadCurrentScene(layer, scenename, false);
            return SceneManager.UnloadSceneAsync(scenename);
        }

        /// <summary>
        /// 同步加载当前场景
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="abname"></param>
        /// <param name="scenename"></param>
        public Scene LoadScene(int layer, string abname, string scenename)
        {
            UnloadCurrentScene(layer, string.Empty, true);

            if (string.IsNullOrEmpty(abname) || string.IsNullOrEmpty(scenename))
                return new Scene();

            sceneNameByLayer[layer] = new KeyValuePair<string, string>(abname, scenename);
            statisticsFacade.LogVerboseFormat("ResourceScene LoadScene success, layer={0}, abname={1}, scenename={1}", layer, abname, scenename);

            if (developMode)
            {
                // 记录Asset和Bundle的关系
                Scene scene = editorTools.LoadScene(abname, scenename, (layer == 0));
                if (Application.isPlaying)
                {
                    statisticsFacade.OnAssetbundleLoaded(abname);
                    statisticsFacade.AddSceneUsage(abname, scenename);
                }
                return scene;
            }
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);
            statisticsFacade.AddSceneUsage(abname, scenename);
            return SceneManager.LoadScene(scenename, new LoadSceneParameters((layer == 0) ? LoadSceneMode.Single : LoadSceneMode.Additive));
        }

        /// <summary>
        /// 异步加载当前场景
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="abname"></param>
        /// <param name="scenename"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsync(int layer, string abname, string scenename)
        {
            UnloadCurrentScene(layer, string.Empty, true);

            if (string.IsNullOrEmpty(abname) || string.IsNullOrEmpty(scenename))
                return null;

            sceneNameByLayer[layer] = new KeyValuePair<string, string>(abname, scenename);
            statisticsFacade.LogVerboseFormat("ResourceScene LoadSceneAsync success, layer={0}, abname={1}, scenename={1}", layer, abname, scenename);

            if (developMode)
            {
                if (Application.isPlaying)
                {
                    // 记录Asset和Bundle的关系
                    statisticsFacade.OnAssetbundleLoaded(abname);
                    statisticsFacade.AddSceneUsage(abname, scenename);
                    return editorTools.LoadSceneAsync(abname, scenename, (layer == 0));
                }
            }
            AssetBundleInfo abInfo = LoadAssetBundleInfo(abname, true);

            var asyncOperation = SceneManager.LoadSceneAsync(scenename, (layer == 0) ? LoadSceneMode.Single : LoadSceneMode.Additive);
            statisticsFacade.AddSceneUsage(abname, scenename);
            asyncOperation.allowSceneActivation = true;
            return asyncOperation;
        }

        public string GetScenePath(string sceneName)
        {
            return "scenes/" + sceneName.ToLower() + Const.AsseetBundleDefaultVariant;
        }

        /// <summary>
        /// 这步需要在Init之前被调用，因为Init里需要路径加载Manifest
        /// </summary>
        /// <param name="manifestDirectory"></param>
        /// <param name="assetbundleDirectory"></param>
        public void RedirectPriorityDirectory(string manifestDirectory, string assetbundleDirectory)
        {
            _redirectedManifestDirectory = manifestDirectory;
            _redirectedAssetbundleDirectory = assetbundleDirectory;
        }
    }
}