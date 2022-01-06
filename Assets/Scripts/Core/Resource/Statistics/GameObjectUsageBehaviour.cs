using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 实例对象计数统计辅助类
    /// </summary>
    public class GameObjectUsageBehaviour : MonoBehaviour
    {
        [SerializeField]      //每次加载完后重新取值，不需要序列化，因为只要保证和自己的InstanceId一致就行了。by Rick，2021年10月27日
        int _cachedAssetId = 0;
        /// <summary>
        /// 池对象需要重置操作的组件
        /// </summary>
        ParticleSystem[] particleSystems = null;

        [Serializable]
        public struct TextureReferenceCache
        {
            public int instanceId;
            public string name;
            public Texture texture;
        }
        [SerializeField]
        TextureReferenceCache[] textureReferences;
        public TextureReferenceCache[] TextureReferences
        {
            get
            {
                return textureReferences;
            }
        }
        [SerializeField]
        bool textureReferenceCached = false;
        public bool TextureReferenceCached
        {
            get
            {
                return textureReferenceCached;
            }
        }

        public void CacheAssetId(StatisticsFacade statisticsFacade)
        {
            _cachedAssetId = gameObject.GetInstanceID();
            ResourceManager.Instance.StatisticsFacade.LogVerboseFormat("GameObjectUsageBehaviour.CacheAssetId, assetId={0}, name={1}", _cachedAssetId, name);
        }

        public int GetCachedAssetId()
        {
            return _cachedAssetId;
        }

        bool awakeCalled = false;

        public bool AwakeCalled
        {
            get
            {
                return awakeCalled;
            }
        }

        void Awake()
        {
            awakeCalled = true;
            int id = GetCachedAssetId();
            ResourceManager.Instance.StatisticsFacade.LogVerboseFormat("GameObjectUsageBehaviour.Awake, assetId={0}, name={1}, instId={2}", id, name, GetInstanceID());
            ResourceManager.Instance.StatisticsFacade.AssetAddRef(id);

            //to do... 需要做工具检查prefab是否有inactive的状态。
            //如果这个prefab被实例化后没有被激活过，这会引起图片的InitImageComponents和ReleaseImageComponents不配对。
            //因为PreDestroy是手动调用的，可能没有经过Awake
            ResourceManager.Instance.StatisticsFacade.PostGameObjectAwake(gameObject);
        }

        void OnDestroy()
        {
            if(ResourceManager.Instance.IsApplicationRunning())
            {
                int id = GetCachedAssetId();
                ResourceManager.Instance.StatisticsFacade.LogVerboseFormat("GameObjectUsageBehaviour.OnDestroy, assetId={0}, name={1}, instId={2}", id, name, GetInstanceID());
                ResourceManager.Instance.StatisticsFacade.AssetReleaseRef(id, false);
            }
        }

        public void PrepareForPool()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public void ResetFromPool()
        {
            foreach(var p in particleSystems)
            {
                p.Simulate(0);
                p.Play();
            }
        }

        /// <summary>
        /// 缓存贴图ID
        /// </summary>
        /// <returns>是否有差异</returns>
        internal bool CacheTextureReferences()
        {
            HashSet<int> oldSet = new HashSet<int>();
            if(textureReferences!=null)
            {
                foreach (var cOld in textureReferences)
                {
                    oldSet.Add(cOld.instanceId);
                }
            }
            bool oldCached = textureReferenceCached;

            HashSet<int> newSet = new HashSet<int>();
            List<TextureReferenceCache> cacheList = new List<TextureReferenceCache>();
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                foreach(var sm in renderer.sharedMaterials)
                {
                    if (sm)
                    {
                        var texIds = sm.GetTexturePropertyNameIDs();
                        foreach (var texId in texIds)
                        {
                            var texture = sm.GetTexture(texId);
                            if (texture != null)
                            {
                                TextureReferenceCache cache = new TextureReferenceCache();
                                cache.instanceId = texture.GetInstanceID();
                                cache.name = texture.name;
                                cache.texture = texture;
                                cacheList.Add(cache);
                                newSet.Add(cache.instanceId);
                            }
                        }
                    }
                }
                /*
                */
            }
            textureReferences = cacheList.ToArray();
            textureReferenceCached = true;
            if (!oldCached)
                return true;
            bool same = oldSet.SetEquals(newSet);
            return !same;
        }
    }
}
