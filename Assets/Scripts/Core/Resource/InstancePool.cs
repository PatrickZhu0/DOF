using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDLL.Resource
{
    /// <summary>
    /// 实例池管理器
    /// </summary>
    public class InstancePool : Singleton<InstancePool>, IManager
    {
        /// <summary>
        /// 回收模板，以单个资源为模板的对象
        /// </summary>
        class RecycleTemplate: IEquatable<RecycleTemplate>
        {
            HashSet<int> _spawned = new HashSet<int>();
            Queue<GameObject> _recycled = new Queue<GameObject>();
            readonly GameObject _template;
            readonly RecycleGroup _group;
            readonly InstancePool _pool;
            readonly GameDLL.Resource.Statistics.StatisticsFacade _facade;

            public RecycleTemplate(GameObject gameObject, RecycleGroup group, InstancePool pool, GameDLL.Resource.Statistics.StatisticsFacade facade)
            {
                _template = gameObject;
                _group = group;
                _pool = pool;
                _facade = facade;
                _facade.AssetAddRef(_pool.GetAssetId(_template));
            }

            public void Dispose()
            {
                foreach (var inst in _recycled)
                {
                    _facade.PreDestroyObject(inst);
                    GameObject.DestroyImmediate(inst);
                }
                _recycled.Clear();
                if (_spawned.Count > 0)
                {
                    //池被释放时，它所产生的对象理应已经全部归还到池里了，走到这里说明有错误
                    GLog.ErrorFormat("RecycleTemplate.Dispose template {0}, name: {1}, still has spawned instance, count: {2}", _pool.GetAssetId(_template), _template.name, _spawned.Count);
                }

                _facade.AssetReleaseRef(_pool.GetAssetId(_template), false);
            }

            public GameObject Spawn()
            {
                int oldCount = _recycled.Count;
                if (oldCount > 0)
                {
                    GameObject spawned = _recycled.Dequeue();
                    int instId = _pool.GetInstanceId(spawned);
                    _spawned.Add(instId);
                    _pool.PreSpawned(spawned);
                    _facade.LogVerboseFormat("RecycleTemplate.Spawn {0} from _recycled {1} -> {2}, id={3}", _template.name, oldCount, _recycled.Count, instId);
                    return spawned;
                }
                else
                {
                    GameObject spawned = GameObject.Instantiate<GameObject>(_template);
                    int instId = _pool.GetInstanceId(spawned);
                    _spawned.Add(instId);
                    _pool.PrepareForPool(spawned);
                    _pool.PreSpawned(spawned);
                    _facade.LogVerboseFormat("RecycleTemplate.Spawn {0} newly, id={1}", _template.name, instId);
                    return spawned;
                }
            }

            public bool Recycle(GameObject recycling)
            {
                int instId = _pool.GetInstanceId(recycling);
                if (_spawned.Contains(instId))
                {
                    int oldCount = _recycled.Count;
                    _pool.PreRecycling(recycling);
                    _spawned.Remove(instId);
                    _recycled.Enqueue(recycling);
                    _pool.PostRecycled(recycling);
                    _facade.LogVerboseFormat("RecycleTemplate.Recycle {0} from _recycled {1} -> {2}, id={3}", _template.name, oldCount, _recycled.Count, instId);
                    return true;
                }
                else
                {
                    GLog.ErrorFormat("RecycleTemplate.Recycle error, {0} is not spawned from this template {1}, id={2}", recycling.name, _template.name, instId);
                    return false;
                }
            }

            public bool Equals(RecycleTemplate other)
            {
                return _pool.GetAssetId(_template) == _pool.GetAssetId(other._template);
            }

            public void TrimFirst()
            {
                _facade.Assert(_recycled.Count > 0, "TrimFirst failed, _recycled.Count=0, _template.name={0}, templateId={1}", _template.name, _pool.GetAssetId(_template));
                GameObject trimmed = _recycled.Dequeue();
                _facade.PreDestroyObject(trimmed);
                GameObject.DestroyImmediate(trimmed);
            }

            public void TrimAll()
            {
                foreach (var inst in _recycled)
                {
                    _facade.PreDestroyObject(inst);
                    GameObject.DestroyImmediate(inst);
                }
                _recycled.Clear();
            }
        }


        /// <summary>
        /// 回收组，管理整体释放
        /// </summary>
        class RecycleGroup
        {
            Transform _groupRootTransform;
            InstancePool _pool;
            GameDLL.Resource.Statistics.StatisticsFacade _facade;
            Dictionary<int, RecycleTemplate> _templates = new Dictionary<int, RecycleTemplate>();
            HashSet<GameDLL.Resource.Statistics.GameObjectUsageBehaviour> noneABUsageBehaviour = null;
            readonly int _recycleLimitSize;
            List<RecycleTemplate> _fifoRecyclingTemplates = new List<RecycleTemplate>();
            readonly string _name;

            private bool LimitEnabled
            {
                get { return _recycleLimitSize > 0; }
            }

            public RecycleGroup(InstancePool pool, Transform groupRootTransform, GameDLL.Resource.Statistics.StatisticsFacade facade, int recycleLimitSize, string name)
            {
                _pool = pool;
                _groupRootTransform = groupRootTransform;
                _facade = facade;
                _recycleLimitSize = recycleLimitSize;
                _name = name;
            }

            public void Dispose()
            {
                _facade.LogVerboseFormat("RecycleGroup.Dispose {0}", _name);
                foreach(var template in _templates.Values)
                {
                    template.Dispose();
                }
                _templates.Clear();
                if (noneABUsageBehaviour != null)
                {
                    foreach (var manuallyCreatedBehaviour in noneABUsageBehaviour)
                    {
                        GameObject.DestroyImmediate(manuallyCreatedBehaviour, true);
                    }
                    noneABUsageBehaviour.Clear();
                }
                _fifoRecyclingTemplates.Clear();
                if(_groupRootTransform)
                {
                    GameObject.DestroyImmediate(_groupRootTransform.gameObject);
                    _groupRootTransform = null;
                }
            }

            public void TrimAll()
            {
                _facade.LogVerboseFormat("RecycleGroup.TrimAll {0}", _name);
                foreach (var template in _templates.Values)
                {
                    template.TrimAll();
                }
            }

            public void EnsureTemplate(GameObject template)
            {
                _facade.LogVerboseFormat("EnsureTemplate template={0}, id={1}", template.name, _pool.GetAssetId(template));
                int assetId = _pool.GetAssetId(template);
                if (!_templates.ContainsKey(assetId))
                {
                    _templates.Add(assetId, new RecycleTemplate(template, this, _pool, _facade));
                }
            }

            public void EnsureNoneABAsset(GameObject template)
            {
                GameDLL.Resource.Statistics.GameObjectUsageBehaviour usageBehaviour = template.GetComponent<GameDLL.Resource.Statistics.GameObjectUsageBehaviour>();
                if(usageBehaviour==null)
                {
                    usageBehaviour = template.AddComponent<GameDLL.Resource.Statistics.GameObjectUsageBehaviour>();
                    usageBehaviour.CacheAssetId(ResourceManager.instance.StatisticsFacade);
                    if(noneABUsageBehaviour==null)
                    {
                        noneABUsageBehaviour = new HashSet<Statistics.GameObjectUsageBehaviour>();
                    }
                    noneABUsageBehaviour.Add(usageBehaviour);
                }
            }

            public GameObject Spawn(int assetId)
            {
                RecycleTemplate template = null;
                if (!_templates.TryGetValue(assetId, out template))
                {
                    GLog.ErrorFormat("RecycleGroup.Spawn failed, assetId {0} not exist, _name={1}", assetId, _name);
                    return null;
                }
                if(LimitEnabled)
                {
                    //remove it from fifo
                    IEnumerator<RecycleTemplate> e = _fifoRecyclingTemplates.GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (e.Current.Equals(template))
                        {
                            _fifoRecyclingTemplates.Remove(e.Current);
                            break;
                        }
                    }
                }
                return template.Spawn();
            }

            public bool Recycle(GameObject recycling)
            {
                int assetId = _pool.GetInstancedAssetId(recycling);
                RecycleTemplate template = null;
                if (!_templates.TryGetValue(assetId, out template))
                {
                    GLog.ErrorFormat("RecycleGroup.Recycle failed, assetId {0} not exist", assetId);
                    return false;
                }
                bool res = template.Recycle(recycling);
                if(res)
                {
                    recycling.transform.SetParent(_groupRootTransform);
                    _fifoRecyclingTemplates.Add(template);
                    if (LimitEnabled)
                    {
                        if (_fifoRecyclingTemplates.Count > _recycleLimitSize)
                        {
                            //销毁一个
                            RecycleTemplate trimmingTemplate = _fifoRecyclingTemplates[0];
                            _fifoRecyclingTemplates.RemoveAt(0);
                            trimmingTemplate.TrimFirst();
                            //确保只需要销毁一个
                            _facade.Assert(_fifoRecyclingTemplates.Count == _recycleLimitSize, "RecycleGroup.Recycle error after trimming, _fifoRecyclingTemplates.Count={0}, _recycleLimitSize={1}", _fifoRecyclingTemplates.Count, _recycleLimitSize);
                        }
                    }
                }
                return res;
            }
        }

        public const string RECYCLED_ROOT_NAME = "RecycledRoot";
        Transform _recycledRootTransform = null;
        private IGame _igame;
        private Dictionary<string, RecycleGroup> groupDic = new Dictionary<string, RecycleGroup>();
        GameDLL.Resource.Statistics.StatisticsFacade _facade;
        Dictionary<string, Dictionary<string, GameObject>> _bundleAndAssetNameToAsset = new Dictionary<string, Dictionary<string, GameObject>>();
        Dictionary<int, GameObject> _idToAsset = new Dictionary<int, GameObject>();
        Dictionary<int, int> _comIdToGameObjectId = new Dictionary<int, int>();
        const string PERSISTENT_GROUP_NAME = "PersistentGroup";
        private string uniquePersistentGroupName = string.Empty;
        const string SCENE_BASED_GROUP_NAME = "SceneGroup";
        private string uniqueSceneBasedGroupName = string.Empty;
        public delegate void CustomResetPoolInstance(GameObject instance);
        private CustomResetPoolInstance _customResetPoolInstance=null;

        #region 管理器接口函数
        public void Init(IGame game)
        {
            _igame = game;
        }

        public void Reset()
        {

        }

        public void Dispose()
        {
            foreach(var group in groupDic.Values)
            {
                group.Dispose();
            }
            groupDic.Clear();
            uniqueSceneBasedGroupName = string.Empty;
            uniquePersistentGroupName = string.Empty;
        }
        #endregion

        #region 初始化
        public void InitMono(MonoBehaviour mono, GameDLL.Resource.Statistics.StatisticsFacade facade, CustomResetPoolInstance customResetPoolInstance)
        {
            _facade = facade;
            _customResetPoolInstance = customResetPoolInstance;
            GameObject root;
            if (mono)
            {
                root = mono.gameObject;
                _recycledRootTransform = mono.transform.Find(RECYCLED_ROOT_NAME);
                if (!_recycledRootTransform)
                {
                    GameObject go = new GameObject(RECYCLED_ROOT_NAME);
                    go.transform.SetParent(mono.transform);
                    _recycledRootTransform = go.transform;
                    go.SetActive(false);
                    _facade.LogVerboseFormat("InstancePool.InitMono _recycledRootTransform created successfully");
                }
                else
                {
                    _facade.LogVerboseFormat("InstancePool.InitMono using existing _recycledRootTransform");
                }

                CreatePersistentGroup();
            }
            else
            {
                GLog.Error("InstancePool.InitMono failed, input mono is null");
            }
        }
        #endregion

        #region 组管理

        const string EditorGroupSubName = "Editor";

        /// <summary>
        /// 创建一个组
        /// </summary>
        /// <param name="usageName"></param>
        /// <returns></returns>
        public string CreateGroup(string usageName, int recycleLimitSize)
        {
            //string timeStamp = DateTime.Now.ToString();
            string subname = null;
            if (Application.isPlaying)
                subname = DateTime.Now.ToString();
            else
                subname = EditorGroupSubName;
            string uniqueName = string.Format("IPUID_{0}_{1}", usageName, subname);
            if (groupDic.ContainsKey(uniqueName))
            {
                GLog.ErrorFormat("InstancePool.CreateGroup, Contains Group {0}", uniqueName);
                return uniqueName;
            }
            if (!Application.isPlaying)
            {
                Transform trans = _recycledRootTransform.Find(uniqueName);
                if (trans)
                {
                    RecycleGroup gp = new RecycleGroup(this, trans, _facade, recycleLimitSize, uniqueName);
                    groupDic.Add(uniqueName, gp);
                    _facade.LogVerboseFormat("InstancePool.CreateGroup [In Editor] success, {0}", uniqueName);
                    return uniqueName;
                }
            }
            GameObject go = new GameObject(uniqueName);
            go.transform.SetParent(_recycledRootTransform);
            RecycleGroup group = new RecycleGroup(this, go.transform, _facade, recycleLimitSize, uniqueName);
            groupDic.Add(uniqueName, group);
            _facade.LogVerboseFormat("InstancePool.CreateGroup success, {0}", uniqueName);
            return uniqueName;
        }

        /// <summary>
        /// 删除一个组
        /// </summary>
        /// <param name="uniqueName"></param>
        public void DestroyGroup(string uniqueName, bool force)
        {
            if(_igame.IsApplicationRunning || force)
            {
                RecycleGroup group = null;
                if (groupDic.TryGetValue(uniqueName, out group))
                {
                    group.Dispose();
                    groupDic.Remove(uniqueName);
                    _facade.LogVerboseFormat("InstancePool.DestroyGroup success: {0}", uniqueName);
                }
                else
                {
                    GLog.ErrorFormat("InstancePool.DestroyGroup failed, {0} not exist in groupDic", uniqueName);
                }
            }
            else
            {
                //走到这里是在强杀进程，不用处理了
            }
        }

        private void CreatePersistentGroup()
        {
            uniquePersistentGroupName = CreateGroup(PERSISTENT_GROUP_NAME, 0);
        }

        public void DestroySceneBasedGroup()
        {
            if(!string.IsNullOrEmpty(uniqueSceneBasedGroupName))
            {
                DestroyGroup(uniqueSceneBasedGroupName, false);
            }
        }

        public void DestroySceneBasedGroupEditorMode()
        {
            if (!string.IsNullOrEmpty(uniqueSceneBasedGroupName))
            {
                DestroyGroup(uniqueSceneBasedGroupName, true);
            }
        }

        public void CreateSceneBasedGroup()
        {
            uniqueSceneBasedGroupName = CreateGroup(SCENE_BASED_GROUP_NAME, 0);
        }
#endregion

#region 实例类
        public int GetAssetId(UnityEngine.Object gameObject)
        {
            _facade.Assert(gameObject != null, "GetAssetId failed, gameObject is null");
            return gameObject.GetInstanceID();
        }

        public int GetInstancedAssetId(GameObject gameObject)
        {
            _facade.Assert(gameObject != null, "GetInstancedAssetId failed, gameObject is null");
            GameDLL.Resource.Statistics.GameObjectUsageBehaviour usageBehaviour = gameObject.GetComponent<GameDLL.Resource.Statistics.GameObjectUsageBehaviour>();
            _facade.Assert(usageBehaviour != null, "GetInstancedAssetId failed, usageBehaviour is null, name={0}", gameObject.name);
            return usageBehaviour.GetCachedAssetId();
        }

        public int GetInstanceId(GameObject gameObject)
        {
            _facade.Assert(gameObject != null, "GetInstanceId failed, gameObject is null");
            return gameObject.GetInstanceID();
        }


        private bool _TemplateExist(string abname, string assetname, out GameObject template)
        {
            Dictionary<string, GameObject> nameToAsset = null;
            if (_bundleAndAssetNameToAsset.TryGetValue(abname, out nameToAsset))
            {
                if (nameToAsset.TryGetValue(assetname, out template))
                {
                    if (template)
                    {
                        return true;
                    }
                    else
                    {
                        //GameObject已经因为引用计数到0被Unity回收了，则这里要清理注册表
                        _facade.LogVerboseFormat("_TemplateExist abname={0}, assetname={1}, gameObject is already disposed by Unity", abname, assetname);
                        nameToAsset.Remove(assetname);
                        template = null;
                        return false;
                    }
                }
            }
            template = null;
            return false;
        }

        private void RegisterTemplate(string abname, string assetname, GameObject template)
        {
            _facade.LogVerboseFormat("RegisterTemplate abname={0}, assetname={1}, template={2}, id={3}", abname, assetname, template.name, GetAssetId(template));
            Dictionary<string, GameObject> nameToAsset = null;
            if (!_bundleAndAssetNameToAsset.TryGetValue(abname, out nameToAsset))
            {
                nameToAsset = new Dictionary<string, GameObject>();
                _bundleAndAssetNameToAsset.Add(abname, nameToAsset);
            }
            _facade.Assert(!nameToAsset.ContainsKey(assetname), "RegisterTemplate failed, abname {0} assetname {1} already registered", abname, assetname);
            nameToAsset.Add(assetname, template);
        }


        public GameObject SpawnGameObject(string abname, string assetname, string uniqueGroupName)
        {
            RecycleGroup group = null;
            if (groupDic.TryGetValue(uniqueGroupName, out group))
            {
                GameObject template = null;
                if (!_TemplateExist(abname, assetname, out template))
                {
                    template = GameDLL.ResourceManager.instance.LoadGameObject(abname, assetname);
                    RegisterTemplate(abname, assetname, template);
                }
                group.EnsureTemplate(template);
                int assetId = GetAssetId(template);
                return group.Spawn(assetId);
            }
            else
            {
                GLog.ErrorFormat("InstancePool.SpawnGameObject failed, {0} not exist in groupDic", uniqueGroupName);
                return null;
            }
        }

        public IEnumerator SpawnGameObjectAsync(string abname, string assetname, string uniqueGroupName, GameDLL.ResourceManager.LoadAsyncComplete complete)
        {
            RecycleGroup group = null;
            if (groupDic.TryGetValue(uniqueGroupName, out group))
            {
                GameObject template = null;
                if (!_TemplateExist(abname, assetname, out template))
                {
                    _facade.LogVerboseFormat("SpawnGameObjectAsync will LoadGameObjectAsync abname={0}, assetname={1}", abname, assetname);
                    yield return GameDLL.ResourceManager.Instance.LoadGameObjectAsync(abname, assetname, (obj) => {
                        _facade.Assert((obj is GameObject), "SpawnGameObjectAsync failed, result object is not a GameObject, abname={0}, assetname={1}", abname, assetname);
                        template = obj as GameObject;
                        RegisterTemplate(abname, assetname, template);

                        group.EnsureTemplate(template);
                        int assetId = GetAssetId(template);
                        GameObject output = group.Spawn(assetId);
                        if (complete != null)
                        {
                            complete(output);
                        }
                    });
                }
                else
                {
                    _facade.LogVerboseFormat("SpawnGameObjectAsync already exist abname={0}, assetname={1}", abname, assetname);
                    if (complete != null)
                    {
                        group.EnsureTemplate(template);
                        int assetId = GetAssetId(template);
                        GameObject output = group.Spawn(assetId);
                        complete(output);
                    }
                }
            }
            else
            {
                GLog.ErrorFormat("InstancePool.SpawnGameObjectAsync failed, {0} not exist in groupDic", uniqueGroupName);
            }
        }



        private int AddManuallyLoadedGameObject(GameObject template)
        {
            if(template)
            {
                int assetId = GetAssetId(template);
                if (!_idToAsset.ContainsKey(assetId))
                {
                    _idToAsset.Add(assetId, template);
                }
                return assetId;
            }
            return 0;
        }

        public int AddGenericManuallyLoadedObject(UnityEngine.Object template)
        {
            _facade.Assert(template != null, "AddGenericManuallyLoadedObject failed, template is null");
            if(template is GameObject)
            {
                AddManuallyLoadedGameObject(template as GameObject);
            }
            else if(template is Component)
            {
                Component com = template as Component;
                int comId = GetAssetId(com);
                GameObject comOwner = com.gameObject;
                _facade.Assert(comOwner != null, "AddGenericManuallyLoadedObject failed, comOwner is null for object: {0}, name={1}", comId, com.name);
                int ownerId = GetAssetId(comOwner);
                if (!_comIdToGameObjectId.ContainsKey(comId))
                {
                    _comIdToGameObjectId.Add(comId, ownerId);
                }
                AddManuallyLoadedGameObject(comOwner);
            }
            return GetAssetId(template);
        }

        private bool RemoveManuallyLoadedGameObject(GameObject template)
        {
            if(template)
            {
                int assetId = GetAssetId(template);
                if (_idToAsset.ContainsKey(assetId))
                {
                    return _idToAsset.Remove(assetId);
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool RemoveGenericManuallyLoadedObject(UnityEngine.Object template)
        {
            _facade.Assert(template != null, "RemoveGenericManuallyLoadedObject failed, template is null");
            if(template is GameObject)
            {
                return RemoveManuallyLoadedGameObject(template as GameObject);
            }
            else if(template is Component)
            {
                Component com = template as Component;
                int comId = GetAssetId(com);
                int ownerId = 0;
                if(_comIdToGameObjectId.TryGetValue(comId, out ownerId))
                {
                    GameObject owner = null;
                    if(_idToAsset.TryGetValue(ownerId, out owner))
                    {
                        return RemoveManuallyLoadedGameObject(owner);
                    }
                    else
                    {
                        GLog.ErrorFormat("RemoveGenericManuallyLoadedObject failed, component cannot find its registered gameObject, comId={0}, comName={1}, ownerId={2}", comId, template.name, ownerId);
                    }
                }
                else
                {
                    GLog.ErrorFormat("RemoveGenericManuallyLoadedObject failed, component is not registered, id={0}, name={1}", comId, template.name);
                }
            }
            return false;
        }

        public GameObject SpawnManuallyLoadedGameObject(int assetId, string uniqueGroupName)
        {
            RecycleGroup group = null;
            if (groupDic.TryGetValue(uniqueGroupName, out group))
            {
                GameObject template = null;
                int realGameObjectId = 0;
                if (_comIdToGameObjectId.TryGetValue(assetId, out realGameObjectId))
                {

                }
                else
                {
                    realGameObjectId = assetId;
                }
                if (_idToAsset.TryGetValue(realGameObjectId, out template))
                {
                    group.EnsureNoneABAsset(template);      //只有这里会绕过ResourceMananger加载prefab，只需要在这里加
                    group.EnsureTemplate(template);
                    return group.Spawn(realGameObjectId);
                }                
                GLog.ErrorFormat("InstancePool.SpawnManuallyLoadedGameObject failed, {0} not exist in _idToAsset", assetId);
                return null;
            }
            else
            {
                GLog.ErrorFormat("InstancePool.SpawnManuallyLoadedGameObject failed, {0} not exist in groupDic", uniqueGroupName);
                return null;
            }
        }

        public bool Recycle(GameObject recycling, string uniqueGroupName)
        {
            if(_igame.IsApplicationRunning)   //有可能在杀进程的时候，从Lua呼叫到这里，所以拦截一下
            {
                RecycleGroup group = null;
                if (groupDic.TryGetValue(uniqueGroupName, out group))
                {
                    return group.Recycle(recycling);
                }
                else
                {
                    GLog.ErrorFormat("InstancePool.Recyle failed, {0} not exist in groupDic", uniqueGroupName);
                    return false;
                }
            }
            return true;
        }

        private void PrepareForPool(GameObject instance)
        {
            _facade.Assert(instance != null, "GetInstancedAssetId failed, gameObject is null");
            GameDLL.Resource.Statistics.GameObjectUsageBehaviour usageBehaviour = instance.GetComponent<GameDLL.Resource.Statistics.GameObjectUsageBehaviour>();
            _facade.Assert(usageBehaviour != null, "GetInstancedAssetId failed, usageBehaviour is null, name={0}", instance.name);
            usageBehaviour.PrepareForPool();
        }

        private void PreSpawned(GameObject instance)
        {
            _facade.Assert(instance != null, "GetInstancedAssetId failed, gameObject is null");
            GameDLL.Resource.Statistics.GameObjectUsageBehaviour usageBehaviour = instance.GetComponent<GameDLL.Resource.Statistics.GameObjectUsageBehaviour>();
            _facade.Assert(usageBehaviour != null, "GetInstancedAssetId failed, usageBehaviour is null, name={0}", instance.name);
            usageBehaviour.ResetFromPool();
            if(_customResetPoolInstance!=null)
            {
                _customResetPoolInstance(instance);
            }
        }

        private void PreRecycling(GameObject recycling)
        {
            _facade.PreRecyclingObject(recycling);
        }

        private void PostRecycled(GameObject recycling)
        {

        }

        public void TrimAll()
        {
            foreach (var group in groupDic.Values)
            {
                group.TrimAll();
            }
        }

        #endregion

        #region 快速方法
        public GameObject SpawnGameObjectPersistent(string abname, string assetname)
        {
            return SpawnGameObject(abname, assetname, uniquePersistentGroupName);
        }

        public GameObject SpawnManuallyLoadedGameObjectPersistent(int assetId)
        {
            return SpawnManuallyLoadedGameObject(assetId, uniquePersistentGroupName);
        }

        public void RecyclePersistent(GameObject recycling)
        {
            Recycle(recycling, uniquePersistentGroupName);
        }

        public GameObject SpawnGameObjectSceneBased(string abname, string assetname)
        {
            return SpawnGameObject(abname, assetname, uniqueSceneBasedGroupName);
        }

        public GameObject SpawnManuallyLoadedGameObjectSceneBased(int assetId)
        {
            return SpawnManuallyLoadedGameObject(assetId, uniqueSceneBasedGroupName);
        }

        public void RecycleSceneBased(GameObject recycling)
        {
            Recycle(recycling, uniqueSceneBasedGroupName);
        }

        public GameObject SpawnTimelineInstance(int assetId)
        {
            return SpawnManuallyLoadedGameObjectSceneBased(assetId);
        }

        public void RecycleTimelineInstance(GameObject recycling)
        {
            RecycleSceneBased(recycling);
        }

#endregion
    }
}
