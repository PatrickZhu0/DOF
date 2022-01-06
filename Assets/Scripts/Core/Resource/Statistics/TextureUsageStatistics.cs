using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 贴图类型资源的计数统计
    /// </summary>
    public class TextureUsageStatistics
    {
        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// 所有贴图的内部计数统计，其实可以不用
        /// </summary>
        Dictionary<int, int> usageDic = new Dictionary<int, int>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public TextureUsageStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="texture"></param>
        void AddRef(Texture texture)
        {
            int instId = texture.GetInstanceID();
            int currentCount = 0;
            if (usageDic.TryGetValue(instId, out currentCount))
            {
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("TextureUsageStatistics.AddRef, Sprite count error: count < 0 on AddRef: {0}, {1}", texture.name, currentCount);
                }
            }
            int nextCount = currentCount + 1;
            usageDic[instId] = nextCount;
            _facade.LogVerboseFormat("TextureUsageStatistics.AddRef, {0}, id={1}, count {2} -> {3}", texture.name, instId, currentCount, nextCount);
            //呼叫真正的计数
            _facade.AssetAddRef(instId);
        }

        void AddCacheRef(GameObjectUsageBehaviour.TextureReferenceCache r)
        {
            if(!r.texture)
            {
                GLog.ErrorFormat("AddCacheRef failed, r.texture is null, r.instanceId={0}, r.name={1}", r.instanceId, r.name);
                return;
            }
            AddRef(r.texture);
            /*
            int instId = r.instanceId;
            int currentCount = 0;
            if (usageDic.TryGetValue(instId, out currentCount))
            {
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("TextureUsageStatistics.AddRef, Sprite count error: count < 0 on AddRef: {0}, {1}", r.name, currentCount);
                }
            }
            int nextCount = currentCount + 1;
            usageDic[instId] = nextCount;
            _facade.LogVerboseFormat("TextureUsageStatistics.AddRef, {0}, id={1}, count {2} -> {3}", r.name, instId, currentCount, nextCount);
            //呼叫真正的计数
            _facade.AssetAddRef(instId);
            */
        }

        /// <summary>
        /// 释放引用
        /// </summary>
        /// <param name="texture"></param>
        void ReleaseRef(Texture texture)
        {
            int instId = texture.GetInstanceID();
            if (usageDic.ContainsKey(instId))
            {
                int oldCount = usageDic[instId];
                int currentCount = --usageDic[instId];
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("Texture count error: count < 0 on ReleaseRef: {0}, {1}, instId={2}", texture.name, currentCount, instId);
                }
                _facade.LogVerboseFormat("TextureUsageStatistics.ReleaseRef, {0}, id={1}, count {2} -> {3}", texture.name, instId, oldCount, currentCount);
            }
            else
            {
                GLog.ErrorFormat("TextureUsageStatistics.ReleaseRef error: {0} not exist in usageDic, instId={1}", texture.name, instId);
            }
            //呼叫真正的计数
            _facade.AssetReleaseRef(instId, true);
        }

        /// <summary>
        /// 监视GameObject使用的的初始化方法，因为除了RawImage外，Renderer的Material也会手动使用Texture，而Renderer没有Lua层对应的接口，需要在这里初始化
        /// </summary>
        /// <param name="gameObject"></param>
        public void InitTextureComponents(GameObject gameObject)
        {
            GameObjectUsageBehaviour usage = gameObject.GetComponent<GameObjectUsageBehaviour>();
            if(false && usage && usage.TextureReferenceCached)
            {
                _facade.LogVerboseFormat("TextureUsageStatistics.InitImageComponents, found {0} Renderers for {1}", (usage.TextureReferences == null) ? 0 : usage.TextureReferences.Length, gameObject.name);
                foreach (var r in usage.TextureReferences)
                {
                    AddCacheRef(r);
                }
            }
            else
            {
                Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
                //GameDLL.GLog.ErrorFormat("supposed to be cached but not!!! name={0}", gameObject.name);
                _facade.LogVerboseFormat("TextureUsageStatistics.InitImageComponents, found {0} Renderers for {1}", (renderers == null) ? 0 : renderers.Length, gameObject.name);
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterial)
                    {
                        var texIds = renderer.sharedMaterial.GetTexturePropertyNameIDs();
                        foreach (var texId in texIds)
                        {
                            var texture = renderer.sharedMaterial.GetTexture(texId);
                            if (texture != null)
                            {
                                _facade.LogVerboseFormat("InitTextureComponents gameObject={0}, id={1}, texture={2}, textureId={3}, renderer.gameObject={4}, renderer.gameObject.Id={5}", gameObject.name, gameObject.GetInstanceID(), texture.name, texture.GetInstanceID(), renderer.gameObject.name, renderer.gameObject.GetInstanceID());
                                AddRef(texture);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 监视GameObject使用的释放方法，原因同上
        /// </summary>
        /// <param name="gameObject"></param>
        public void ReleaseTextureComponents(GameObject gameObject)
        {
            //GameObjectUsageBehaviour usage = gameObject.GetComponent<GameObjectUsageBehaviour>();
            //if(usage==null)
            //{
            //    GameDLL.GLog.ErrorFormat("ReleaseTextureComponents error, GameObjectUsageBehaviour not exist, instId={0}, name={1}", gameObject.GetInstanceID(), gameObject.name);
            //}
            //else if(!usage.AwakeCalled)
            //{
            //    GameDLL.GLog.ErrorFormat("ReleaseTextureComponents error, GameObjectUsageBehaviour Awake not called, instId={0}, name={1}", gameObject.GetInstanceID(), gameObject.name);
            //}
            //不能缓存InitTextureComponents时的renderer表，因为物体层级关系可能会改变，删除时的和加载时的不一定一致
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterial)
                {
                    var texIds = renderer.sharedMaterial.GetTexturePropertyNameIDs();
                    foreach (var texId in texIds)
                    {
                        var texture = renderer.sharedMaterial.GetTexture(texId);
                        if (texture != null)
                        {
                            _facade.LogVerboseFormat("ReleaseTextureComponents gameObject={0}, id={1}, texture={2}, textureId={3}, renderer.gameObject={4}, renderer.gameObject.Id={5}, rootGameObject={6}", gameObject.name, gameObject.GetInstanceID(), texture.name, texture.GetInstanceID(), renderer.gameObject.name, renderer.gameObject.GetInstanceID(), gameObject.name);
                            ReleaseRef(texture);
                        }
                    }
                }
            }
            _facade.LogVerboseFormat("TextureUsageStatistics.ReleaseTextureComponents, found {0} Renderers for {1}", (renderers == null) ? 0 : renderers.Length, gameObject.name);
        }

        /// <summary>
        /// 对应用层暴露的统一设置材质贴图的方法
        /// </summary>
        /// <param name="material"></param>
        /// <param name="name"></param>
        /// <param name="texture"></param>
        public void SetMaterialTexture(Material material, string name, Texture texture)
        {
            _facade.Assert(material != null, "TextureUsageStatistics.SetMaterialTexture failed, material is null");
            Texture oldTexture = material.GetTexture(name);
            _facade.LogVerboseFormat("SetMaterialTexture material.name={0}, name={1}, texture.name={2}, texture.Id={3}", material.name, name, texture.name, texture.GetInstanceID());
            if(oldTexture!=null)
            {
                ReleaseRef(oldTexture);
            }
            material.SetTexture(name, texture);
            if(texture!=null)
            {
                AddRef(texture);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一设置RawImage的方法
        /// </summary>
        /// <param name="rawImage"></param>
        /// <param name="texture"></param>
        public void SetRawImage(RawImage rawImage, Texture texture)
        {
            _facade.Assert(rawImage != null, "TextureUsageStatistics.SetRawImage failed, rawImage is null");
            Texture oldTexture = rawImage.texture;
            if(oldTexture!=null)
            {
                ReleaseRef(oldTexture);
            }
            rawImage.texture = texture;
            if(texture!=null)
            {
                AddRef(texture);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一初始化RawImage方法
        /// </summary>
        /// <param name="image"></param>
        public void OnRawImageCreate(RawImage image)
        {
            if (image.texture)
            {
                AddRef(image.texture);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一释放RawImage方法
        /// </summary>
        /// <param name="image"></param>
        public void OnRawImageDestroy(RawImage image)
        {
            if (image.texture)
            {
                ReleaseRef(image.texture);
            }
        }

        /// <summary>
        /// 除RawImage以外的贴图引用
        /// </summary>
        /// <param name="texture"></param>
        public void AddCustomTextureRef(Texture texture)
        {
            AddRef(texture);
        }

        /// <summary>
        /// 除RawImage以外的贴图引用
        /// </summary>
        /// <param name="texture"></param>
        public void ReleaseCustomTextureRef(Texture texture)
        {
            ReleaseRef(texture);
        }
    }
}
