using System;
using System.Collections.Generic;
using System.Text;


public sealed class NpcManager
{
    public DamageDelegation OnDamage;

    private List<NpcInfo> m_DelayAdd = new List<NpcInfo>();
    private LinkedListDictionary<int, NpcInfo> m_Npcs = new LinkedListDictionary<int, NpcInfo>();
    private Queue<NpcInfo> m_UnusedNpcs = new Queue<NpcInfo>();
    //private Heap<int> m_UnusedIds = new Heap<int>(new DefaultReverseComparer<int>());
    //private Heap<int> m_UnusedClientIds = new Heap<int>(new DefaultReverseComparer<int>());
    //private int m_NpcPoolSize = 1024;

    //private const int c_StartId = 20000;
    //private const int c_MaxIdNum = 10000;
    //private const int c_StartId_Client = 30000;
    //private int m_NextInfoId = c_StartId;

    //private SceneContextInfo m_SceneContext = null;


}
