using System;

namespace GameDLL
{
    [Serializable]
    public class AssetDataInfo : IEquatable<AssetDataInfo>, ICloneable
    {
        /// <summary>
        /// 资源数据guid|AssetBundle名称[资源名]
        /// </summary>
        public string assetInfo;

        /// <summary>
        /// 资源类型
        /// </summary>
        public string assetType;

        /// <summary>
        /// 资源路径是否在场景中
        /// </summary>
        public bool assetInScene;

        /// <summary>
        /// 资源在工程或场景中的路径
        /// </summary>
        public string assetPath;

        public AssetDataInfo()
        {
            assetType = typeof(UnityEngine.Object).AssemblyQualifiedName;
        }

        public AssetDataInfo(Type type, bool inScene = false)
        {
            assetInfo = string.Empty;
            assetType = type.AssemblyQualifiedName;
            assetInScene = inScene;
            assetPath = string.Empty;
        }

        public AssetDataInfo(string assemblyQualifiedName, bool inScene = false)
        {
            assetInfo = string.Empty;
            assetType = assemblyQualifiedName;
            assetInScene = inScene;
            assetPath = string.Empty;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(assetPath);
        }

        public bool Equals(AssetDataInfo other)
        {
            return assetInfo == other.assetInfo && assetType == other.assetType && assetInScene == other.assetInScene && assetPath == other.assetPath;
        }

        public object Clone()
        {
            AssetDataInfo info = new AssetDataInfo();
            info.assetInfo = assetInfo;
            info.assetType = assetType;
            info.assetInScene = assetInScene;
            info.assetPath = assetPath;
            return info;
        }
    }
}