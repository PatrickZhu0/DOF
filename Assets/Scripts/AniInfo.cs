using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AniInfo
{
    private enum ANIData
    {
        LOOP,
        SHADOW,
        COORD = 3,
        IMAGE_RATE = 7,
        IMAGE_ROTATE,
        RGBA,
        INTERPOLATION,
        GRAPHIC_EFFECT,
        DELAY,
        DAMAGE_TYPE,
        DAMAGE_BOX,
        ATTACK_BOX,
        PLAY_SOUND,
        PRELOAD,
        SPECTRUM,
        SET_FLAG = 23,
        FLIP_TYPE,
        LOOP_START,
        LOOP_END,
        CLIP,
        OPERATION
    }

    private enum Effect_Item
    {
        NONE,
        DODGE,
        LINEARDODGE,
        DARK,
        XOR,
        MONOCHROME,
        SPACEDISTORT
    }

    // Token: 0x020000B8 RID: 184
    private enum DAMAGE_TYPE_Item
    {
        NORMAL,
        SUPERARMOR,
        UNBREAKABLE
    }

    // Token: 0x020000B9 RID: 185
    private enum FLIP_TYPE_Item
    {
        // Token: 0x040004AE RID: 1198
        HORIZON = 1,
        // Token: 0x040004AF RID: 1199
        VERTICAL,
        // Token: 0x040004B0 RID: 1200
        ALL
    }

    public int frameMax = 0;

    public int imgCount = 0;

    public List<string> imgList;





}
