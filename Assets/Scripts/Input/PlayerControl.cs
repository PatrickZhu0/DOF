using System;

using System.Collections.Generic;
using UnityEngine;


/*
  * TODO: 
  * 1. 由於多數操作依賴playerself的存在, 而playerself可能未創建或者已經刪除
  *    所以是否可以考慮外部輸入還是設計成可以與某個obj綁定?
  * 2. 組合鍵, 設定一組鍵的狀態同時彙報, 例如wasd的狀態希望能夠在一個囘調函數中得到通知
  *    (不適用主動查詢的情況下), 使用主動查詢就必須存在一個Tick函數
  *    
  */
public class PlayerControl
{

    // static methods
    private static PlayerControl inst_ = new PlayerControl();
    public static PlayerControl Instance { get { return inst_; } }

    // members
    private bool m_IsJoystickControl = false;
    private float m_lastDir = -1f;
    private float m_lastMoveDir = -1f;
    private bool m_LastTickIsMoving = false;
    private bool m_LastTickIsSkillMoving = false;
    private int m_lastSelectObjId = -1;
    private long m_LastTickTime = 0;
    private long m_LastTickIntervalMs = 0;
    private PlayerMovement pm_;

    private Vector2 old_mouse_pos_;
    private Vector2 mouse_pos_;

    private float c_2PI = (float)Math.PI * 2;

    // properties
    public bool EnableMoveInput { get; set; }
    public bool EnableRotateInput { get; set; }
    public bool EnableSkillInput { get; set; }

    // methods
    public PlayerControl()
    {
        pm_ = new PlayerMovement();
        EnableMoveInput = true;
        EnableRotateInput = true;
        EnableSkillInput = true;
    }

    public void Reset()
    {
        EnableMoveInput = true;
        EnableRotateInput = true;
        EnableSkillInput = true;
    }
    public void Init()
    {

    }

    public void Tick()
    {
        long now = (long)UnityEngine.Time.realtimeSinceStartup;
        m_LastTickIntervalMs = now - m_LastTickTime;

        m_LastTickTime = now;


        pm_.Update(EnableMoveInput);

        UserInfo playerself = WorldSystem.Instance.GetPlayerSelf();
        if (null == playerself)
            return;

        if (!m_IsJoystickControl)
        {
            pm_.Update(EnableMoveInput);
        }
        MovementStateInfo msi = playerself.GetMovementStateInfo();
        Vector3 pos = msi.GetPosition3D();

        bool reface = false;
        if (m_LastTickIsSkillMoving && !msi.IsSkillMoving)
        {
            reface = true;
        }

        Vector3 mouse_pos = new Vector3(GfxSystem.GetMouseX(), GfxSystem.GetMouseY(), GfxSystem.GetMouseZ());
        if (pm_.MotionStatus == PlayerMovement.Motion.Moving || pm_.JoyStickMotionStatus == PlayerMovement.Motion.Moving)
        {
            if (pm_.MotionChanged || pm_.JoyStickMotionChanged || !m_LastTickIsMoving)
            {
                //playerself.SkillController.AddBreakSkillTask();
                float moveDir = RoundMoveDir(pm_.MoveDir);
                if (!m_LastTickIsMoving || (moveDir != m_lastMoveDir))
                {
                    msi.SetMoveDir(moveDir);
                    msi.IsMoving = true;
                    msi.TargetPosition = Vector3.zero;
                }
                m_lastDir = pm_.MoveDir;
                m_lastMoveDir = moveDir;
            }
            m_LastTickIsMoving = true;
        }
        else
        {
            if (m_LastTickIsMoving)
            {
                //playerself.SkillController.CancelBreakSkillTask();
                playerself.GetMovementStateInfo().IsMoving = false;

                if (EnableRotateInput)
                {
                    if (reface)
                    {
                        msi.SetFaceDir(m_lastDir);
                        msi.SetWantFaceDir(m_lastDir);
                    }
                }
            }
            m_LastTickIsMoving = false;
        }
        m_LastTickIsSkillMoving = msi.IsSkillMoving;

        old_mouse_pos_ = mouse_pos_;
        mouse_pos_.x = GfxSystem.GetMouseX();
        mouse_pos_.y = GfxSystem.GetMouseY();
    }

