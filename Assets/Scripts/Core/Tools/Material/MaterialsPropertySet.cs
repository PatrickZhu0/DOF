using UnityEngine;

namespace GameDLL.Tools
{
    [ExecuteInEditMode]
    [AddComponentMenu("Material/MaterialsPropertySet")]
    public class MaterialsPropertySet : IMaterialsReplace
    {
        [SerializeField]
        protected MaterialCache _cache;

        protected override void SetInit()
        {

        }

        void OnEnable()
        {
            SetPropertyBlocks();
        }

        void OnDisable()
        {
            ClearPropertyBlocks();
        }

        public void SetPropertyBlocks()
        {
            if (_init && _cache != null)
                _cache.SetMaterialPropertyBloacks(mpb, _renderers);
        }

        public void ClearPropertyBlocks()
        {
            if (_init && _cache != null)
                _cache.ClearMaterialPropertyBlocks(_renderers);
        }

        public void ChangeInt(int index, int value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockInt(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeFloat(int index, float value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockFloat(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeVector(int index, Vector4 value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockVector(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeColor(int index, Color value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockColor(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeTexture(int index, Texture value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockTexture(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeAnimationCurve(int index, AnimationCurve value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockCurve(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }

        public void ChangeGradient(int index, Gradient value)
        {
            if (_init && _cache != null)
            {
                if (_cache.ChangePropertyBlockGradient(index, value))
                    _cache.SetMaterialPropertyBloacks(mpb, _renderers);
            }
        }
    }
}