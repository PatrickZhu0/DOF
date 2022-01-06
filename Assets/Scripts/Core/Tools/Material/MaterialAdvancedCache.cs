using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class MaterialAdvancedCache : MaterialCache
    {
        /// <summary>
        /// 渲染队列变化
        /// </summary>
        public ChangeQueueMode changeQueueMode;
        /// <summary>
        /// 渲染队列变化值
        /// </summary>
        public int rendereQueueValue;

        /// <summary>
        /// 修改数据模式
        /// </summary>
        public ChangeDataMode changeDataMode = ChangeDataMode.DATA_ONLY;

        /// <summary>
        /// 需要完全替换的材质
        /// </summary>
        public Material[] materials;

        /// <summary>
        /// 需要替换的shader名称
        /// </summary>
        public string[] shaderNames;

        public void UpdateRenderers(MaterialsReplace replace)
        {
            switch(changeDataMode)
            {
                case ChangeDataMode.DATA_SHADER:
                    UpdateShaders(replace);
                    break;
                case ChangeDataMode.DATA_MATERIAL:
                    UpdateMaterials(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_SHADER:
                    UpdateUnifiedShaders(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL:
                    UpdateUnifiedMaterials(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_SHADER_REVERSE:
                    UpdateUnifiedShadersReverse(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_REVERSE:
                    UpdateUnifiedMaterialsReverse(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_ADD:
                    AddUnifiedMaterials(replace);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_REMOVE:
                    RemoveUnifiedMaterials(replace);
                    break;
            }
            UpdateRendereQueues(replace);
            UpdateMaterialPropertyBlocks(replace, 0);
        }

        public void UpdateRendereQueues(MaterialsReplace replace)
        {
            //变化材质渲染队列
            if (changeQueueMode == ChangeQueueMode.PLUS_VALUE)
                replace.UpdateRenderQueue(rendereQueueValue, rendererFlag, materialFlag);
            else if (changeQueueMode == ChangeQueueMode.SET_VALUE)
                replace.SetRenderQueue(rendereQueueValue, rendererFlag, materialFlag);

        }

        public void UpdateShaders(MaterialsReplace replace)
        {
            replace.SetShaders(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedShaders(MaterialsReplace replace)
        {
            replace.SetUnifiedShaders(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedShadersReverse(MaterialsReplace replace)
        {
            replace.SetUnifiedShadersReverse(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateMaterials(MaterialsReplace replace)
        {
            replace.SetMaterials(materials, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedMaterials(MaterialsReplace replace)
        {
            replace.SetUnifiedMaterials(materials, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedMaterialsReverse(MaterialsReplace replace)
        {
            replace.SetUnifiedMaterialsReverse(materials, rendererFlag, materialFlag);
        }

        public void AddUnifiedMaterials(MaterialsReplace replace)
        {
            replace.AddUnifiedMaterials(materials, rendererFlag);
        }

        public void RemoveUnifiedMaterials(MaterialsReplace replace)
        {
            replace.RemoveUnifiedMaterials(materials.Length, rendererFlag);
        }

        public void UpdateMaterialPropertyBlocks(MaterialsReplace replace, float time)
        {
            if (blockData != null && blockData.Length > 0)
            {
                replace.ClearPropertyBlock();
                for (int i = 0; i < blockData.Length; i++)
                    replace.UpdatePropertyBlock(blockData[i], time);
                replace.ApplyPropertyBlock(rendererFlag, materialFlag);
            }
        }

        public void UpdateRenderers(RenderersUpdater updater)
        {
            switch (changeDataMode)
            {
                case ChangeDataMode.DATA_SHADER:
                    UpdateShaders(updater);
                    break;
                case ChangeDataMode.DATA_MATERIAL:
                    UpdateMaterials(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_SHADER:
                    UpdateUnifiedShaders(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL:
                    UpdateUnifiedMaterials(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_SHADER_REVERSE:
                    UpdateUnifiedShadersReverse(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_REVERSE:
                    UpdateUnifiedMaterialsReverse(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_ADD:
                    AddUnifiedMaterials(updater);
                    break;
                case ChangeDataMode.DATA_UNIFIED_MATERIAL_REMOVE:
                    RemoveUnifiedMaterials(updater);
                    break;
            }
            UpdateRendereQueues(updater);
            UpdateMaterialPropertyBlocks(updater, 0);
        }

        public void UpdateRendereQueues(RenderersUpdater updater)
        {
            //变化材质渲染队列
            if (changeQueueMode == ChangeQueueMode.PLUS_VALUE)
                updater.UpdateRenderQueue(rendereQueueValue, rendererFlag, materialFlag);
            else if (changeQueueMode == ChangeQueueMode.SET_VALUE)
                updater.SetRenderQueue(rendereQueueValue, rendererFlag, materialFlag);

        }

        public void UpdateShaders(RenderersUpdater updater)
        {
            updater.SetShaders(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedShaders(RenderersUpdater updater)
        {
            updater.SetUnifiedShaders(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedShadersReverse(RenderersUpdater updater)
        {
            updater.SetUnifiedShadersReverse(shaderNames, rendererFlag, materialFlag);
        }

        public void UpdateMaterials(RenderersUpdater updater)
        {
            updater.SetMaterials(materials, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedMaterials(RenderersUpdater updater)
        {
            updater.SetUnifiedMaterials(materials, rendererFlag, materialFlag);
        }

        public void UpdateUnifiedMaterialsReverse(RenderersUpdater updater)
        {
            updater.SetUnifiedMaterialsReverse(materials, rendererFlag, materialFlag);
        }

        public void AddUnifiedMaterials(RenderersUpdater updater)
        {
            updater.AddUnifiedMaterials(materials, rendererFlag);
        }

        public void RemoveUnifiedMaterials(RenderersUpdater updater)
        {
            updater.RemoveUnifiedMaterials(materials.Length, rendererFlag);
        }

        public void UpdateMaterialPropertyBlocks(RenderersUpdater updater, float time)
        {
            if (blockData != null && blockData.Length > 0)
            {
                updater.ClearPropertyBlock();
                for (int i = 0; i < blockData.Length; i++)
                    updater.UpdatePropertyBlock(blockData[i], time);
                updater.ApplyPropertyBlock(rendererFlag, materialFlag);
            }
        }

    }
}