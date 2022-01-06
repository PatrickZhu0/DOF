using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockHDRColor : PropertyBlockColor
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.HDRCOLOR;
            }
        }

        public PropertyBlockHDRColor(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            Color color = value;
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                color = color.gamma;
            mpb.SetColor(nameID, color);
        }
    }
}
