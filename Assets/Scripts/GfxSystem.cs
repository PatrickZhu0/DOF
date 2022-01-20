using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed partial class GfxSystem
{
    internal static GfxSystem Instance {
        get {
            return s_Instance;
        }
    }
    private static GfxSystem s_Instance = new GfxSystem();


    //引擎线程调用的方法，不要在逻辑线程调用
    public static void Init()
    {
        s_Instance.InitImpl();
    }
    public static void Release()
    {
        s_Instance.ReleaseImpl();
    }
    public static void Tick()
    {
        s_Instance.TickImpl();
    }

    public static float GetMouseX()
    {
        return s_Instance.GetMouseXImpl();
    }
    public static float GetMouseY()
    {
        return s_Instance.GetMouseYImpl();
    }
    public static float GetMouseZ()
    {
        return s_Instance.GetMouseZImpl();
    }
    public static float GetMouseRayPointX()
    {
        return s_Instance.GetMouseRayPointXImpl();
    }
    public static float GetMouseRayPointY()
    {
        return s_Instance.GetMouseRayPointYImpl();
    }
    public static float GetMouseRayPointZ()
    {
        return s_Instance.GetMouseRayPointZImpl();
    }
}
