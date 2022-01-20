using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UserView : CharacterView
{
    private UserInfo m_User = null;
    private int m_IndicatorActor = 0;
    private float m_IndicatorDir = 0;
    private bool m_IndicatorVisible = false;
    private int m_IndicatorTargetType = 1;

    internal void Create(UserInfo obj)
    {
        throw new NotImplementedException();
    }

    internal void Update()
    {
        throw new NotImplementedException();
    }
}