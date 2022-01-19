using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogicThread s_LogicThread = new GameLogicThread();

    private bool m_IsInit = false;



    internal void FixedUpdate()
    {
        s_LogicThread.OnTick();
    }



    internal void Update()
    {
        //UnityEngine.Debug.Log("CurScene:" + Application.loadedLevelName);
        try
        {
            if (m_IsInit)
            {
                GameControler.TickGame();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(string.Format("GameLogic.Update throw exception:{0}\n{1}", ex.Message, ex.StackTrace));
        }
    }


}
