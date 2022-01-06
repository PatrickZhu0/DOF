using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class MaterialCache
    {
        /// <summary>
        /// 修改的渲染器索引
        /// </summary>
        [Min(-1)]
        public int rendererFlag = -1;
        /// <summary>
        /// 修改的材质索引
        /// </summary>
        [Min(-1)]
        public int materialFlag = -1;

        [SerializeReference]
        [SerializeField]
        public PropertyBlockData[] blockData;

        public bool hasBlockData
        {
            get
            {
                return blockData != null && blockData.Length > 0;
            }
        }

        public void SetMaterialPropertyBloacks(MaterialPropertyBlock mpb, Renderer[] renderers)
        {
            if (mpb != null && renderers != null)
            {
                mpb.Clear();
                for (int i = 0; i < blockData.Length; i++)
                {
                    if (blockData[i] != null)
                        blockData[i].SetPropertyBlock(mpb, 0);
                }
                int rCheck = 0;
                int mCheck = 0;
                for (int i = 0; i < renderers.Length; i++)
                {
                    rCheck = 1 << i;
                    if ((rCheck & rendererFlag) != 0)
                    {
                        Material[] materials = renderers[i].sharedMaterials;
                        for (int j = 0; j < materials.Length; j++)
                        {
                            mCheck = 1 << j;
                            if ((mCheck & materialFlag) != 0)
                            {
                                if (materials[j])
                                    renderers[i].SetPropertyBlock(mpb, j);
                            }
                        }
                    }
                }
            }
        }

        public void ClearMaterialPropertyBlocks(Renderer[] renderers)
        {
            if (renderers != null)
            {
                int rCheck = 0;
                int mCheck = 0;
                for (int i = 0; i < renderers.Length; i++)
                {
                    rCheck = 1 << i;
                    if ((rCheck & rendererFlag) != 0)
                    {
                        Material[] materials = renderers[i].sharedMaterials;
                        for (int j = 0; j < materials.Length; j++)
                        {
                            mCheck = 1 << j;
                            if ((mCheck & materialFlag) != 0)
                            {
                                if (materials[j])
                                    renderers[i].SetPropertyBlock(null, j);
                            }
                        }
                    }
                }
            }
        }

        T GetPropertyBlockData<T>(int index, PropertyBlockType type) where T : PropertyBlockData
        {
            if (index >= 0 && index < blockData.Length)
            {
                if (blockData[index].propertyBlockType == type)
                    return blockData[index] as T;
            }
            return null;
        }

        T GetPropertyBlockData<T>(int index, params PropertyBlockType[] types) where T : PropertyBlockData
        {
            if (index >= 0 && index < blockData.Length)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (blockData[index].propertyBlockType == types[i])
                        return blockData[index] as T;
                }
            }
            return null;
        }

        public bool ChangePropertyBlockInt(int index, int value)
        {
            PropertyBlockInt data = GetPropertyBlockData<PropertyBlockInt>(index, PropertyBlockType.INT);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockFloat(int index, float value)
        {
            PropertyBlockFloat data = GetPropertyBlockData<PropertyBlockFloat>(index, PropertyBlockType.FLOAT);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockVector(int index, Vector4 value)
        {
            PropertyBlockVector data = GetPropertyBlockData<PropertyBlockVector>(index, PropertyBlockType.VECTOR4);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockColor(int index, Color value)
        {
            PropertyBlockColor data = GetPropertyBlockData<PropertyBlockColor>(index, PropertyBlockType.COLOR, PropertyBlockType.HDRCOLOR);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockTexture(int index, Texture value)
        {
            PropertyBlockTexture data = GetPropertyBlockData<PropertyBlockTexture>(index, PropertyBlockType.TEXTURE);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockCurve(int index, AnimationCurve value)
        {
            PropertyBlockCurve data = GetPropertyBlockData<PropertyBlockCurve>(index, PropertyBlockType.CURVE);
            if (data != null)
            {
                GameTools.CopyAnimationCurve(value, data.value);
                return true;
            }
            return false;
        }

        public bool ChangePropertyBlockGradient(int index, Gradient value)
        {
            PropertyBlockGradient data = GetPropertyBlockData<PropertyBlockGradient>(index, PropertyBlockType.GRADIENT, PropertyBlockType.HDRGRADIENT);
            if (data != null)
            {
                GameTools.CopyGradient(value, data.value);
                return true;
            }
            return false;
        }
    }
}
