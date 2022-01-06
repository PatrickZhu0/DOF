using System;
using UnityEngine;

namespace GameDLL.Tools
{

    [Serializable]
    public class PropertyBlockFloat : PropertyBlockData<float>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.FLOAT;
            }
        }

        public PropertyBlockFloat(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            mpb.SetFloat(nameID, value);
        }
    }
}
