using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FStarWars
{
    public class DataActionConfig
    {
        public class DataActionInfo
        {
            public string m_AnimName;
            public string m_SoundId;
        }

        public int m_ModelId;
        public DataActionInfo m_Default;
        public Dictionary<Animation_Type, List<DataActionInfo>> m_ActionContainer;
        public float m_CombatStdSpeed;
        public float m_ForwardStdSpeed;
        public float m_SlowStdSpeed;
        public float m_FastStdSpeed;
        public string m_ActionPrefix;

    }

}

