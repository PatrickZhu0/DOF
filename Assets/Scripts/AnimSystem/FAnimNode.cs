using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FAnimSystem
{

    internal enum ANIData
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

    internal enum Effect_Item
    {
        NONE,
        DODGE,
        LINEARDODGE,
        DARK,
        XOR,
        MONOCHROME,
        SPACEDISTORT
    }

    internal enum DAMAGE_TYPE_Item
    {
        NORMAL,
        SUPERARMOR,
        UNBREAKABLE
    }

    internal enum FLIP_TYPE_Item
    {
        HORIZON = 1,
        VERTICAL,
        ALL
    }

    public class FFrame
    {
        #region Private Fields
        int imgIndex;
        int imgFrameIdx;

        int imagePosX;
        int imagePosY;

        int delay;

        #endregion
        public int ImgIndex { get => imgIndex; set => imgIndex = value; }
        public int ImgFrameIdx { get => imgFrameIdx; set => imgFrameIdx = value; }
        public int ImagePosX { get => imagePosX; set => imagePosX = value; }
        public int ImagePosY { get => imagePosY; set => imagePosY = value; }
        public int Delay { get => delay; set => delay = value; }

        //int boxCount;




    }

    public class FAnimNode
    {
        #region Private Fields
        int frameMax;
        List<string> imgNames;
        List<FFrame> frames;

        List<Sprite> sprites;

        #endregion

        public int FrameMax { get => frameMax; set => frameMax = value; }
        public List<string> ImgNames { get => imgNames; set => imgNames = value; }
        public List<FFrame> Frames { get => frames; set => frames = value; }
        public List<Sprite> Sprites { get => sprites; set => sprites = value; }

        int curFrameIdx = 0;
        public FFrame getCurFrame()
        {
            if (curFrameIdx >= Frames.Count)
            {
                curFrameIdx = 0;
            }
            FFrame curFrame = Frames[curFrameIdx];
            ++curFrameIdx;
            return curFrame;
        }

    }

}


