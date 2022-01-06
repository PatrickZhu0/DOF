using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockGradient : PropertyBlockData<Gradient>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.GRADIENT;
            }
        }

        public PropertyBlockGradient(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            mpb.SetColor(nameID, value.Evaluate(t));
        }
    }
}
