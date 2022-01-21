using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ANIData
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

public enum Effect_Item
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
public enum DAMAGE_TYPE_Item
{
    NORMAL,
    SUPERARMOR,
    UNBREAKABLE
}

public enum FLIP_TYPE_Item
{
    HORIZON = 1,
    VERTICAL,
    ALL
}

public struct DamageBox
{
    public int type;

    public int x1;
    public int y1;
    public int z1;
    public int x2;
    public int y2;
    public int z2;
}

public class SpriteFrame
{


}

public class SpriteMotion
{

    private string m_FileName;

    private long m_FileSize;


    private int m_FrameMax = 0;
    List<string> imgList = new List<string>();

    List<SpriteFrame> spriteFrames = new List<SpriteFrame>();

    List<DamageBox> damageBoxList = new List<DamageBox>();

    List<DamageBox> attackBoxList = new List<DamageBox>();
    
    public bool Load(string path)
    {
        bool ret = false;

        if (path == "" || !FileReaderProxy.Exists(path))
        {
            return ret;
        }

        Stream ms = null;
        BinaryReader br = null;
        try
        {
            ms = FileReaderProxy.ReadFileAsMemoryStream(path);
            if (ms == null)
            {
                UnityEngine.Debug.LogErrorFormat("DBC, Warning, Load file error or file is empty: {0}", path);
                return false;
            }

            m_FileName = path;
            ms.Seek(0, SeekOrigin.Begin);
            m_FileSize = ms.Length;
            if (m_FileSize <= 0 || m_FileSize >= int.MaxValue)
                return ret;

            br = new BinaryReader(ms);

            ret = LoadFromStream(br);
            ret = true;
        }
        catch (Exception e)
        {
            string err = "Exception:" + e.Message + "\n" + e.StackTrace + "\n";
            System.Diagnostics.Debug.WriteLine(err);
        }
        finally
        {
            if (br != null)
            {
                br.Close();
            }
            if (ms != null)
            {
                ms.Close();
            }
        }

        return ret;
    }

    /// <summary>
    ///  从文件流中读取
    /// </summary>
    /// <param name="sr"></param>
    /// <returns></returns>
    public bool LoadFromStream(BinaryReader br)
    {
        return LoadFromStream_Binary(br);
    }

    /// <summary>
    ///  文件流中读取
    /// </summary>
    /// <param name="sr"></param>
    /// <returns></returns>
    private bool LoadFromStream_Binary(BinaryReader br)
    {
        //两字节 最大帧数
        int frameMax = br.ReadInt16();


        //两字节 引用img文件个数
        int imgCount = br.ReadInt16();
        for (int i = 0; i < imgCount; i++)
        {
            //四字节 img路径名长度
            int imgNameLength = br.ReadInt32();
            //按长度读取字符
            char[] imgName = br.ReadChars(imgNameLength);

            imgList.Add(imgName.ToString());
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

        for (int frame_idx = 0; frame_idx < frameMax; frame_idx++)
        {
            //两字节 碰撞盒个数
            int boxCount = br.ReadInt16();
            for (int i = 0; i < boxCount; i++)
            {
                DamageBox damageBox = new DamageBox();
                damageBox.type = br.ReadInt16();
                damageBox.x1   = br.ReadInt32();
                damageBox.y1   = br.ReadInt32();
                damageBox.z1   = br.ReadInt32();
                damageBox.x2   = br.ReadInt32();
                damageBox.y2   = br.ReadInt32();
                damageBox.z2   = br.ReadInt32();
                
                if (damageBox.type == 1)
                    damageBoxList.Add(damageBox);
                if (damageBox.type == 2)
                    attackBoxList.Add(damageBox);
            }

            //使用第几个img路ing
            int imgPathIdx = br.ReadInt16();
            if (imgPathIdx >= 0)
            {
                //引用该img文件的第几帧
                int imgFrameIdx = br.ReadInt16();
            }


            int imgPosX = br.ReadInt32();
            int imgPosY = br.ReadInt32();

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
        }
        return true;
    }


}
