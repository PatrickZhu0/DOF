using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using pvfLoaderXinyu;
using System;

public class BinaryAniCompiler
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

    private enum FLIP_TYPE_Item
    {
        HORIZON = 1,
        VERTICAL,
        ALL
    }


    public static void LoadAni(string filePath)
    {
        filePath = Application.dataPath + "/../PvfRoot/character/fighter/animation/move.ani";
        //filePath = Path.Combine(Application.streamingAssetsPath, "108stairsexfinal.ani");
        //string filePath = "C:/Users/unknown/Desktop/Documents/DNFTools/finishfinal5.ani";
        Debug.LogError(filePath);
        StringBuilder sb = new StringBuilder();
        using (FileStream st = File.OpenRead(filePath))
        {
            BinaryReader br = new BinaryReader(st);

            //两字节 最大帧数
            int frameMax = br.ReadInt16();
            sb.Append("[FRAME MAX]\r\n");
            sb.Append(frameMax + "\r\n");

            //两字节 引用img文件个数
            int imgCount = br.ReadInt16();
            for (int i = 0; i < imgCount; i++)
            {
                //四字节 img路径名长度
                int imgNameLength = br.ReadInt32();
                //按长度读取字符
                char[] imgName = br.ReadChars(imgNameLength);
                string name = new(imgName);
                sb.AppendFormat("[IMAGE LIST]\r\n");
                sb.AppendFormat("  [{0}] {1} \r\n", i, name);
            }

            //两字节 全局配置个数
            int globalCount = br.ReadInt16();
            for (int i = 1; i <= globalCount; i++)
            {
                int globalType = br.ReadInt16();
                //按类型读取不同长度值
                switch (globalType)
                {
                    case (int)ANIData.SHADOW:
                        sb.Append("   [SHADOW]\r\n");
                        //byte globalParamValue = br.ReadByte
                        byte shadow = br.ReadByte();
                        sb.AppendFormat("   {0}\r\n", shadow);
                        break;
                    case (int)ANIData.LOOP:
                        sb.Append("   [LOOP]\r\n");
                        //byte globalParamValue = br.ReadByte
                        byte loop = br.ReadByte();
                        sb.AppendFormat("   {0}\r\n", loop);
                        break;
                }
            }

            //读取frame数据
            for (int frame_idx = 0; frame_idx < frameMax; frame_idx++)
            {
                sb.AppendFormat("[FRAME00{0}]\r\n", frame_idx);

                //两字节 碰撞盒个数
                int boxCount = br.ReadInt16();
                for (int i = 0; i < boxCount; i++)
                {
                    //按类型读取
                    int boxType = br.ReadInt16();
                    switch (boxType)
                    {
                        case (int)ANIData.DAMAGE_BOX:
                            sb.Append("   [DAMAGE BOX]\r\n");
                            break;
                        case (int)ANIData.ATTACK_BOX:
                            sb.Append("   [ATTACK BOX]\r\n");
                            break;
                    }

                    //目前只发现两种，切参数一致
                    string strParam = "    ";
                    for (int j = 0; j < 6; j++)
                    {
                        int boxParam2 = br.ReadInt32();
                        strParam = strParam + boxParam2 + "  ";
                    }
                    sb.AppendFormat("{0}\r\n", strParam);
                }

                //使用第几个img路ing
                int imgPathIdx = br.ReadInt16();
                if (imgPathIdx >= 0)
                {
                    //引用该img文件的第几帧
                    int imgFrameIdx = br.ReadInt16();
                    //Debug.LogError("    [IMAGE FRAME INDEX]:" + imgFrameIdx);

                    sb.Append("   [IMAGE LIST INDEX]\r\n");
                    sb.AppendFormat("   {0}  {1}\r\n", imgPathIdx, imgFrameIdx);
                }
                else
                {
                    //不引用img文件
                    sb.Append("   [IMAGE LIST INDEX]\r\n");
                    sb.AppendFormat("   {0}\r\n", imgPathIdx);
                }

                int imgPosX = br.ReadInt32();
                int imgPosY = br.ReadInt32();
                sb.Append("   [IMAGE POS]\r\n");
                sb.AppendFormat("   {0}  {1}\r\n", imgPosX, imgPosY);

                //两字节 帧参数个数
                int frameAttrCount = br.ReadInt16();
                for (int i = 1; i <= frameAttrCount; i++)
                {
                    //两字节 参数类型
                    int frameAttrType = br.ReadInt16();
                    switch (frameAttrType)
                    {
                        case (int)ANIData.DELAY:
                            int value = br.ReadInt32();
                            sb.Append("   [DELAY]\r\n");
                            sb.AppendFormat("   {0}\r\n", value);
                            break;
                        case (int)ANIData.PLAY_SOUND:
                            int length = br.ReadInt32();
                            char[] name = br.ReadChars(length);
                            sb.Append("   [PLAY_SOUND]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", new string(name));
                            break;
                        case (int)ANIData.IMAGE_RATE:
                            float rate1 = br.ReadSingle();
                            float rate2 = br.ReadSingle();
                            sb.Append("   [IMAGE_RATE]\r\n");
                            sb.AppendFormat("   {0}, {1}\r\n", rate1, rate2);
                            break;
                        case (int)ANIData.IMAGE_ROTATE:
                            float ratate = br.ReadSingle();
                            sb.Append("   [IMAGE_RATE]\r\n");
                            sb.AppendFormat("   {0}\r\n", ratate);
                            break;
                        case (int)ANIData.INTERPOLATION:
                            byte interpolation = br.ReadByte();
                            sb.Append("   [INTERPOLATION]\r\n");
                            sb.AppendFormat("   {0}\r\n", interpolation);
                            break;
                        case (int)ANIData.GRAPHIC_EFFECT:
                            var effectType = br.ReadInt16();
                            sb.Append("   [GRAPHIC_EFFECT]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", (Effect_Item)effectType);
                            break;
                    }
                }

            }

        }
        Debug.LogError(sb.ToString());
    }


    public static void LoadAni(string filePath, ref FAnimSystem.FAnimNode fAnimNode)
    {
        filePath = Application.dataPath + "/../PvfRoot/character/fighter/animation/rest.ani";
        StringBuilder sb = new StringBuilder();
        using (FileStream st = File.OpenRead(filePath))
        {
            BinaryReader br = new BinaryReader(st);

            //两字节 最大帧数
            int frameMax = br.ReadInt16();
            fAnimNode.FrameMax = frameMax;


            //两字节 引用img文件个数
            int imgCount = br.ReadInt16();
            if (imgCount > 0)
            {
                fAnimNode.ImgNames = new List<string>();
            }
            for (int i = 0; i < imgCount; i++)
            {
                //四字节 img路径名长度
                int imgNameLength = br.ReadInt32();
                //按长度读取字符
                char[] imgName = br.ReadChars(imgNameLength);
                fAnimNode.ImgNames.Add(new(imgName));
            }

            //两字节 全局配置个数
            int globalCount = br.ReadInt16();
            for (int i = 1; i <= globalCount; i++)
            {
                int globalType = br.ReadInt16();
                //按类型读取不同长度值
                switch (globalType)
                {
                    case (int)ANIData.SHADOW:
                        sb.Append("   [SHADOW]\r\n");
                        //byte globalParamValue = br.ReadByte
                        byte shadow = br.ReadByte();
                        sb.AppendFormat("   {0}\r\n", shadow);
                        break;
                    case (int)ANIData.LOOP:
                        sb.Append("   [LOOP]\r\n");
                        //byte globalParamValue = br.ReadByte
                        byte loop = br.ReadByte();
                        sb.AppendFormat("   {0}\r\n", loop);
                        break;
                }
            }

            if (frameMax > 0)
            {
                fAnimNode.Frames = new List<FAnimSystem.FFrame>();
            }

            //读取frame数据
            for (int frame_idx = 0; frame_idx < frameMax; frame_idx++)
            {
                FAnimSystem.FFrame fFrame = new FAnimSystem.FFrame();

                //两字节 碰撞盒个数
                int boxCount = br.ReadInt16();
                for (int i = 0; i < boxCount; i++)
                {
                    //按类型读取
                    int boxType = br.ReadInt16();
                    switch (boxType)
                    {
                        case (int)ANIData.DAMAGE_BOX:
                            //目前只发现两种，切参数一致
                            for (int j = 0; j < 6; j++)
                            {
                                int boxParam2 = br.ReadInt32();

                            }
                            break;
                        case (int)ANIData.ATTACK_BOX:
                            //目前只发现两种，切参数一致
                            for (int j = 0; j < 6; j++)
                            {
                                int boxParam2 = br.ReadInt32();

                            }
                            break;
                    }

                }

                //使用第几个img路ing
                int imgPathIdx = br.ReadInt16();
                if (imgPathIdx >= 0)
                {
                    //引用该img文件的第几帧
                    int imgFrameIdx = br.ReadInt16();
                    fFrame.ImgIndex = imgPathIdx;
                    fFrame.ImgFrameIdx = imgFrameIdx;
                }

                int imgPosX = br.ReadInt32();
                int imgPosY = br.ReadInt32();
                fFrame.ImagePosX = imgPosX;
                fFrame.ImagePosY = imgPosY;
                //两字节 帧参数个数
                int frameAttrCount = br.ReadInt16();
                for (int i = 1; i <= frameAttrCount; i++)
                {
                    //两字节 参数类型
                    int frameAttrType = br.ReadInt16();
                    switch (frameAttrType)
                    {
                        case (int)ANIData.DELAY:
                            int value = br.ReadInt32();
                            fFrame.Delay = value;
                            break;
                        case (int)ANIData.PLAY_SOUND:
                            int length = br.ReadInt32();
                            char[] name = br.ReadChars(length);
                            sb.Append("   [PLAY_SOUND]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", new string(name));
                            break;
                        case (int)ANIData.IMAGE_RATE:
                            float rate1 = br.ReadSingle();
                            float rate2 = br.ReadSingle();
                            sb.Append("   [IMAGE_RATE]\r\n");
                            sb.AppendFormat("   {0}, {1}\r\n", rate1, rate2);
                            break;
                        case (int)ANIData.IMAGE_ROTATE:
                            float ratate = br.ReadSingle();
                            sb.Append("   [IMAGE_RATE]\r\n");
                            sb.AppendFormat("   {0}\r\n", ratate);
                            break;
                        case (int)ANIData.INTERPOLATION:
                            byte interpolation = br.ReadByte();
                            sb.Append("   [INTERPOLATION]\r\n");
                            sb.AppendFormat("   {0}\r\n", interpolation);
                            break;
                        case (int)ANIData.GRAPHIC_EFFECT:
                            var effectType = br.ReadInt16();
                            sb.Append("   [GRAPHIC_EFFECT]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", (Effect_Item)effectType);
                            break;
                    }
                }
                fAnimNode.Frames.Add(fFrame);
            }

        }
        Debug.LogError(sb.ToString());
    }
}
