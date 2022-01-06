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

    private enum DAMAGE_TYPE_Item
    {
        NORMAL,
        SUPERARMOR,
        UNBREAKABLE
    }

    private enum FLIP_TYPE_Item
    {
        HORIZON = 1,
        VERTICAL,
        ALL
    }

    public int frameMax = 0;

    public int imgCount = 0;

    public List<string> imgList;


}
