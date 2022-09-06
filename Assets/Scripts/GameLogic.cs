using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogicThread s_LogicThread = new GameLogicThread();

    private bool m_IsInit = false;

    private float AccumilatedTime = 0f;
    private float FrameLength = 0.05f; //50 miliseconds

    //called once per unity frame
    internal void Update() {
        //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
        AccumilatedTime = AccumilatedTime + Time.deltaTime;

        //in case the FPS is too slow, we may need to update the game multiple times a frame
        while(AccumilatedTime > FrameLength) {
            s_LogicThread.OnTick();
            AccumilatedTime = AccumilatedTime - FrameLength;
        }
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
