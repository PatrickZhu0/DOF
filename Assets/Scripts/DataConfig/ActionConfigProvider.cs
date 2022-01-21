using System;
using System.Collections.Generic;
using System.Text;

public interface IData
{
    int GetId();
}

public class AnimDictionaryMgr<TData> where TData : IData, new()
{

}


public class ActionConfigProvider
{
    public AnimDictionaryMgr<Data_ActionConfig> ActionConfigMgr
    {
        get { return m_ActionConfigMgr; }
    }
    public Data_ActionConfig GetDataById(int id)
    {
        return m_ActionConfigMgr.GetDataById(id);
    }

    public Data_ActionConfig GetCharacterCurActionConfig(List<int> action_list)
    {
        for (int i = 0; i < action_list.Count; ++i)
        {
            Data_ActionConfig action_config = GetDataById(action_list[i]);
            if (action_config != null)
            {
                return action_config;
            }
        }
        return null;
    }

    public void Load(string file, string root)
    {
        m_ActionConfigMgr.CollectDataFromDBC(file, root);
    }

    private AnimDictionaryMgr<Data_ActionConfig> m_ActionConfigMgr = new AnimDictionaryMgr<Data_ActionConfig>();

    public static ActionConfigProvider Instance
    {
        get { return s_Instance; }
    }
    private static ActionConfigProvider s_Instance = new ActionConfigProvider();
}


public class Data_ActionConfig : IData
{
    int m_ModelId;

    public Dictionary<string, Anim> m_ActionContainer;


    public bool CollectData(string path)
    {

        m_ActionContainer = new Dictionary<string, Anim>();

        m_ActionContainer["stay"] = ExtractAction(path, "stay");

        return true;
    }

    public virtual int GetId()
    {
        return m_ModelId;
    }




}



