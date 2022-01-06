using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameDLL
{
    public interface IEditorTools
    {
        bool DevelopMode();
        bool SimulatorRunningCheck(bool editor);
        bool CheckRunningOrSimulator(bool editor);
        /// <summary>
        /// 注册一个撤销的操作
        /// </summary>
        /// <param name="objectToUndo">需要撤销的对象</param>
        /// <param name="name">撤销动作的名称</param>
        void RegisterCreatedObjectUndo(Object objectToUndo, string name);

        Object EditorLoadObject(bool editor, AssetDataInfo info, string guid, string assetName, Type assetType);
        Object EditorLoadObjectFromGUID(string guid, Type assetType);
        T LoadAsset<T>(string abname, string assetname) where T : Object;
        Object LoadAsset(Type type, string abname, string assetname);
        IEnumerator LoadAssetAsync(string abname, string assetname, AssetBundleData assetIn, ResourceManager.LoadAsyncComplete complete = null, ResourceManager.LoadAsyncPreComplete preComplete = null);
        Scene LoadScene(string abname, string scenename, bool single);
        AsyncOperation LoadSceneAsync(string abname, string scenenname, bool single);
    }
}
