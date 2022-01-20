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



}
