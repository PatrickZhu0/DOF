using System;
using UnityEngine;

namespace GameDLL.Tools
{

    [Serializable]
    public class PropertyBlockVector : PropertyBlockData<Vector4>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.VECTOR4;
            }
        }

        public PropertyBlockVector(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            mpb.SetVector(nameID, value);
        }
    }
}
