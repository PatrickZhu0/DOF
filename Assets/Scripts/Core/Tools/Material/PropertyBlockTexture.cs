using System;
using UnityEngine;

namespace GameDLL.Tools
{
    [Serializable]
    public class PropertyBlockTexture : PropertyBlockData<Texture>
    {
        public override PropertyBlockType propertyBlockType
        {
            get
            {
                return PropertyBlockType.TEXTURE;
            }
        }

        public PropertyBlockTexture(string name) : base(name)
        {
        }

        public override void SetPropertyBlock(MaterialPropertyBlock mpb, float t)
        {
            if (nameID == 0)
            {
                GLog.Warning("Please set property's name first!");
                return;
            }
            mpb.SetTexture(nameID, value);
        }
    }
}
