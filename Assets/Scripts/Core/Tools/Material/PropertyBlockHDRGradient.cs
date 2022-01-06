using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockHDRGradient : PropertyBlockGradient
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.HDRGRADIENT;
            }
        }

        public PropertyBlockHDRGradient(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            Color color = value.Evaluate(t);
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
                color = color.gamma;
            mpb.SetColor(nameID, color);
        }
    }
}
