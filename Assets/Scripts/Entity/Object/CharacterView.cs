using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public partial class CharacterView
{
    private int m_Actor = 0;
    private int m_ObjId = 0;
    private SharedGameObjectInfo m_ObjectInfo = new SharedGameObjectInfo();

    protected long m_LastLeaveCombatTime = 0;
    protected bool m_IsCombat2IdleChanging = true;
    protected bool m_IsWeaponMoved = false;

    private const string c_CylinderName = "1_Cylinder";
    private const float c_AffectPlayerSelfDistanceSquare = 900;
    private List<string> m_CurWeaponName = new List<string>();

    private bool m_Visible = true;
    private bool m_CanAffectPlayerSelf = true;

    //private ScriptRuntime.Vector4 m_NormalColor = new ScriptRuntime.Vector4(1, 1, 1, 1);
    //private ScriptRuntime.Vector4 m_BurnColor = new ScriptRuntime.Vector4(0.75f, 0.2f, 0.2f, 1);
    //private ScriptRuntime.Vector4 m_FrozonColor = new ScriptRuntime.Vector4(0.2f, 0.2f, 0.75f, 1);
    //private ScriptRuntime.Vector4 m_ShineColor = new ScriptRuntime.Vector4(0.2f, 0.75f, 0.2f, 1);
    private Dictionary<string, uint> effect_map_ = new Dictionary<string, uint>();


    public int Actor
    {
        get { return m_Actor; }
    }
    public int ObjId
    {
        get { return m_ObjId; }
    }
    public SharedGameObjectInfo ObjectInfo
    {
        get { return m_ObjectInfo; }
    }



    private void Init()
    {
        //m_NormalColor = new ScriptRuntime.Vector4(1, 1, 1, 1);
        //m_BurnColor = new ScriptRuntime.Vector4(0.75f, 0.2f, 0.2f, 1);
        //m_FrozonColor = new ScriptRuntime.Vector4(0.2f, 0.2f, 0.75f, 1);
        //m_ShineColor = new ScriptRuntime.Vector4(0.2f, 0.75f, 0.2f, 1);
        m_Actor = 0;

        //m_CurActionConfig = null;
    }

    protected void CreateActor(int objId, string model, Vector3 pos, float dir, float scale = 1.0f)
    {
        Init();

        m_ObjId = objId;
        m_Actor = GameObjectIdManager.Instance.GenNextId();
        m_ObjectInfo.m_ActorId = m_Actor;
        m_ObjectInfo.m_LogicObjectId = objId;
        m_ObjectInfo.X = pos.x;
        m_ObjectInfo.Y = pos.y;
        m_ObjectInfo.Z = pos.z;
        m_ObjectInfo.FaceDir = dir;
        m_ObjectInfo.Sx = scale;
        m_ObjectInfo.Sy = scale;
        m_ObjectInfo.Sz = scale;
        GfxSystem.CreateGameObject(m_Actor, model, m_ObjectInfo);
    }
}

