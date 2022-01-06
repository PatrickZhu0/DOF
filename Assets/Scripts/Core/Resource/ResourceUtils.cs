using System;
using System.IO;
using UnityEngine;

namespace GameDLL
{
    public class ResourceUtils
    {
        /// <summary>
        /// 可读可写持久数据www访问前缀
        /// </summary>
        private static string persistentDataWWWHead
        {
            get
            {
                return "file://";
            }
        }

        /// <summary>
        /// 可读可写持久数据的路径
        /// </summary>
        private static string persistentDataPath
        {
            get
            {
                return Application.persistentDataPath + "/";
            }
        }

        /// <summary>
        /// 持久化数据使用www访问路径
        /// </summary>
        public static string PersistentDataPathFromWWW()
        {
            return persistentDataWWWHead + persistentDataPath;
        }

        /// <summary>
        /// 持久化数据使用www访问文件路径
        /// </summary>
        public static string GetPersistentDataFilePathFromWWW(string file)
        {
            return persistentDataWWWHead + persistentDataPath + file;
        }

        /// <summary>
        /// 持久化数据的路径
        /// </summary>
        public static string PersistentDataPath()
        {
            return persistentDataPath;
        }

        /// <summary>
        /// 持久化数据文件路径
        /// </summary>
        public static string GetPersistentDataFilePath(string file)
        {
            return persistentDataPath + file;
        }

        /// <summary>
        /// 只读文件文件www访问前缀
        /// </summary>
        private static string streamingAssetsWWWHead
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                    return "";
                return "file://";
                //#if UNITY_ANDROID && !UNITY_EDITOR
                //                return "";
                //#else
                //                return "file://";
                //#endif
            }
        }

        /// <summary>
        /// StreamingAssets路径（包内只读文件）
        /// </summary>
        private static string streamingAssetsPath
        {
            get
            {
                return Application.streamingAssetsPath + "/";
            }
        }

        /// <summary>
        /// StreamingAssets使用www访问路径
        /// </summary>
        public static string StreamingAssetsPathFromWWW()
        {
            return streamingAssetsWWWHead + streamingAssetsPath;
        }

        /// <summary>
        /// StreamingAssets使用www访问文件路径
        /// </summary>
        public static string GetStreamingAssetsFilePathFromWWW(string file)
        {
            return streamingAssetsWWWHead + streamingAssetsPath + file;
        }

        /// <summary>
        /// StreamingAssets路径
        /// </summary>
        public static string StreamingAssetsPath()
        {
            return streamingAssetsPath;
        }

        /// <summary>
        /// StreamingAssets文件路径
        /// </summary>
        public static string GetStreamingAssetsFilePath(string file)
        {
            return streamingAssetsPath + file;
        }

        /// <summary>
        /// 获取在补丁根目录或此目录下的文件路径
        /// </summary>
        /// <param name="withFileName">文件名，为空表示仅获取目录名</param>
        /// <returns></returns>
        public static string GetPatchRootDirectoryOrFilePath(string withFileName = "")
        {
            return Path.Combine(persistentDataPath, Const.PatchRootDirectoryName, withFileName);
        }

        /// <summary>
        /// 获取在补丁各版本目录下的目录或文件路径
        /// </summary>
        /// <param name="version">版本名字，为空表示要获取版本根路径</param>
        /// <param name="withFileName">文件名，为空表示仅获取目录名</param>
        /// <returns></returns>
        public static string GetPatchVersionDirectoryOrFilePath(string version="", string withFileName = "")
        {
            return Path.Combine(persistentDataPath, Const.PatchRootDirectoryName, Const.PatchVersionSubDirectoryName, version, withFileName);
        }

        /// <summary>
        /// 获取补丁下载文件的临时目录或文件路径
        /// </summary>
        /// <param name="withFileName">文件名，为空表示仅获取目录名</param>
        /// <returns></returns>
        public static string GetPatchDownloadDirectoryOrFilePath(string withFileName = "")
        {
            return Path.Combine(persistentDataPath, Const.PatchRootDirectoryName, Const.PatchDownloadSubDirectoryName, withFileName);
        }

        /// <summary>
        /// 获取各补丁解开后的Assetbundle所在的统一目录或文件路径
        /// </summary>
        /// <param name="withFileName">文件名，为空表示仅获取目录名</param>
        /// <returns></returns>
        public static string GetPatchAssetBundleDirectoryOrFilePath(string withFileName = "")
        {
            return Path.Combine(persistentDataPath, Const.PatchRootDirectoryName, Const.PatchAssetBundleSubDirectoryName, withFileName);
        }

        /// <summary>
        /// 获取资源路径，优先查找persistentDataPath，再查找streamingAssetsPath
        /// </summary>
        public static string GetAssetsPath(string fileName)
        {
            string filePath = GetPatchAssetBundleDirectoryOrFilePath(fileName);
            if (!File.Exists(filePath))
                filePath = GetStreamingAssetsFilePath(fileName);
            return filePath;
        }

        public static string GetAssetsManifestPath(string fileName, string version)
        {
            string filePath = GetPatchVersionDirectoryOrFilePath(version, fileName);
            if (!File.Exists(filePath))
                filePath = GetStreamingAssetsFilePath(fileName);
            return filePath;
        }

        /// <summary>
        /// 通过UnityWebRequest获取资源路径，优先查找persistentDataPath，再查找streamingAssetsPath
        /// </summary>
        public static string GetAssetsPathFromRequest(string fileName)
        {
            string filePath = GetPersistentDataFilePathFromWWW(fileName);
            if (!File.Exists(filePath))
                filePath = GetStreamingAssetsFilePathFromWWW(fileName);
            return filePath;
        }

        /// <summary>
        /// 通过AssetDataInfo数据获取资源信息
        /// </summary>
        /// <param name="assetPath">资源数据信息</param>
        /// <param name="guid">物体GUID</param>
        /// <param name="assetName">资源名</param>
        public static string GetAssetDataInfo(string assetPath, out string guid, out string assetName)
        {
            guid = string.Empty;
            int idx = assetPath.IndexOf("|");
            if (idx > 0)
            {
                guid = assetPath.Substring(0, idx);
                assetPath = assetPath.Substring(idx + 1);
            }

            assetName = string.Empty;
            if (assetPath.EndsWith("]"))
            {
                idx = assetPath.LastIndexOf("[");
                int len = assetPath.Length - idx - 2;
                assetName = assetPath.Substring(idx + 1, len);
                assetPath = assetPath.Substring(0, idx);
            }
            return assetPath;
        }

        /// <summary>
        /// 从场景或指定位置开始查找物件
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="parent">开始查找的父级</param>
        public static Transform GetTransformFromPath(string path, Transform parent = null)
        {
            string[] p = path.Split('/');
            Transform trans = parent;
            if (p.Length > 0)
            {
                int start = 0;
                if (!trans)
                {
                    //没有父节点从当前场景中获取
                    GameObject obj = GameObject.Find(p[start++]);
                    if (obj)
                        trans = obj.transform;
                }
                if (trans)
                {
                    for (int i = start; i < p.Length; i++)
                    {
                        trans = trans.Find(p[i]);
                        if (trans == null)
                            break;
                    }
                }
            }
            return trans;
        }
    }
}
