using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSystem
{
    #region Singleton
    private static WorldSystem m_Instance = new WorldSystem();
    public static WorldSystem Instance
    {
        get { return m_Instance; }
    }
    #endregion

    #region Fields
    //private NpcManager m_NpcMgr = new NpcManager(256);
    private UserManager m_UserMgr = new UserManager(16);

    private UserInfo m_PlayerSelf = null;
    private int m_PlayerSelfId = -1;
    #endregion

    #region Properties
    public UserManager UserMgr { get => m_UserMgr; set => m_UserMgr = value; }


    #endregion


    public UserInfo CreatePlayerSelf(int id, int resId)
    {
        m_PlayerSelf = CreateUser(id, resId);
        m_PlayerSelfId = id;
        return m_PlayerSelf;
    }

    public UserInfo GetPlayerSelf()
    {
        return m_PlayerSelf;
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
        UserInfo user = GetPlayerSelf();
        if (null != user)
        {
            //EntityManager.Instance.DestroyUserView(user.GetId());
            //DestroyCharacterById(user.GetId());
        }
        user = CreatePlayerSelf(1, 1);
        //user.SetAIEnable(true);
        user.GetMovementStateInfo().SetPosition(1, 1, 1);
        //user.GetMovementStateInfo().SetFaceDir(unit.m_RotAngle);
        //user.SetHp(Operate_Type.OT_Absolute, 100);
        //user.SetEnergy(Operate_Type.OT_Absolute, user.GetActualProperty().EnergyMax);

    }



    public void Tick()
    {
        TickUsers();

    }

    private void TickUsers()
    {
        for (LinkedListNode<UserInfo> linkNode = m_UserMgr.Users.FirstValue; null != linkNode; linkNode = linkNode.Next)
        {
            UserInfo info = linkNode.Value;
            //if (info.SkillController != null)
            //{
            //    info.SkillController.OnTick();
            //}  
        }
        // 连击
        if (null != m_PlayerSelf)
        {
            //long curTime = TimeUtility.GetLocalMilliseconds();
            //CombatStatisticInfo combatInfo = m_PlayerSelf.GetCombatStatisticInfo();
            //if (combatInfo.LastHitTime + 1500 < curTime && combatInfo.MultiHitCount > 1)
            //{
            //    combatInfo.MultiHitCount = 1;
            //    GfxSystem.PublishGfxEvent("ge_hitcount", "ui", 0);
            //}
        }
        UserInfo player = WorldSystem.Instance.GetPlayerSelf();
        if (null != player && player.Hp <= 0)
        {
            if (player.DeadTime <= 0)
            {
                //player.GetCombatStatisticInfo().AddDeadCount(1);  //死亡计数+1
                //if (player.SkillController != null)
                //{
                //    player.SkillController.ForceInterruptCurSkill();
                //}

                //if (IsPveScene() || IsPureClientScene())
                //{
                //    ClientStorySystem.Instance.SendMessage("userkilled", player.GetId());
                //    ClientStorySystem.Instance.SendMessage("playerselfkilled", player.GetId());
                //}
                //player.DeadTime = TimeUtility.GetServerMilliseconds();
                //if (WorldSystem.Instance.IsPveScene() && !IsExpeditionScene() || WorldSystem.Instance.IsMultiPveScene())
                //{
                //    GfxSystem.PublishGfxEvent("ge_role_dead", "ui");
                //}
                //// 禁止输入
                //PlayerControl.Instance.EnableMoveInput = false;
                //PlayerControl.Instance.EnableRotateInput = false;
                //PlayerControl.Instance.EnableSkillInput = false;
            }
        }
    }

}