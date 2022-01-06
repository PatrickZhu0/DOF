using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockColor : PropertyBlockData<Color>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.COLOR;
            }
        }

        public PropertyBlockColor(string name) : base(name)
        {
            value = Color.white;
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            mpb.SetColor(nameID, value);
        }
    }
}
