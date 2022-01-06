namespace GameDLL
{
    /// <summary>
    /// 游戏大部分配置设定
    /// </summary>
    public class Const
    {
        /// <summary>
        /// 默认的AssetBundleManifest文件名
        /// </summary>
        public const string AssetBundleManifestName = "ABManifest";

        /// <summary>
        /// 保存补丁的子目录名字
        /// </summary>
        public const string PatchRootDirectoryName = "Patch";

        /// <summary>
        /// 保存补丁各版本的子目录名字
        /// </summary>
        public const string PatchVersionSubDirectoryName = "Version";

        /// <summary>
        /// 保存补丁下载的临时目录名字
        /// </summary>
        public const string PatchDownloadSubDirectoryName = "Download";

        /// <summary>
        /// 保存补丁解压后的统一AB目录名字
        /// </summary>
        public const string PatchAssetBundleSubDirectoryName = "AB";

        /// <summary>
        /// AssetBundle文件统一后缀
        /// </summary>
        public const string AsseetBundleDefaultVariant = ".z";

        /// <summary>
        /// 默认资源
        /// </summary>
        public const string AsseetBundleDefaultData = "defaultdata" + AsseetBundleDefaultVariant;

        /// <summary>
        /// 清单文件后缀
        /// </summary>
        public const string ManifestSurfix = ".manifest";

        /// <summary>
        /// Lua单独打的AssetBundle
        /// </summary>
        public const string LuaAssetBundleName = "lua/logic.unity3d";


        /// <summary>
        /// 额外的AssetBundle的后缀
        /// </summary>
        public const string ExtraAssetBundleExtension = ".unity3d";

        /// <summary>
        /// Lua单独打的AB清单
        /// </summary>
        public const string LuaAssetBundleManifestName = LuaAssetBundleName + ManifestSurfix;

        /// <summary>
        /// 文件夹分割符号
        /// </summary>
        public const string DirectorySeparator = "/";

        /// <summary>
        /// 管理器根节点
        /// </summary>
        public const string GameBaseNodeName = "GameRoot";

        /// <summary>
        /// 默认的Lua文件后缀名
        /// </summary>
        public const string LuaSuffix = ".lua";

        /// <summary>
        /// Lua文件夹路径符号
        /// </summary>
        public const string LuaDirectorySeparator = ".";

        /// <summary>
        /// 默认的Lua文件保存路径
        /// </summary>
        public const string LuaPath = "lua" + DirectorySeparator;

        /// <summary>
        /// 是否使用屏幕Console工具
        /// </summary>
        public static bool useConsole = true;
    }
}