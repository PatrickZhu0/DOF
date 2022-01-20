using FStarWars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum IdleState
{
    kNotIdle, //未进入
    kReady, //空闲开始计时
    kBegin, //开始播放空闲动画
}

public sealed class CharacterAnimationInfo
{
    public bool IsPlayChangeWeapon;
    public bool IsPlayDead;
    public bool IsPlayTaunt;

    public bool IsMoving;
    public bool CanMove;
    public MovementMode MoveMode;
    public bool IsCombat;
    public float m_Speed;

    public void Reset()
    {
        //IsPlayChangeWeapon = false;
        //IsPlayDead = false;
        //IsPlayTaunt = false;
        //IsMoving = false;
        //CanMove = true;
        //IsCombat = false;
        //m_Speed = 0;
    }
    public bool IsIdle()
    {
        //return !IsMoving && !IsPlayChangeWeapon && !IsPlayDead && !IsPlayTaunt;
        return true;
    }
}

public delegate void AnimationStopCallback(string anim_name);

public partial class CharacterView
{
    protected bool m_IsCombatState = false;

    protected CharacterAnimationInfo m_CharacterAnimationInfo = new CharacterAnimationInfo();
    protected IdleState m_IdleState = IdleState.kNotIdle;
    protected long m_BeginIdleTime = 0;
    protected long m_IdleInterval = 0;



}