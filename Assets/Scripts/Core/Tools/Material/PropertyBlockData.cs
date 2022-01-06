using System;
using UnityEngine;

namespace GameDLL.Tools
{
    public enum PropertyBlockType
    {
        NONE,
        INT,
        FLOAT,
        VECTOR4,
        COLOR,
        HDRCOLOR,
        TEXTURE,
        CURVE,
        GRADIENT,
        HDRGRADIENT,
    }

    [Serializable]
    public class PropertyBlockData
    {
//#if UNITY_EDITOR
        [SerializeField]
        private string _propertyName;

        [SerializeField]
        private PropertyBlockType _propertyType = PropertyBlockType.NONE;
//#endif

        private int _nameID;

        public int nameID
        {
            get
            {
                if(_nameID == 0)
                    _nameID = Shader.PropertyToID(_propertyName);
                return _nameID;
            }
        }

        public string name
        {
            set
            {
                if (_propertyName != value && !string.IsNullOrEmpty(value))
                {
                    _propertyName = value;
                    _nameID = Shader.PropertyToID(value);
                }
            }
        }

        public virtual PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.NONE;
            }
        }

        public PropertyBlockData(string name)
        {
//#if UNITY_EDITOR
            _propertyType = propertyBlockType;
            _propertyName = name;
//#endif
            _nameID = Shader.PropertyToID(name);
        }

        public bool IsName(string name)
        {
            return nameID == Shader.PropertyToID(name);
        }

        public virtual void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            GLog.Warning("Please set property's type first!");
        }
    }

    [Serializable]
    public abstract class PropertyBlockData<T> : PropertyBlockData
    {
        public T value;

        protected PropertyBlockData(string name) : base(name)
        {
            
        }
    }
}