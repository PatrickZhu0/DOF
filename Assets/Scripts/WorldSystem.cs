using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSystem
{
    #region Singleton
    private static WorldSystem m_Instance = new WorldSystem();
    public static WorldSystem Instance {
        get { return m_Instance; }
    }
    #endregion


    //private NpcManager m_NpcMgr = new NpcManager(256);
    private UserManager m_UserMgr = new UserManager(16);


    public void Tick()
    {


    }


    public UserInfo CreateUser(int id, int resId)
    {
        UserInfo info = m_UserMgr.AddUser(id, resId);
        if (null != info)
        {
            //info.SceneContext = m_SceneContext;
            //info.SkillController = new SwordManSkillController(info, GfxModule.Skill.GfxSkillSystem.Instance);
            //info.SkillController.Init();
            //if (null != m_SpatialSystem)
            //{
            //    m_SpatialSystem.AddObj(info.SpaceObject);
            //}
        }
        return info;
    }

    public void StartGame()
    {
        //m_SceneStartTime = TimeUtility.GetServerMilliseconds();
        //UserInfo user = GetPlayerSelf();
        //if (null != user)
        //{
        //    EntityManager.Instance.DestroyUserView(user.GetId());
        //    DestroyCharacterById(user.GetId());
        //}


    }
}