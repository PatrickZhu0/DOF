using System;
using System.Collections.Generic;
using System.Text;

public interface IData
{
    int GetId();
}


public class Data_ActionConfig : IData
{
    int m_ModelId;


    public virtual int GetId()
    {
        return m_ModelId;
    }




}



