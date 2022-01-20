using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void DamageDelegation(int receiver, int caster, bool isShootDamage, bool isCritical, int hpDamage, int npDamage);
public delegate void GainMoneyDelegation(int receiver, int money);
public delegate void OnBeginAttackEvent();

public interface IShootTarget
{

}

//！！！注意，不要在这里添加属于CharacterProperty管理的内容，只能加人物状态数据，不能加属性数据，属性由属性计算得到
public enum PropertyIndex
{
    IDX_HP = 0,
    IDX_MP,
    IDX_STATE,
}

public enum HilightType
{
    kNone,
    kBurnType,
    kFrozenType,
    kShineType,
}

public enum CharacterState_Type
{
    CST_FixedPosition = 1 << 0,  // 定身，不能移动
    CST_Silence = 1 << 1,  // 沉默，不能释放技能
    CST_Invincible = 1 << 2,  // 无敌
    CST_BODY = 1 << 3,  // ？
    CST_Sleep = 1 << 4,  // 昏迷，不能移动，不能攻击，不能放技能
    CST_DamageAbsorb = 1 << 5,  // 伤害吸收，在计算伤害时需要考虑
    CST_Hidden = 1 << 6,  // 隐身的
    CST_Opened = 1 << 7,  // 用于npc带物品的，表明已经捡过了
    CST_Disarm = 1 << 8,  // 缴械，不能开枪
}


/**
 * @brief 角色基类
 */
public class CharacterInfo : IShootTarget
{
    public struct AttackerInfo
    {
        public int m_AttackerType;
        public long m_AttackTime;
        public int m_HpDamage;
        public int m_NpDamage;
    }

    #region Fields

    protected int m_Id = 0;
    /// <summary>
    /// 单位ID，在地图中赋予的id，用于剧情查找
    /// </summary>
    protected int m_UnitId = 0;
    protected int m_LinkId = 0;
    protected int m_OwnerId = -1;
    protected string m_Name = "";
    protected int m_Level = 1;

    protected float m_Combat2IdleTime = 3;
    protected int m_Combat2IdleSkill = 0;
    protected string m_Idle2CombatWeaponMoves = "";

    private bool m_LevelChanged = false;
    private int m_Hp = 0;
    private int m_Rage = 0;
    private int m_Energy = 0;
    private float m_HpMaxCoefficient = 1;
    private float m_EnergyMaxCoefficient = 1;
    private float m_AttackRangeCoefficient = 1;
    private float m_VelocityCoefficient = 1;
    private float m_ViewRange = 0;
    private float m_GohomeRange = 0;
    private bool m_GfxDead = false;
    private bool m_SuperArmor = false;  // 可被破除的霸体
    private bool m_UltraArmor = false;  // 不可被破除的霸体
    private bool m_IsArmorChanged = true;
    protected string m_Model = "";
    protected string m_BornEffect = "";


    protected bool m_AIEnable = true;

    private long m_ReleaseTime = 0;  //尸体存在时间
    private long m_EmptyBloodTime = 0;
    private long m_DeadTime = 0;
    protected bool m_CauseStiff = true;
    protected bool m_AcceptStiff = true;
    protected bool m_AcceptStiffEffect = true;

    //基础属性值
    protected CharacterProperty m_BaseProperty;
    //当前属性值
    protected CharacterProperty m_ActualProperty;

    private MovementStateInfo m_MovementStateInfo = new MovementStateInfo();

    #endregion

    #region Properties
    public int Hp { get => m_Hp; }
    public long DeadTime { get => m_DeadTime; set => m_DeadTime = value; }


    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id"></param>
    public CharacterInfo(int id)
    {
        m_Id = id;
        m_UnitId = 0;
        m_LinkId = 0;
        m_AIEnable = true;
        m_BaseProperty = new CharacterProperty();
        m_ActualProperty = new CharacterProperty();

        m_ReleaseTime = 0;
    }

    /// <summary>
    /// 获取id
    /// </summary>
    /// <returns></returns>
    public int GetId()
    {
        return m_Id;
    }

    /// <summary>
    /// 单位id
    /// </summary>
    /// <returns></returns>
    public int GetUnitId()
    {
        return m_UnitId;
    }



    public MovementStateInfo GetMovementStateInfo()
    {
        return m_MovementStateInfo;
    }
}
