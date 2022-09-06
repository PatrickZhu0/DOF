using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ٵ��̣߳������Ժ��Ϊ�߳�
public class GameLogicThread
{
    private long m_LastLogTime = 0;

    public void OnStart()
    {

    }

    public void OnTick()
    {
        if (!GameControler.IsPaused)
        {
            //NetworkSystem.Instance.Tick();
            //LobbyNetworkSystem.Instance.Tick();
            PlayerControl.Instance.Tick();
            WorldSystem.Instance.Tick();
        }

    }

    public void OnQuit()
    {
    }
}
