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
        filePath = Application.dataPath + "/../PvfRoot/character/swordman/animation/HopSmash.ani";
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
                        case (int)ANIData.DAMAGE_TYPE:
                            int damage_type = br.ReadInt16();
                            //char[] type_name = br.ReadChars(type_length);
                            sb.Append("   [DAMAGE_TYPE]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", (DAMAGE_TYPE_Item)damage_type);
                            break;
                        case (int)ANIData.SET_FLAG:
                            int set_flag = br.ReadInt32();
                            sb.Append("   [SET_FLAG]\r\n");
                            sb.AppendFormat("   '{0}'\r\n", set_flag);
                            break;
                    }
                }

            }

        }
        Debug.LogError(sb.ToString());
    }


    public static void LoadAni(string filePath, ref FAnimSystem.FAnimNode fAnimNode)
    {
        //filePath = Application.dataPath + "/../PvfRoot/character/fighter/animation/rest.ani";
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
                        byte shadow = br.ReadByte();
                        break;
                    case (int)ANIData.LOOP:
                        byte loop = br.ReadByte();
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
                            int[] boxParams = new int[6];
                            for (int j = 0; j < 6; j++)
                            {
                                int boxParam = br.ReadInt32();
                                boxParams[j] = boxParam;
                            }
                            var boxRect1 = Rect.MinMaxRect(boxParams[0], boxParams[1], boxParams[3], boxParams[4]);
                            fFrame.DamageBoxXY.Add(boxRect1);
                            var boxRect2 = Rect.MinMaxRect(boxParams[0], boxParams[2], boxParams[3], boxParams[5]);
                            fFrame.DamageBoxXZ.Add(boxRect2);
                            break;
                        case (int)ANIData.ATTACK_BOX:
                            int[] attackBoxParams = new int[6];
                            for (int j = 0; j < 6; j++)
                            {
                                int boxParam = br.ReadInt32();
                                attackBoxParams[j] = boxParam;
                            }
                            var attackBoxRect1 = Rect.MinMaxRect(attackBoxParams[0], attackBoxParams[1], attackBoxParams[3], attackBoxParams[4]);
                            fFrame.AttackBoxXY.Add(attackBoxRect1);
                            var attackBoxRect2 = Rect.MinMaxRect(attackBoxParams[0], attackBoxParams[2], attackBoxParams[3], attackBoxParams[5]);
                            fFrame.AttackBoxXZ.Add(attackBoxRect2);
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
                            break;
                        case (int)ANIData.IMAGE_RATE:
                            float rate1 = br.ReadSingle();
                            float rate2 = br.ReadSingle();
                            break;
                        case (int)ANIData.IMAGE_ROTATE:
                            float ratate = br.ReadSingle();
                            break;
                        case (int)ANIData.INTERPOLATION:
                            byte interpolation = br.ReadByte();
                            break;
                        case (int)ANIData.GRAPHIC_EFFECT:
                            var effectType = br.ReadInt16();
                            break;
                        case (int)ANIData.DAMAGE_TYPE:
                            int damage_type = br.ReadInt16();
                            break;
                        case (int)ANIData.SET_FLAG:
                            int set_flag = br.ReadInt32();
                            break;
                    }
                }
                fAnimNode.Frames.Add(fFrame);
            }
        }
    }

    public static void loadStringTableBin(string filePath)
    {
        using (FileStream fs = File.OpenRead(filePath))
        {
            byte[] buffer = new byte[fs.Length];

            fs.Read(buffer, 0, buffer.Length);
            fs.Seek(0, SeekOrigin.Begin);
                
            //    BinaryReader br = new BinaryReader(st);
            ////Util.unpackHeaderTree(ref buffer, buffer.Length, fileCrc32);
            //uint count = br.ReadUInt16();
            //Debug.LogError(count);
            //for (int i = 0; i < count; i++)
            //{
            //    int startpos = br.ReadInt32();
            //    int endpos = br.ReadInt32();
            //    int len = endpos - startpos;//相减就是值的长度
            //    //int index = i;//索引就是出现的第几个
            //    //var pathBytes = new byte[len];//分配内存以存储该值的字符串
            //    //Array.Copy(unpackedFileByteArr, startpos + 4, pathBytes, 0, len);//取出该字符串内容
            //    var pathBytes = br.ReadBytes(len);
            //    string pathName = Encoding.GetEncoding("BIG5").GetString(pathBytes).TrimEnd(new char[1]);//解码，这里使用的是BIG5，对于某些文件不一定正确，如果需要更正可以在这个编码这里下手。
            //    Debug.LogError(pathName);
            //}
        }
    }

    public static void LoadChr(string filePath)
    {
        Debug.LogError(filePath);
        StringBuilder sb = new StringBuilder();
        using (FileStream fs = File.OpenRead(filePath))
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Seek(0, SeekOrigin.Begin);
            
            Util.unpackHeaderTree(ref buffer, buffer.Length, 2001802409);

            

            //BinaryReader br = new BinaryReader(buffer);
            Debug.Log(BitConverter.ToInt32(buffer, 16));

            //byte type = br.ReadByte();
            //Debug.Log(type);
            //if (type == 5)
            //{

            //}

            //Debug.Log(br.ReadString());
            //if (unpackedStrBytes.Length >= 7)//如果总字节长度>=7
            //{
            //    for (int i = 2; i < unpackedStrBytes.Length; i += 5)//以5为单步从第二位开始遍历字节
            //    {
            //        //string s = encoding.GetString(numArray).TrimEnd(new char[1]);
            //        if (unpackedStrBytes.Length - i >= 5)//到最后了就不处理了防止内存越界
            //        {
            //            byte currentByte = unpackedStrBytes[i];//猜测应该是内容指示位
            //            if (currentByte == 2 || currentByte == 4 || currentByte == 5 || currentByte == 6 || currentByte == 7 || currentByte == 8 || currentByte == 10)
            //            //如果这个字节是这些中的一个进行对应的特殊处理，如果不是那就没有字符串
            //            {
            //                int after1 = BitConverter.ToInt32(unpackedStrBytes, i + 1);//取该指示位后面的整数
            //                if (currentByte == 10)//这个字符是10时
            //                {
            //                    int before1 = BitConverter.ToInt32(unpackedStrBytes, i - 4);//取指示位前面的整数
            //                                                                                //解释字符串内容的方法已集成到unpackSpecialChr(指示位,后一位整数,前一位整数)中
            //                    bts = Encoding.UTF8.GetBytes(string.Concat(unpackSpecialChr(currentByte, after1, before1), "\r\n"));//获取该指示位代表的字符串
            //                    arr.Add(strpos, bts);
            //                    strpos += bts.Length;
            //                }
            //                else if (currentByte == 7)//这个字符是7时
            //                {
            //                    bts = Encoding.UTF8.GetBytes(string.Concat("`", unpackSpecialChr(currentByte, after1, 0), "`\r\n"));//7不需要前一位整数，外面要套上“``”
            //                    arr.Add(strpos, bts);
            //                    strpos += bts.Length;
            //                }
            //                else if (currentByte == 2 || currentByte == 4)//这个字符是2或者4时，末尾不是换行而是制表符\t
            //                {
            //                    bts = Encoding.UTF8.GetBytes(string.Concat(unpackSpecialChr(currentByte, after1, 0), "\t"));
            //                    arr.Add(strpos, bts);
            //                    strpos += bts.Length;
            //                }
            //                else if (currentByte == 6 || currentByte == 8)//{指示位=`stringbin[后面的整数]`}
            //                {
            //                    string[] str = new string[] { "{", currentByte.ToString(), "=`", unpackSpecialChr(currentByte, after1, 0), "`}\r\n" };
            //                    bts = encoding.GetBytes(string.Concat(str));
            //                    arr.Add(strpos, bts);
            //                    strpos += bts.Length;
            //                }
            //                else if (currentByte == 5) //是5的情况，stringbin[后面的整数]
            //                {
            //                    bts = Encoding.UTF8.GetBytes(string.Concat("\r\n", unpackSpecialChr(currentByte, after1, 0), "\r\n"));
            //                    arr.Add(strpos, bts);
            //                    strpos += bts.Length;
            //                }
            //            }
            //        }
            //    }
            //    bts = encoding.GetBytes("\r\n");//末尾添个换行符
            //    arr.Add(strpos, bts);
            //    strpos += bts.Length;
            //}
        }

    }

}
