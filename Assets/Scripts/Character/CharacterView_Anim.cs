using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public bool IsCombat;
    public float m_Speed;

    public void Reset()
    {
        IsPlayChangeWeapon = false;
        IsPlayDead = false;
        IsPlayTaunt = false;
        IsMoving = false;
        CanMove = true;
        IsCombat = false;
        m_Speed = 0;
    }
    public bool IsIdle()
    {
        return !IsMoving && !IsPlayChangeWeapon && !IsPlayDead && !IsPlayTaunt;
    }
}

public delegate void AnimationStopCallback(string anim_name);

public partial class CharacterView
{
    //这里可以传一个序列帧的序号
    
    public void PlayAnimation(int type, float speed)
    {

    }

}
