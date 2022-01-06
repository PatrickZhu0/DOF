using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [ExecuteInEditMode]
    [AddComponentMenu("Material/MaterialsReplace")]
    public class MaterialsReplace : IMaterialsReplace
    {
        public int groupId;

        RenderersUpdater updater;

        protected override void SetInit()
        {
            if (updater == null)
                updater = RenderersUpdater.Create(_renderers, mpb, groupId);
            else
                updater.ResetMaterials();
        }

        void OnEnable()
        {
            if (!Application.isPlaying)
            {
                _init = false;
                Init();
            }
        }

        void OnValidate()
        {
            if (!Application.isPlaying)
            {
                _init = false;
                RevertMaterials();
            }
        }

        public void SetRenderersEnabled(bool enabled, int rFlag = -1)
        {
            if (updater != null)
                updater.SetRenderersEnabled(enabled, rFlag);
        }

        /// <summary>
        /// 重置Renderer是否渲染
        /// </summary>
        public void ResetRenderersEnable(int rFlag = -1)
        {

            if (updater != null)
                updater.ResetRenderersEnable(rFlag);
        }

        #region MaterialPropertyBlock

        /// <summary>
        /// 清除缓存的MaterialPropertyBlock
        /// </summary>
        public void ClearPropertyBlock()
        {
            if (updater != null)
                updater.ClearPropertyBlock();
        }

        public void UpdatePropertyBlock(PropertyBlockData data, float t)
        {
            Init();
            updater.UpdatePropertyBlock(data, t);
        }

        public void SetPropertyBlock(int nameID, int value)
        {
            Init();
            updater.SetPropertyBlock(nameID, value);
        }

        public void SetPropertyBlock(int nameID, float value)
        {
            Init();
            updater.SetPropertyBlock(nameID, value);
        }

        public void SetPropertyBlock(int nameID, Vector4 vectory)
        {
            Init();
            updater.SetPropertyBlock(nameID, vectory);
        }

        public void SetPropertyBlock(int nameID, Color color)
        {
            Init();
            updater.SetPropertyBlock(nameID, color);
        }

        /// <summary>
        /// Texture对象没有被脚本其它对象引用，那么此Texture对象是非持久的，会被Resources.UnloadUnusedAssets回收
        /// </summary>
        /// <param name="nameID"></param>
        /// <param name="texture"></param>
        public void SetPropertyBlock(int nameID, Texture texture)
        {
            Init();
            updater.SetPropertyBlock(nameID, texture);
        }

        /// <summary>
        /// 应用所有或指定材质的MaterialPropertyBlock
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ApplyPropertyBlock(int rFlag = -1, int mFlag = -1)
        {
            if (updater != null)
                updater.ApplyPropertyBlock(rFlag, mFlag);
        }

        public void SetAllPropertyBlocks(MaterialPropertyBlock[][] propertyBlocks)
        {
            SetPropertyBlocks(propertyBlocks);
        }

        public MaterialPropertyBlock[][] GetAllPropertyBlocks()
        {
            return GetPropertyBlocks(-1, -1);
        }

        /// <summary>
        /// 设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetPropertyBlocks(MaterialPropertyBlock[][] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetPropertyBlocks(propertyBlocks, rFlag, mFlag);
        }

        /// <summary>
        /// 获取所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public MaterialPropertyBlock[][] GetPropertyBlocks(int rFlag = -1, int mFlag = -1)
        {
            Init();
            return updater.GetPropertyBlocks(rFlag, mFlag);
        }

        /// <summary>
        /// 统一规则设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedPropertyBlocks(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedPropertyBlocks(propertyBlocks, rFlag = -1, mFlag);
        }

        /// <summary>
        /// 统一规则反向设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedPropertyBlocksReverse(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedPropertyBlocksReverse(propertyBlocks, rFlag = -1, mFlag);
        }

        /// <summary>
        /// 重置MaterialPropertyBlock
        /// </summary>
        public void ResetPropertyBlocks()
        {
            Init();
            updater.ResetPropertyBlocks();
        }

        /// <summary>
        /// 设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetPropertyBlocks(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetPropertyBlocks(propertyBlocks, rFlag, mFlag);
        }
        #endregion

        #region Material

        /// <summary>
        /// 按顺序重新设置材质
        /// </summary>
        public void SetAllMaterials(Material[][] materials)
        {
            SetMaterials(materials);
        }

        /// <summary>
        /// 获取当前所有的材质，不管是否有效
        /// </summary>
        public Material[][] GetAllMaterials(bool original = false)
        {
            return GetMaterials(-1, -1, original);
        }

        /// <summary>
        /// 设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetMaterials(Material[][] materials, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetMaterials(materials, rFlag, mFlag);
        }

        /// <summary>
        /// 获取当前所有的材质，不管是否有效
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public Material[][] GetMaterials(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            Init();
            return updater.GetMaterials(rFlag, mFlag, original);
        }

        /// <summary>
        /// 统一规则设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据，空材质为保留原有材质</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedMaterials(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedMaterials(materials, rFlag, mFlag);
        }

        /// <summary>
        /// 统一规则反向设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据，空材质为保留原有材质</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedMaterialsReverse(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedMaterialsReverse(materials, rFlag, mFlag);
        }

        /// <summary>
        /// 统一末尾增加材质
        /// </summary>
        /// <param name="materials">统一添加的材质</param>
        /// <param name="rFlag">renderer标记</param>
        public void AddUnifiedMaterials(Material[] materials, int rFlag = -1)
        {
            Init();
            updater.AddUnifiedMaterials(materials, rFlag);
        }

        /// <summary>
        /// 统一末尾删除材质
        /// </summary>
        /// <param name="materials">统一删除的材质</param>
        /// <param name="rFlag">renderer标记</param>
        public void RemoveUnifiedMaterials(int count, int rFlag = -1)
        {
            Init();
            updater.RemoveUnifiedMaterials(count, rFlag);
        }

        /// <summary>
        /// 重置所有材质
        /// </summary>
        public void ResetMaterials()
        {
            if (updater != null)
                updater.ResetMaterials();
        }

        /// <summary>
        /// 设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetMaterials(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetMaterials(materials, rFlag, mFlag);
        }
        #endregion

        #region Shader
        /// <summary>
        /// 设置所有的Shader
        /// </summary>
        public void SetAllShaders(Shader[][] shaders)
        {
            SetShaders(shaders);
        }

        public Shader[][] GetAllShaders(bool original = false)
        {
            return GetShaders(-1, -1, original);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(Shader[][] shaders, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetShaders(shaders, rFlag, mFlag);
        }

        /// <summary>
        /// 获取所有或指定材质的Shader
        /// </summary>
        /// <param name="original">是否SharedMaterials的Shader</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public Shader[][] GetShaders(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            Init();
            return updater.GetShaders(rFlag, mFlag, original);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShaders(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedShaders(shaders, rFlag, mFlag);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShadersReverse(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedShadersReverse(shaders, rFlag, mFlag);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(string[][] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetShaders(shaderNames, rFlag, mFlag);
        }

        /// <summary>
        /// 统一规则设置所有或指定Shader
        /// </summary>
        /// <param name="shaderNames">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShaders(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedShaders(shaderNames, rFlag, mFlag);
        }

        /// <summary>
        /// 统一规则反向设置所有或指定Shader
        /// </summary>
        /// <param name="shaderNames">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShadersReverse(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedShadersReverse(shaderNames, rFlag, mFlag);
        }

        /// <summary>
        /// 重置Shader
        /// </summary>
        /// <param name="original">是否还原SharedMaterials的Shader</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ResetShaders(int rFlag = -1, int mFlag = -1, bool original = true)
        {
            Init();
            updater.ResetShaders(rFlag, mFlag, original);
        }


        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetShaders(shaders, rFlag, mFlag);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetShaders(shaderNames, rFlag, mFlag);
        }
        #endregion

        #region RenderQueue

        /// <summary>
        /// 设置当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="renderQueues">渲染队列数据</param>
        public void SetAllRenderQueues(int[][] renderQueues)
        {
            SetRenderQueues(renderQueues);
        }

        /// <summary>
        /// 获取当前材质或原始共享材质所有或指定材质的渲染队列
        /// </summary>
        /// <param name="original">是否直接获取原始共享材质</param>
        public int[][] GetAllRenderQueues(bool original = false)
        {
            return GetRenderQueues(-1, -1, original);
        }

        /// <summary>
        /// 设置当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="renderQueues">渲染队列数据</param>
        public void SetRenderQueues(int[][] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetRenderQueues(renderQueues, rFlag, mFlag);
        }

        /// <summary>
        /// 获取当前材质或原始共享材质所有或指定材质的渲染队列
        /// </summary>
        /// <param name="original">是否直接获取原始共享材质</param>
        public int[][] GetRenderQueues(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            Init();
            return updater.GetRenderQueues(rFlag, mFlag, original);
        }

        public void SetUnifiedRenderQueue(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedRenderQueue(renderQueues, rFlag, mFlag);
        }

        public void SetUnifiedRenderQueueReverse(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetUnifiedRenderQueueReverse(renderQueues, rFlag, mFlag);
        }


        /// <summary>
        /// 设置所有或指定材质的渲染队列值
        /// </summary>
        /// <param name="value">队列值</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetRenderQueue(int value, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetRenderQueue(value, rFlag, mFlag);
        }

        /// <summary>
        /// 增减所有或指定材质的渲染队列值
        /// </summary>
        /// <param name="dValue">增减值</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void UpdateRenderQueue(int dValue, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.UpdateRenderQueue(dValue, rFlag, mFlag);
        }

        /// <summary>
        /// 还原当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="original">是否还原SharedMaterials的RenderQueue</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ResetRenderQueues(int rFlag = -1, int mFlag = -1, bool original = true)
        {
            Init();
            updater.ResetRenderQueues(rFlag, mFlag, original);
        }

        /// <summary>
        /// 设置当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="renderQueues">渲染队列数据</param>
        public void SetRenderQueues(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            Init();
            updater.SetRenderQueues(renderQueues, rFlag, mFlag);
        }
        #endregion

        public void RevertMaterials()
        {
            ResetMaterials();
            ResetPropertyBlocks();
            //ResetRenderersEnable();
        }

        void OnDestroy()
        {
            RevertMaterials();
        }

        [ContextMenu("Set RenderQueue 3000")]
        void SetRenderQueue3000()
        {
            SetRenderQueue(3000);
        }

        [ContextMenu("Set RenderQueue 3300")]
        void SetRenderQueue3300()
        {
            SetRenderQueue(3300);
        }

        [ContextMenu("Set RenderQueue 3400")]
        void SetRenderQueue3400()
        {
            SetRenderQueue(3400);
        }


        [ContextMenu("Update RenderQueue50")]
        void UpdateRenderQueue50()
        {
            UpdateRenderQueue(50);
        }

        [ContextMenu("Update RenderQueue-50")]
        void UpdateRenderQueue_50()
        {
            UpdateRenderQueue(-50);
        }

        [ContextMenu("Revert RenderQueue")]
        void Revert()
        {
            RevertMaterials();
        }
    }
}
