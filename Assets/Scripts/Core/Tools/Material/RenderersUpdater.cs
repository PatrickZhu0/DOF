using System;
using UnityEngine;

namespace GameDLL.Tools
{
    public class RenderersUpdater
    {
        int _groupId;

        Renderer[] _renderers;

        bool[] _startEnabled;

        /// <summary>
        /// 缓存共享材质
        /// </summary>
        Material[][] _sharedMaterials;
        /// <summary>
        /// 缓存上一次的材质
        /// </summary>
        Material[][] _materials;

        MaterialPropertyBlock _mpb;

        private RenderersUpdater() { }

        public static RenderersUpdater Create(Renderer[] renderers, MaterialPropertyBlock _mpb = null, int _groupId = 0)
        {
            RenderersUpdater update = new RenderersUpdater();
            update._renderers = renderers;
            update._groupId = _groupId;
            update._mpb = _mpb;
            update.Init();
            return update;
        }





        public void Init()
        {
            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            if (_materials == null)
                _materials = new Material[_renderers.Length][];
            if (_startEnabled == null)
                _startEnabled = new bool[_renderers.Length];
            if (_sharedMaterials == null)
                _sharedMaterials = new Material[_renderers.Length][];
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_sharedMaterials[i] == null)
                {
                    _sharedMaterials[i] = _renderers[i].sharedMaterials;
                    _startEnabled[i] = _renderers[i].enabled;
                }
                Material[] mats = new Material[_sharedMaterials[i].Length];
                for (int j = 0; j < mats.Length; j++)
                {
                    if (_sharedMaterials[i][j])
                        mats[j] = _sharedMaterials[i][j];
                }
                _materials[i] = mats;
            }
        }

        /// <summary>
        /// 设置Renderer是否渲染
        /// </summary>
        public void SetRenderersEnabled(bool enabled, int rFlag = -1)
        {
            if (_renderers != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                        _renderers[i].enabled = enabled;
                }
            }
        }

        /// <summary>
        /// 重置Renderer是否渲染
        /// </summary>
        public void ResetRenderersEnable(int rFlag = -1)
        {
            if (_renderers != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                        _renderers[i].enabled = _startEnabled[i];
                }
            }
        }


        #region MaterialPropertyBlock

        /// <summary>
        /// 清除缓存的MaterialPropertyBlock
        /// </summary>
        public void ClearPropertyBlock()
        {
            if (_mpb != null)
                _mpb.Clear();
        }

        public void UpdatePropertyBlock(PropertyBlockData data, float t)
        {
            if (data != null)
            {
                //Init();
                data.SetPropertyBlock(_mpb, t);
            }
        }

        public void SetPropertyBlock(int nameID, int value)
        {
            //Init();
            _mpb.SetInt(nameID, value);
        }

        public void SetPropertyBlock(int nameID, float value)
        {
            //Init();
            _mpb.SetFloat(nameID, value);
        }

        public void SetPropertyBlock(int nameID, Vector4 vector)
        {
            //Init();
            _mpb.SetVector(nameID, vector);
        }

        public void SetPropertyBlock(int nameID, Color color)
        {
            //Init();
            _mpb.SetColor(nameID, color);
        }

        /// <summary>
        /// Texture对象没有被脚本其它对象引用，那么此Texture对象是非持久的，会被Resources.UnloadUnusedAssets回收
        /// </summary>
        /// <param name="nameID"></param>
        /// <param name="texture"></param>
        public void SetPropertyBlock(int nameID, Texture texture)
        {
            _mpb.SetTexture(nameID, texture);
        }

        /// <summary>
        /// 应用所有或指定材质的MaterialPropertyBlock
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ApplyPropertyBlock(int rFlag = -1, int mFlag = -1)
        {
            if (_mpb != null)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                    {
                        for (int j = 0; j < _materials[i].Length; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                if (_materials[i][j])
                                {
                                    _renderers[i].SetPropertyBlock(_mpb, j);
                                }
                            }
                        }
                    }
                }
            }
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
            if (propertyBlocks == null || propertyBlocks.Length == 0)
                return;
            //Init();
            int length = Math.Min(_materials.Length, propertyBlocks.Length);
            for (int i = 0; i < length; i++)
            {
                if ((rFlag & (1 << i)) != 0 && propertyBlocks[i] != null)
                {
                    int len = Math.Min(_materials[i].Length, propertyBlocks[i].Length);
                    for (int j = 0; j < len; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j])
                                _renderers[i].SetPropertyBlock(propertyBlocks[i][j], j);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public MaterialPropertyBlock[][] GetPropertyBlocks(int rFlag = -1, int mFlag = -1)
        {
            //Init();
            MaterialPropertyBlock[][] propertyBlocks = new MaterialPropertyBlock[_materials.Length][];
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    propertyBlocks[i] = new MaterialPropertyBlock[_materials[i].Length];
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j])
                            {
                                MaterialPropertyBlock block = new MaterialPropertyBlock();
                                _renderers[i].GetPropertyBlock(block, j);
                                propertyBlocks[i][j] = block;
                            }
                        }
                    }
                }
            }
            return propertyBlocks;
        }

        /// <summary>
        /// 统一规则设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedPropertyBlocks(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            if (propertyBlocks == null || propertyBlocks.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j])
                                _renderers[i].SetPropertyBlock(propertyBlocks[idx], j);
                            idx++;
                            if (idx >= propertyBlocks.Length)
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 统一规则反向设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedPropertyBlocksReverse(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            if (propertyBlocks == null || propertyBlocks.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j])
                                _renderers[i].SetPropertyBlock(propertyBlocks[idx], j);
                            idx++;
                            if (idx >= propertyBlocks.Length)
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 重置MaterialPropertyBlock
        /// </summary>
        public void ResetPropertyBlocks()
        {
            if (_renderers != null && _materials != null)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if (_materials[i][j])
                            _renderers[i].SetPropertyBlock(null, j);
                    }
                }
            }
        }

        /// <summary>
        /// 设置所有或指定材质的MaterialPropertyBlock数据
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetPropertyBlocks(MaterialPropertyBlock[] propertyBlocks, int rFlag = -1, int mFlag = -1)
        {
            if (propertyBlocks == null || propertyBlocks.Length == 0)
                return;
            //Init();
            int arrayIdx = 0;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0 && propertyBlocks[i] != null)
                {
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            MaterialPropertyBlock propertyBlock = propertyBlocks[arrayIdx];
                            if (_materials[i][j])
                                _renderers[i].SetPropertyBlock(propertyBlock, j);
                            arrayIdx++;
                            if (arrayIdx >= propertyBlocks.Length)
                                break;
                        }
                    }
                    if (arrayIdx >= propertyBlocks.Length)
                        break;
                }
            }
        }
        #endregion

        #region Materials
        /// <summary>
        /// 设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetMaterials(Material[][] materials, int rFlag = -1, int mFlag = -1)
        {
            if (materials == null || materials.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    int oldLength = _materials[i].Length;
                    int newLength = materials[i].Length;
                    Material[] mats = new Material[newLength];
                    if (newLength > oldLength)
                    {
                        for (int j = 0; j < newLength; j++)
                        {
                            if (j < oldLength)
                            {
                                if (_materials[i][j] != materials[i][j])
                                    ReplaceMaterial.Remove(_materials[i][j]);
                            }
                            mats[j] = materials[i][j];
                        }
                        setMaterial = true;
                    }
                    else
                    {
                        for (int j = 0; j < oldLength; j++)
                        {
                            if (j < newLength)
                            {
                                if (_materials[i][j] != materials[i][j])
                                {
                                    ReplaceMaterial.Remove(_materials[i][j]);
                                    mats[j] = materials[i][j];
                                    setMaterial = true;
                                }
                                mats[j] = _sharedMaterials[i][j];
                            }
                            else
                            {
                                ReplaceMaterial.Remove(_materials[i][j]);
                                _materials[i][j] = null;
                                setMaterial = true;
                            }
                        }
                        if (setMaterial)
                        {
                            _materials[i] = mats;
                            _renderers[i].materials = _materials[i];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前所有的材质，不管是否有效
        /// </summary>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public Material[][] GetMaterials(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            //Init();
            Material[][] materials = original ? _sharedMaterials : _materials;
            Material[][] mats = new Material[materials.Length][];
            for (int i = 0; i < materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    mats[i] = new Material[materials[i].Length];
                    for (int j = 0; j < materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (materials[i][j])
                                mats[i][j] = materials[i][j];
                        }
                    }
                }
            }
            return mats;
        }

        /// <summary>
        /// 统一规则设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据，空材质为保留原有材质</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedMaterials(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            if (materials == null || materials.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Material material = materials[idx];
                            if (material)
                            {
                                if (_materials[i][j] == material)
                                {
                                }
                                else if (material == _sharedMaterials[i][j])
                                {
                                    ReplaceMaterial.Remove(_materials[i][j]);
                                    _materials[i][j] = material;
                                    setMaterial = true;
                                }
                                else
                                {
                                    Material mat = _materials[i][j];
                                    _materials[i][j] = ReplaceMaterial.Add(material, _groupId, -1);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= materials.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 统一规则反向设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据，空材质为保留原有材质</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedMaterialsReverse(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            if (materials == null || materials.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    bool setMaterial = false;
                    int materialsLen = _materials[i].Length;
                    for (int j = 0; j < materialsLen; j++)
                    {
                        int jj = materialsLen - j - 1;
                        if ((mFlag & (1 << jj)) != 0)
                        {
                            Material material = materials[idx];
                            if (material)
                            {
                                if (_materials[i][jj] == material)
                                {
                                }
                                else if (material == _sharedMaterials[i][jj])
                                {
                                    ReplaceMaterial.Remove(_materials[i][jj]);
                                    _materials[i][jj] = material;
                                    setMaterial = true;
                                }
                                else
                                {
                                    Material mat = _materials[i][jj];
                                    _materials[i][jj] = ReplaceMaterial.Add(material, _groupId, -1);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= materials.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 统一末尾增加材质
        /// </summary>
        /// <param name="materials">统一添加的材质</param>
        /// <param name="rFlag">renderer标记</param>
        public void AddUnifiedMaterials(Material[] materials, int rFlag = -1)
        {
            if (materials == null || materials.Length == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int length = _materials[i].Length;
                    Material[] mats = new Material[length + materials.Length];
                    Array.Copy(_materials[i], mats, length);
                    for (int j = 0; j < materials.Length; j++)
                        mats[length + j] = ReplaceMaterial.Add(materials[j], _groupId);
                    _materials[i] = mats;
                    _renderers[i].materials = mats;
                }
            }
        }

        /// <summary>
        /// 统一末尾删除材质
        /// </summary>
        /// <param name="materials">统一删除的材质</param>
        /// <param name="rFlag">renderer标记</param>
        public void RemoveUnifiedMaterials(int count, int rFlag = -1)
        {
            if (count <= 0)
                return;
            //Init();
            if (count >= _materials.Length)
                return;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int length = _materials[i].Length;
                    Material[] mats = new Material[length - count];
                    Array.Copy(_materials[i], mats, mats.Length);
                    for (int j = mats.Length; j < length; j++)
                        ReplaceMaterial.Remove(_materials[i][j]);
                    _materials[i] = mats;
                    _renderers[i].materials = mats;
                }
            }
        }

        /// <summary>
        /// 重置所有材质
        /// </summary>
        public void ResetMaterials()
        {
            if (_materials != null)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    bool setMaterial = false;
                    int sharedLength = _sharedMaterials[i].Length;
                    int length = _materials[i].Length;
                    Material[] mats = new Material[sharedLength];
                    if (sharedLength > length)
                    {
                        for (int j = 0; j < sharedLength; j++)
                        {
                            if (j < length)
                            {
                                if (_materials[i][j] != _sharedMaterials[i][j])
                                    ReplaceMaterial.Remove(_materials[i][j]);
                            }
                            mats[j] = _sharedMaterials[i][j];
                        }
                        setMaterial = true;
                    }
                    else
                    {
                        for (int j = 0; j < length; j++)
                        {
                            if (j < sharedLength)
                            {
                                if (_materials[i][j] != _sharedMaterials[i][j])
                                {
                                    ReplaceMaterial.Remove(_materials[i][j]);
                                    setMaterial = true;
                                }
                                mats[j] = _sharedMaterials[i][j];
                            }
                            else
                            {
                                ReplaceMaterial.Remove(_materials[i][j]);
                                _materials[i][j] = null;
                                setMaterial = true;
                            }
                        }
                        if (setMaterial)
                        {
                            _materials[i] = mats;
                            _renderers[i].materials = _materials[i];
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 设置所有或指定材质
        /// </summary>
        /// <param name="materials">材质数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetMaterials(Material[] materials, int rFlag = -1, int mFlag = -1)
        {
            if (materials == null || materials.Length == 0)
                return;
            //Init();
            int arrayIdx = 0;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Material material = materials[arrayIdx];
                            if (material)
                            {
                                if (_materials[i][j] == material)
                                {
                                }
                                else if (material == _sharedMaterials[i][j])
                                {
                                    ReplaceMaterial.Remove(_materials[i][j]);
                                    _materials[i][j] = material;
                                    setMaterial = true;
                                }
                                else
                                {
                                    Material mat = _materials[i][j];
                                    _materials[i][j] = ReplaceMaterial.Add(material, _groupId, -1);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            if (arrayIdx >= materials.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                    if (arrayIdx >= materials.Length)
                        break;
                }
            }
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
            if (shaders == null || shaders.Length == 0)
                return;
            Shader[] ss;
            int length = Math.Min(_materials.Length, shaders.Length);
            for (int i = 0; i < length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    ss = shaders[i];
                    if (ss != null)
                    {
                        int len = Math.Min(_materials[i].Length, ss.Length);
                        bool setMaterial = false;
                        for (int j = 0; j < len; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                Shader shader = ss[j];
                                Material mat = _materials[i][j];
                                if (shader && mat)
                                {
                                    if (mat.shader == shader)
                                    {
                                    }
                                    else
                                    {
                                        _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                        ReplaceMaterial.Remove(mat);
                                        setMaterial = true;
                                    }
                                }
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有或指定材质的Shader
        /// </summary>
        /// <param name="original">是否SharedMaterials的Shader</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public Shader[][] GetShaders(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            //Init();
            Material[][] materials = original ? _sharedMaterials : _materials;
            Shader[][] shaders = new Shader[materials.Length][];
            for (int i = 0; i < materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    shaders[i] = new Shader[materials[i].Length];
                    for (int j = 0; j < materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j])
                                shaders[i][j] = _materials[i][j].shader;
                        }
                    }
                }
            }
            return shaders;
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShaders(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            if (shaders == null || shaders.Length == 0)
                return;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Shader shader = shaders[idx];
                            Material mat = _materials[i][j];
                            if (shader && mat)
                            {
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= shaders.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShadersReverse(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            if (shaders == null || shaders.Length == 0)
                return;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int materialsLen = _materials[i].Length;
                    int idx = 0;
                    bool setMaterial = false;
                    for (int j = 0; j < materialsLen; j++)
                    {
                        int jj = materialsLen - j - 1;
                        if ((mFlag & (1 << jj)) != 0)
                        {
                            Shader shader = shaders[idx];
                            Material mat = _materials[i][jj];
                            if (shader && mat)
                            {
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][jj] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= shaders.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        Shader FindShader(Shader shader, string shaderName)
        {
            if (shaderName[0] == '+')
                return Shader.Find(shader.name + shaderName.Substring(1));
            else if (shaderName[shaderName.Length - 1] == '+')
            {
                string currentShaderName = shader.name;
                //原来的shader名称后加前缀
                int index = currentShaderName.LastIndexOf('/');
                if (index >= 0)
                    return Shader.Find(currentShaderName.Substring(0, index + 1) + shaderName.Substring(0, shaderName.Length - 1) + currentShaderName.Substring(index + 1));
                else
                    return Shader.Find(shaderName.Substring(0, shaderName.Length - 1) + currentShaderName);
            }
            else
                return Shader.Find(shaderName);
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(string[][] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            if (shaderNames == null || shaderNames.Length == 0)
                return;
            int length = Math.Min(_materials.Length, shaderNames.Length);
            for (int i = 0; i < length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    string[] names = shaderNames[i];
                    if (names != null)
                    {
                        bool setMaterial = false;
                        int len = Math.Min(_materials[i].Length, names.Length);
                        for (int j = 0; j < len; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                Material mat = _materials[i][j];
                                string shaderName = names[j];
                                if (mat && !string.IsNullOrEmpty(shaderName))
                                {
                                    Shader shader = FindShader(mat.shader, shaderName);
                                    if (mat.shader == shader)
                                    {
                                    }
                                    else
                                    {
                                        _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                        ReplaceMaterial.Remove(mat);
                                        setMaterial = true;
                                    }
                                }
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                    }
                }
            }
        }

        /// <summary>
        /// 统一规则设置所有或指定Shader
        /// </summary>
        /// <param name="shaderNames">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShaders(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            if (shaderNames == null || shaderNames.Length == 0)
                return;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int idx = 0;
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Material mat = _materials[i][j];
                            string shaderName = shaderNames[idx];
                            if (mat && !string.IsNullOrEmpty(shaderName))
                            {
                                Shader shader = FindShader(mat.shader, shaderName);
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= shaderNames.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 统一规则反向设置所有或指定Shader
        /// </summary>
        /// <param name="shaderNames">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetUnifiedShadersReverse(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            if (shaderNames == null || shaderNames.Length == 0)
                return;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int materialsLen = _materials[i].Length;
                    int idx = 0;
                    bool setMaterial = false;
                    for (int j = 0; j < materialsLen; j++)
                    {
                        int jj = materialsLen - j - 1;
                        if ((mFlag & (1 << jj)) != 0)
                        {
                            Material mat = _materials[i][jj];
                            string shaderName = shaderNames[idx];
                            if (mat && !string.IsNullOrEmpty(shaderName))
                            {
                                Shader shader = FindShader(mat.shader, shaderName);
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][jj] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            idx++;
                            if (idx >= shaderNames.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 重置Shader
        /// </summary>
        /// <param name="original">是否还原SharedMaterials的Shader</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ResetShaders(int rFlag = -1, int mFlag = -1, bool original = true)
        {
            if (_renderers != null && _materials != null)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                    {
                        bool setMaterial = false;
                        for (int j = 0; j < _materials[i].Length; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                Material material = _materials[i][j];
                                if (original)
                                {
                                    if (material != _sharedMaterials[i][j])
                                    {
                                        _materials[i][j] = ReplaceMaterial.Add(material, _groupId, material.renderQueue, _sharedMaterials[i][j].shader);
                                        ReplaceMaterial.Remove(material);
                                        setMaterial = true;
                                    }
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.ResetShader(material);
                                    setMaterial = material != _materials[i][j];
                                }
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                    }
                }
            }
        }


        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(Shader[] shaders, int rFlag = -1, int mFlag = -1)
        {
            if (shaders == null || shaders.Length == 0)
                return;
            int arrayIdx = 0;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Shader shader = shaders[arrayIdx];
                            Material mat = _materials[i][j];
                            if (shader && mat)
                            {
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            if (arrayIdx >= shaders.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                    if (arrayIdx >= shaders.Length)
                        break;
                }
            }
        }

        /// <summary>
        /// 设置所有或指定Shader
        /// </summary>
        /// <param name="materials">Sahder数据</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetShaders(string[] shaderNames, int rFlag = -1, int mFlag = -1)
        {
            if (shaderNames == null || shaderNames.Length == 0)
                return;
            int arrayIdx = 0;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            Material mat = _materials[i][j];
                            string shaderName = shaderNames[arrayIdx];
                            if (mat && !string.IsNullOrEmpty(shaderName))
                            {
                                Shader shader = FindShader(mat.shader, shaderName);
                                if (mat.shader == shader)
                                {
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue, shader);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            if (arrayIdx >= shaderNames.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                    if (arrayIdx >= shaderNames.Length)
                        break;
                }
            }
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
            if (renderQueues == null || renderQueues.Length == 0)
                return;
            //Init();
            int length = Math.Min(_materials.Length, renderQueues.Length);
            for (int i = 0; i < length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    int[] queues = renderQueues[i];
                    if (queues != null)
                    {
                        bool setMaterial = false;
                        int len = Math.Min(_materials[i].Length, queues.Length);
                        for (int j = 0; j < len; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                int renderQueue = queues[j];
                                if (renderQueue >= 0)
                                {
                                    if (_materials[i][j] == _sharedMaterials[i][j])
                                    {
                                        if (renderQueue != _materials[i][j].renderQueue)
                                        {
                                            _materials[i][j] = ReplaceMaterial.Add(_sharedMaterials[i][j], _groupId, renderQueue);
                                            setMaterial = true;
                                        }
                                    }
                                    else if (_materials[i][j].renderQueue != renderQueue)
                                    {
                                        Material mat = _materials[i][j];
                                        _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, renderQueue);
                                        ReplaceMaterial.Remove(mat);
                                        setMaterial = true;
                                    }
                                }
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前材质或原始共享材质所有或指定材质的渲染队列
        /// </summary>
        /// <param name="original">是否直接获取原始共享材质</param>
        public int[][] GetRenderQueues(int rFlag = -1, int mFlag = -1, bool original = false)
        {
            //Init();
            Material[][] mats = original ? _sharedMaterials : _materials;
            int[][] renderQueues = new int[mats.Length][];
            for (int i = 0; i < mats.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    renderQueues[i] = new int[mats[i].Length];
                    for (int j = 0; j < mats[i].Length; j++)
                    {
                        if (mats[i][j] && (mFlag & (1 << j)) != 0)
                            renderQueues[i][j] = mats[i][j].renderQueue;
                        else
                            renderQueues[i][j] = -1;
                    }
                }
            }
            return renderQueues;
        }

        public void SetUnifiedRenderQueue(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            if (renderQueues == null || renderQueues.Length == 0)
                ResetRenderQueues();
            else
            {
                //Init();
                for (int i = 0; i < _materials.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                    {
                        int idx = 0;
                        bool setMaterial = false;
                        for (int j = 0; j < _materials[i].Length; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                int renderQueue = renderQueues[idx];
                                if (renderQueue >= 0)
                                {
                                    if (_materials[i][j] == _sharedMaterials[i][j])
                                    {
                                        if (renderQueue != _materials[i][j].renderQueue)
                                        {
                                            _materials[i][j] = ReplaceMaterial.Add(_sharedMaterials[i][j], _groupId, renderQueue);
                                            setMaterial = true;
                                        }
                                    }
                                    else if (_materials[i][j].renderQueue != renderQueue)
                                    {
                                        Material mat = _materials[i][j];
                                        _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, renderQueue);
                                        ReplaceMaterial.Remove(mat);
                                        setMaterial = true;
                                    }
                                }
                                idx++;
                                if (idx >= renderQueues.Length)
                                    break;
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                        if (idx >= renderQueues.Length)
                            break;
                    }
                }
            }
        }

        public void SetUnifiedRenderQueueReverse(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            if (renderQueues == null || renderQueues.Length == 0)
                ResetRenderQueues();
            else
            {
                //Init();
                for (int i = 0; i < _materials.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                    {
                        int idx = 0;
                        bool setMaterial = false;
                        int materialsLen = _materials[i].Length;
                        for (int j = 0; j < materialsLen; j++)
                        {
                            int jj = materialsLen - j - 1;
                            if ((mFlag & (1 << jj)) != 0)
                            {
                                int renderQueue = renderQueues[idx];
                                if (renderQueue >= 0)
                                {
                                    Material mat = _materials[i][jj];
                                    if (mat == _sharedMaterials[i][jj])
                                    {
                                        if (renderQueue != _materials[i][j].renderQueue)
                                        {
                                            _materials[i][jj] = ReplaceMaterial.Add(_sharedMaterials[i][jj], _groupId, renderQueue);
                                            setMaterial = true;
                                        }
                                    }
                                    else if (mat.renderQueue != renderQueue)
                                    {
                                        _materials[i][jj] = ReplaceMaterial.Add(mat, _groupId, renderQueue);
                                        ReplaceMaterial.Remove(mat);
                                        setMaterial = true;
                                    }
                                }
                                idx++;
                                if (idx >= renderQueues.Length)
                                    break;
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                        if (idx >= renderQueues.Length)
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 设置所有或指定材质的渲染队列值
        /// </summary>
        /// <param name="value">队列值</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void SetRenderQueue(int value, int rFlag = -1, int mFlag = -1)
        {
            if (value < 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if (_materials[i][j] && (mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j] == _sharedMaterials[i][j])
                            {
                                _materials[i][j] = ReplaceMaterial.Add(_sharedMaterials[i][j], _groupId, value);
                                setMaterial = true;
                            }
                            else if (_materials[i][j].renderQueue != value)
                            {
                                Material mat = _materials[i][j];
                                _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, value);
                                ReplaceMaterial.Remove(mat);
                                setMaterial = true;
                            }
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 增减所有或指定材质的渲染队列值
        /// </summary>
        /// <param name="dValue">增减值</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void UpdateRenderQueue(int dValue, int rFlag = -1, int mFlag = -1)
        {
            if (dValue == 0)
                return;
            //Init();
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if (_materials[i][j] && (mFlag & (1 << j)) != 0)
                        {
                            if (_materials[i][j] == _sharedMaterials[i][j])
                            {
                                _materials[i][j] = ReplaceMaterial.Add(_sharedMaterials[i][j], _groupId, _sharedMaterials[i][j].renderQueue + dValue);
                                setMaterial = true;
                            }
                            else
                            {
                                Material mat = _materials[i][j];
                                _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, mat.renderQueue + dValue);
                                ReplaceMaterial.Remove(mat);
                                setMaterial = true;
                            }
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                }
            }
        }

        /// <summary>
        /// 还原当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="original">是否还原SharedMaterials的RenderQueue</param>
        /// <param name="rFlag">renderer标记</param>
        /// <param name="mFlag">单个renderer中material标记</param>
        public void ResetRenderQueues(int rFlag = -1, int mFlag = -1, bool original = true)
        {
            if (_renderers != null && _materials != null)
            {
                for (int i = 0; i < _materials.Length; i++)
                {
                    if ((rFlag & (1 << i)) != 0)
                    {
                        bool setMaterial = false;
                        for (int j = 0; j < _materials[i].Length; j++)
                        {
                            if ((mFlag & (1 << j)) != 0)
                            {
                                Material material = _materials[i][j];
                                if (original)
                                {
                                    if (material != _sharedMaterials[i][j])
                                    {
                                        _materials[i][j] = ReplaceMaterial.Add(material, _groupId, _sharedMaterials[i][j].renderQueue, material.shader);
                                        ReplaceMaterial.Remove(material);
                                        setMaterial = true;
                                    }
                                }
                                else
                                {
                                    _materials[i][j] = ReplaceMaterial.ResetRenderQueue(material);
                                    setMaterial = material != _materials[i][j];
                                }
                            }
                        }
                        if (setMaterial)
                            _renderers[i].materials = _materials[i];
                    }
                }
            }
        }

        /// <summary>
        /// 设置当前所有材质或指定材质的渲染队列
        /// </summary>
        /// <param name="renderQueues">渲染队列数据</param>
        public void SetRenderQueues(int[] renderQueues, int rFlag = -1, int mFlag = -1)
        {
            if (renderQueues == null || renderQueues.Length == 0)
                return;
            //Init();
            int arrayIdx = 0;
            for (int i = 0; i < _materials.Length; i++)
            {
                if ((rFlag & (1 << i)) != 0)
                {
                    bool setMaterial = false;
                    for (int j = 0; j < _materials[i].Length; j++)
                    {
                        if ((mFlag & (1 << j)) != 0)
                        {
                            int renderQueue = renderQueues[arrayIdx];
                            if (renderQueue >= 0)
                            {
                                if (_materials[i][j] == _sharedMaterials[i][j])
                                {
                                    if (renderQueue != _materials[i][j].renderQueue)
                                    {
                                        _materials[i][j] = ReplaceMaterial.Add(_sharedMaterials[i][j], _groupId, renderQueue);
                                        setMaterial = true;
                                    }
                                }
                                else if (_materials[i][j].renderQueue != renderQueue)
                                {
                                    Material mat = _materials[i][j];
                                    _materials[i][j] = ReplaceMaterial.Add(mat, _groupId, renderQueue);
                                    ReplaceMaterial.Remove(mat);
                                    setMaterial = true;
                                }
                            }
                            arrayIdx++;
                            if (arrayIdx >= renderQueues.Length)
                                break;
                        }
                    }
                    if (setMaterial)
                        _renderers[i].materials = _materials[i];
                    if (arrayIdx >= renderQueues.Length)
                        break;
                }
            }
        }
        #endregion

        public void RevertMaterials()
        {
            ResetMaterials();
            ResetPropertyBlocks();
            //ResetRenderersEnable();
        }
    }
}
