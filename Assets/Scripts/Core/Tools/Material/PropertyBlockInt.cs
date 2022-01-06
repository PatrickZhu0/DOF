using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockInt : PropertyBlockData<int>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.INT;
            }
        }

        public PropertyBlockInt(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            mpb.SetInt(nameID, value);
        }

    }
}
