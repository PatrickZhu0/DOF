using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UserView : CharacterView
{
    private UserInfo m_User = null;
    private int m_IndicatorActor = 0;
    private float m_IndicatorDir = 0;
    private bool m_IndicatorVisible = false;
    private int m_IndicatorTargetType = 1;

    protected override CharacterInfo GetOwner()
    {
        return m_User;
    }

    public void Create(UserInfo user)
    {
        //Init();
        if (null != user)
        {
            m_User = user;
            //ObjectInfo.CampId = m_User.GetCampId();
            MovementStateInfo msi = m_User.GetMovementStateInfo();
            Vector3 pos = msi.GetPosition3D();
            float dir = msi.GetFaceDir();
            CreateActor(m_User.GetId(), "Swordman", pos, dir);

            //CreateActor(m_User.GetId(), m_User.GetModel(), pos, dir);
            //CreateIndicatorActor(m_User.GetId(), m_User.GetIndicatorModel());
            InitAnimationSets();
            ObjectInfo.IsPlayer = true;
            //if (user.GetId() == WorldSystem.Instance.PlayerSelfId)
            //{
            //    GfxSystem.MarkPlayerSelf(Actor);
            //}
        }
    }

    public void Update()
    {
        //UpdateAttr();
        //UpdateSpatial();
        UpdateAnimation();
        //UpdateIndicator();
    }

    private void UpdateAnimation()
    {
        //if (!CanAffectPlayerSelf) return;
        //if (null != m_User)
        //{
        //    UpdateState();
        //    if (ObjectInfo.IsGfxAnimation)
        //    {
        //        m_CharacterAnimationInfo.Reset();
        //        m_IdleState = IdleState.kNotIdle;
        //        return;
        //    }
        //    UpdateMoveAnimation();
        //    UpdateDead();
        //    UpdateIdle();
        //}
    }
}