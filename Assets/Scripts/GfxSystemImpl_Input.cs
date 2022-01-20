using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public sealed partial class GfxSystem
{

    private bool m_IsLastHitUi;
    private Vector3 m_LastMousePos;
    private Vector3 m_CurMousePos;
    private Vector3 m_MouseRayPoint;

    //private bool[] m_KeyPressed = new bool[(int)Keyboard.Code.MaxNum];
    //private bool[] m_ButtonPressed = new bool[(int)Mouse.Code.MaxNum];
    private HashSet<int> m_KeysForListen = new HashSet<int>();

    private Dictionary<int, Action<int, int>> m_KeyboardHandlers = new Dictionary<int, Action<int, int>>();
    private Dictionary<int, Action<int, int>> m_MouseHandlers = new Dictionary<int, Action<int, int>>();

    public bool IsLastHitUi {
        get { return m_IsLastHitUi; }
        internal set { m_IsLastHitUi = value; }
    }
    private float GetMouseXImpl()
    {
        return m_CurMousePos.x;
    }
    private float GetMouseYImpl()
    {
        return m_CurMousePos.y;
    }
    private float GetMouseZImpl()
    {
        return m_CurMousePos.z;
    }
    private float GetMouseRayPointXImpl()
    {
        return m_MouseRayPoint.x;
    }
    private float GetMouseRayPointYImpl()
    {
        return m_MouseRayPoint.y;
    }
    private float GetMouseRayPointZImpl()
    {
        return m_MouseRayPoint.z;
    }

    private void HandleInput()
    {
        m_LastMousePos = m_CurMousePos;
        m_CurMousePos = Input.mousePosition;

        if ((m_CurMousePos - m_LastMousePos).sqrMagnitude >= 1 && null != Camera.main)
        {
            Ray ray = Camera.main.ScreenPointToRay(m_CurMousePos);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                m_MouseRayPoint = hitInfo.point;
            }
        }

        //foreach (int c in m_KeysForListen)
        //{
        //    if (Input.GetKeyDown((KeyCode)c))
        //    {
        //        m_KeyPressed[c] = true;
        //        FireKeyboard(c, (int)Keyboard.Event.Down);
        //    }
        //    else if (Input.GetKeyUp((KeyCode)c))
        //    {
        //        m_KeyPressed[c] = false;
        //        FireKeyboard(c, (int)Keyboard.Event.Up);
        //    }
        //}
    }



}