    private float RoundMoveDir(float dir)
    {
        const float c_PiDiv8 = (float)Math.PI / 8;
        const float c_PiDiv16 = (float)Math.PI / 16;
        int intDir = (int)(dir / c_PiDiv16);
        intDir = (intDir + 1) % 32;
        intDir /= 2;
        return intDir * c_PiDiv8;
    }


    private void OnPlaySkill(int key_code, int what)
    {

    }


    private void FindPath(UserInfo playerself, Vector3 targetpos)
    {

    }

    private void UpdateTowards(UserInfo playerself, float towards)
    {
        if (null != playerself && float.NegativeInfinity != towards)
        {
            playerself.GetMovementStateInfo().SetFaceDir(towards);
            playerself.GetMovementStateInfo().SetMoveDir(towards);
        }
    }
}

class PlayerMovement
{
    private enum KeyIndex
    {
        W = 0,
        A,
        S,
        D
    }
    public enum KeyHit
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        Other = 16,
    }

    public enum Motion
    {
        Moving,
        Stop,
    }

    public PlayerMovement()
    {
        MoveDir = 0;
        MotionStatus = Motion.Stop;
        MotionChanged = false;
        JoyStickMotionStatus = Motion.Stop;
        JoyStickMotionChanged = false;
        last_key_hit_ = KeyHit.None;
    }

    public void Update(bool move_enable)
    {
        UserInfo playerself = WorldSystem.Instance.GetPlayerSelf();
        //if (playerself == null || playerself.IsDead())
        //{
        //    return;
        //}
        KeyHit kh = KeyHit.None;
        if (move_enable)
        {
            if (Input.GetKey(KeyCode.UpArrow))
                kh |= KeyHit.Up;
            if (Input.GetKey(KeyCode.LeftArrow))
                kh |= KeyHit.Left;
            if (Input.GetKey(KeyCode.DownArrow))
                kh |= KeyHit.Down;
            if (Input.GetKey(KeyCode.RightArrow))
                kh |= KeyHit.Right;
        }

        Motion m = kh == KeyHit.None ? Motion.Stop : Motion.Moving;
        MotionChanged = MotionStatus != m || last_key_hit_ != kh;

        last_key_hit_ = kh;
        MotionStatus = m;
        MoveDir = CalcMoveDir(kh);
        Debug.LogError(MoveDir);
        if (MoveDir < 0)
        {
            MotionStatus = Motion.Stop;
        }
    }

    public float MoveDir { get; set; }
    public Motion MotionStatus { get; set; }
    public bool MotionChanged { get; set; }
    public Motion JoyStickMotionStatus { get; set; }
    public bool JoyStickMotionChanged { get; set; }

    private KeyCode GetKeyCode(KeyIndex index)
    {
        KeyCode ret = KeyCode.W;
        if (index >= KeyIndex.W && index <= KeyIndex.D)
        {
            KeyCode[] list = s_Normal;
            ret = list[(int)index];
        }
        return ret;
    }

    /**
      * @brief 计算移动方向
      *           0       
      *          /|\
      *           |
      *3π/2 -----*-----> π/2
      *           |
      *           |
      *           |
      *           π
      */
    private float CalcMoveDir(KeyHit kh)
    {
        return s_MoveDirs[(int)kh];
    }

    public KeyHit last_key_hit_;

    private static readonly KeyCode[] s_Normal = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private static readonly KeyCode[] s_Blue = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W };
    private static readonly KeyCode[] s_Red = new KeyCode[] { KeyCode.D, KeyCode.W, KeyCode.A, KeyCode.S };
    //                                                          N   U  D        UD  L            UL           DL
    private static readonly float[] s_MoveDirs = new float[] { -1,  0, (float)Math.PI, -1, 3*(float)Math.PI/2, 7*(float)Math.PI/4, 5*(float)Math.PI/4, 
      //                    UDL          R          UR         DR           UDR        LR  ULR  LRD      UDLR
                            3*(float)Math.PI/2, (float)Math.PI/2, (float)Math.PI/4, 3*(float)Math.PI/4, (float)Math.PI/2, -1, 0,   (float)Math.PI, -1 };
}