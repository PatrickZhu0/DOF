using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace GameDLL
{
    public static class GameUtils
    {
        /// <summary>
        /// sbyte取值限制
        /// </summary>
        public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// byte取值限制
        /// </summary>
        public static byte Clamp(this byte value, byte min, byte max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// short取值限制
        /// </summary>
        public static short Clamp(this short value, short min, short max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// ushort取值限制
        /// </summary>
        public static ushort Clamp(this ushort value, ushort min, ushort max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// int取值限制
        /// </summary>
        public static int Clamp(this int value, int min, int max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// uint取值限制
        /// </summary>
        public static uint Clamp(this uint value, uint min, uint max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// long取值限制
        /// </summary>
        public static long Clamp(this long value, long min, long max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// ulong取值限制
        /// </summary>
        public static ulong Clamp(this ulong value, ulong min, ulong max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// float取值限制
        /// </summary>
        public static float Clamp(this float value, float min, float max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// double取值限制
        /// </summary>
        public static double Clamp(this double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        /// <summary>
        /// short取绝对值
        /// </summary>
        public static short Abs(this short value)
        {
            if (value < 0)
                return (short)-value;
            return value;
        }

        /// <summary>
        /// int取绝对值
        /// </summary>
        public static int Abs(this int value)
        {
            if (value < 0)
                return -value;
            return value;
        }

        /// <summary>
        /// long取绝对值
        /// </summary>
        public static long Abs(this long value)
        {
            if (value < 0)
                return -value;
            return value;
        }

        /// <summary>
        /// float取绝对值
        /// </summary>
        public static float Abs(this float value)
        {
            if (value < 0)
                return -value;
            return value;
        }

        /// <summary>
        /// double取绝对值
        /// </summary>
        public static double Abs(this double value)
        {
            if (value < 0)
                return -value;
            return value;
        }

        /// <summary>
        /// float近似比较
        /// </summary>
        public static bool Approximately(this float value, float other, float epsilon = float.Epsilon)
        {
            return Math.Abs(value - other) < epsilon;
        }

        /// <summary>
        /// double近似比较
        /// </summary>
        public static bool Approximately(this double value, double other, double epsilon = double.Epsilon)
        {
            return Math.Abs(value - other) < epsilon;
        }

        public static double RoundUpToNearest(this double passednumber, double roundto)
        {
            return roundto == 0 ? passednumber : Math.Ceiling(passednumber / roundto) * roundto;
        }

        public static double RoundDownToNearest(this double passednumber, double roundto)
        {
            return roundto == 0 ? passednumber : Math.Floor(passednumber / roundto) * roundto;
        }

        public static float RoundUpToNearest(this float passednumber, float roundto)
        {
            return roundto == 0 ? passednumber : Mathf.Ceil(passednumber / roundto) * roundto;
        }

        public static float RoundDownToNearest(this float passednumber, float roundto)
        {
            return roundto == 0 ? passednumber : Mathf.Floor(passednumber / roundto) * roundto;
        }

        public static int RoundUpToNearest(this int passednumber, int roundto)
        {
            return (int)(roundto == 0 ? passednumber : Mathf.Ceil(passednumber / roundto) * roundto);
        }

        public static int RoundDownToNearest(this int passednumber, int roundto)
        {
            return (int)(roundto == 0 ? passednumber : Mathf.Floor(passednumber / roundto) * roundto);
        }

        public static int Flip(this int value)
        {
            return value == 0 ? 1 : 0;
        }







        /// <summary>
        /// 创建文件前检测路径是否存在并创建
        /// </summary>
        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }




        /// <summary>
        /// 将二进制数据写入指定文件
        /// </summary>
        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (Exception ex)
            {
                GLog.ErrorFormat("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 获取流的MD5
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMD5FromSteam(Stream stream)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (System.Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 获取文件的MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                string output = GetMD5FromSteam(file);
                file.Close();
                return output;
            }
            catch (System.Exception)
            {

            }
            return null;
        }

        /// <summary>
        /// 获取字符串的MD5
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GetMD5HashFromString(string pwd)
        {
            var res = "";
            System.Security.Cryptography.MD5 md = System.Security.Cryptography.MD5.Create();
            byte[] s = md.ComputeHash(Encoding.Default.GetBytes(pwd));
            for (int i = 0; i < s.Length; i++)
                res = res + s[i].ToString("X");
            return res;
        }

        /// <summary>
        /// 异步获取MD5
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="finishCallback"></param>
        public static void ComputeFileMD5(string fileName, Action<string> finishCallback)
        {
            GameDLL.Loom loom = GameDLL.Loom.Instance;
            loom.RunAsync(() =>
            {
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    string md5 = GameDLL.GameUtils.GetMD5HashFromFile(fileName);
                    loom.QueueOnMainThread(() =>
                    {
                        finishCallback(md5);
                    });
                });
                thread.Start();
            });
        }

        /// <summary>
        /// 删除文件
        /// 这个操作比较快，就不在后台做了
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        /// <summary>
        /// 移动文件
        /// 这个操作比较快，就不在后台做了
        /// </summary>
        /// <param name="srcFilePath"></param>
        /// <param name="dstFilePath"></param>
        public static void MoveFile(string srcFilePath, string dstFilePath)
        {
            try
            {
                string dstDir = Path.GetDirectoryName(dstFilePath);
                if (!Directory.Exists(dstDir))
                {
                    Directory.CreateDirectory(dstDir);
                }
                if (File.Exists(dstFilePath))
                {
                    File.Delete(dstFilePath);
                }
                File.Move(srcFilePath, dstFilePath);
            }
            catch(Exception e)
            {
                GLog.ErrorFormat("MoveFile failed {0} -> {1} : {2}", srcFilePath, dstFilePath, e.Message);
            }
        }


        public static string GetFileName(string fullPath)
        {
            return Path.GetFileName(fullPath);
        }

        public static string GetFileExtension(string fullPath)
        {
            return Path.GetExtension(fullPath);
        }

        public static string GetDirectoryName(string fullPath)
        {
            return Path.GetDirectoryName(fullPath);
        }

        public static void ResetDirectory(string dirPath)
        {
            if(Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);
            }
            Directory.CreateDirectory(dirPath);
        }

        public static string[] ListFilesInDirectory(string dir)
        {
            return Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
        }
    }
}
