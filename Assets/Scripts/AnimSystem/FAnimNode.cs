using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FAnimSystem {

    internal enum ANIData {
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

    internal enum Effect_Item {
        NONE,
        DODGE,
        LINEARDODGE,
        DARK,
        XOR,
        MONOCHROME,
        SPACEDISTORT
    }

    internal enum DAMAGE_TYPE_Item {
        NORMAL,
        SUPERARMOR,
        UNBREAKABLE
    }

    internal enum FLIP_TYPE_Item {
        HORIZON = 1,
        VERTICAL,
        ALL
    }

    public class FFrame {
        #region Private Fields
        int imgIndex;
        int imgFrameIdx;

        //int boxCount;




        #endregion
    }

    public class FAnimNode {
        #region Private Fields
        int frameMax; //(2 bytes)

        List<string> imgNames;
        List<FFrame> frames;


        #endregion

    }

}


