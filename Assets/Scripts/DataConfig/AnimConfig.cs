using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public delegate byte[] delegate_ReadFile(string path);

public static class FileReaderProxy
{
    private static delegate_ReadFile handlerReadFile;

    public static MemoryStream ReadFileAsMemoryStream(string filePath)
    {
        try
        {
            byte[] buffer = ReadFileAsArray(filePath);
            if (buffer == null)
            {
                UnityEngine.Debug.LogErrorFormat("Err ReadFileAsMemoryStream failed:{0}\n", filePath);
                return null;
            }
            return new MemoryStream(buffer);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogErrorFormat("Exception:{0}\n", e.Message);
            return null;
        }
    }

    public static byte[] ReadFileAsArray(string filePath)
    {
        byte[] buffer = null;
        try
        {
            if (handlerReadFile != null)
            {
                buffer = handlerReadFile(filePath);
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("ReadFileByEngine handler have not register: {0}", filePath);
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogErrorFormat("Exception:{0}\n", e.Message);
            return null;
        }
        return buffer;
    }

    public static bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    public static void RegisterReadFileHandler(delegate_ReadFile handler)
    {
        handlerReadFile = handler;
    }

}

public class AnimConfig
{
    private string m_FileName;

    private long m_FileSize;



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
        }


        return true;
    }
}
