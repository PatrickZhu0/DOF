using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;


//public delegate void LogicLogCallbackDelegation(bool isError, string format, object[] args);
public delegate void BeforeLoadSceneDelegation(string curName, string targetName, int targetSceneId);
public delegate void AfterLoadSceneDelegation(string targetName, int targetSceneId);
public sealed partial class GfxSystem
{
    private class GameObjectInfo
    {
        public GameObject ObjectInstance;
        public SharedGameObjectInfo ObjectInfo;

        public GameObjectInfo(GameObject o, SharedGameObjectInfo i)
        {
            ObjectInstance = o;
            ObjectInfo = i;
        }
    }



    private object m_SyncLock = new object();
    private LinkedListDictionary<int, GameObjectInfo> m_GameObjects = new LinkedListDictionary<int, GameObjectInfo>();
    private Dictionary<GameObject, int> m_GameObjectIds = new Dictionary<GameObject, int>();
    private Action<bool, string, object[]> m_LogicLogCallback;



    private bool m_SceneResourcePrepared = false;
    private bool m_SceneResourceStartPrepare = false;
    private float m_SceneResourcePreparedProgress = 0;

    private bool m_LoadScenePaused = false;
    private UnityEngine.AsyncOperation m_LoadingBarAsyncOperation = null;
    private UnityEngine.AsyncOperation m_LoadingLevelAsyncOperation = null;

    private GameObjectInfo m_PlayerSelf = null;

    private BeforeLoadSceneDelegation m_OnBeforeLoadScene;
    private AfterLoadSceneDelegation m_OnAfterLoadScene;

    private string m_LoadingBarScene = "";
    private int m_TargetSceneId = 0;
    private HashSet<int> m_TargetSceneLimitList = null;
    private string m_TargetScene = "";
    private int m_TargetChapter = 0;
    private float m_LoadingProgress = 0;
    private string m_LoadingTip = "";
    private string m_VersionInfo = "";

    private long m_LastLogTime = 0;

    private float c_MinTerrainHeight = 120.0f;


    private GfxSystem() { }


    //初始化阶段调用的函数
    private void InitImpl()
    {

    }
    private void TickImpl()
    {
        long curTime = (long)UnityEngine.Time.realtimeSinceStartup;
        m_LastLogTime = curTime;

        HandleInput();
    }
    private void ReleaseImpl()
    {

    }


    internal GameObject GetGameObject(int id)
    {
        GameObject ret = null;
        if (m_GameObjects.Contains(id))
            ret = m_GameObjects[id].ObjectInstance;
        return ret;
    }

    private int GetGameObjectId(GameObject obj)
    {
        int ret = 0;
        if (m_GameObjectIds.ContainsKey(obj))
        {
            ret = m_GameObjectIds[obj];
        }
        return ret;
    }

    internal SharedGameObjectInfo GetSharedGameObjectInfo(int id)
    {
        SharedGameObjectInfo ret = null;
        if (m_GameObjects.Contains(id))
            ret = m_GameObjects[id].ObjectInfo;
        return ret;
    }
    internal SharedGameObjectInfo GetSharedGameObjectInfo(GameObject obj)
    {
        int id = GetGameObjectId(obj);
        return GetSharedGameObjectInfo(id);
    }
    internal bool ExistGameObject(GameObject obj)
    {
        int id = GetGameObjectId(obj);
        return id > 0;
    }



    private void UpdateGameObjectLocalPosition2DImpl(int id, float x, float z, bool attachTerrain)
    {
        GameObject obj = GetGameObject(id);
        if (null != obj)
        {
            float y = 0;
            //if (attachTerrain)
            //    y = SampleTerrainHeight(x, z);
            //else
            //    y = obj.transform.localPosition.y;
            y = obj.transform.localPosition.y;
            obj.transform.localPosition = new Vector3(x, y, z);
        }
    }

    private void RememberGameObject(int id, GameObject obj)
    {
        RememberGameObject(id, obj, null);
    }

    private void RememberGameObject(int id, GameObject obj, SharedGameObjectInfo info)
    {
        if (m_GameObjects.Contains(id))
        {
            GameObject oldObj = m_GameObjects[id].ObjectInstance;
            oldObj.SetActive(false);
            m_GameObjectIds.Remove(oldObj);
            GameObject.Destroy(oldObj);
            m_GameObjects[id] = new GameObjectInfo(obj, info);
        }
        else
        {
            m_GameObjects.AddLast(id, new GameObjectInfo(obj, info));
        }
        if (null != info)
        {
            if (!info.m_SkinedMaterialChanged)
            {
                SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    info.m_SkinedOriginalMaterials.Add(renderer.materials);
                }
            }
            if (!info.m_MeshMaterialChanged)
            {
                MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer renderer in renderers)
                {
                    info.m_MeshOriginalMaterials.Add(renderer.materials);
                }
            }
        }
        m_GameObjectIds.Add(obj, id);
    }

    private void ForgetGameObject(int id, GameObject obj)
    {
        SharedGameObjectInfo info = GetSharedGameObjectInfo(id);
        if (null != info)
        {
            info.m_SkinedOriginalMaterials.Clear();
            info.m_MeshOriginalMaterials.Clear();
        }
        m_GameObjects.Remove(id);
        m_GameObjectIds.Remove(obj);
    }

    private void CreateGameObjectImpl(int id, string resource, SharedGameObjectInfo info)
    {
        if (null != info)
        {
            try
            {
                Vector3 pos = new Vector3(info.X, info.Y, info.Z);
                //if (!info.IsFloat)
                //    pos.y = SampleTerrainHeight(pos.x, pos.z);
                //Quaternion q = Quaternion.Euler(0, RadianToDegree(info.FaceDir), 0);
                GameObject obj = ResourceManager.Instance.NewObject(resource) as GameObject;
                if (null != obj)
                {
                    if (null != obj.transform)
                    {
                        obj.transform.position = pos;
                        //obj.transform.localRotation = q;
                        obj.transform.localScale = new Vector3(info.Sx, info.Sy, info.Sz);
                    }
                    RememberGameObject(id, obj, info);
                    obj.SetActive(true);
                }
                else
                {
                    //CallLogicErrorLog("CreateGameObject {0} can't load resource", resource);
                }
            }
            catch (Exception ex)
            {
                //CallGfxErrorLog("CreateGameObject {0} throw exception:{1}\n{2}", resource, ex.Message, ex.StackTrace);
            }
        }
    }
}
