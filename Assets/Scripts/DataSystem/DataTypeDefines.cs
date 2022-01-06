using System;
using System.Collections.Generic;

namespace FStarWars
{

    public enum Animation_Type
    {
        // 无动作
        AT_None = -1,

        AT_Waiting = 0,

        AT_Move = 1,

        AT_Sit = 2,

        AT_Damage1,
        AT_Damage2,

        AT_Down,
        AT_Overturn,
        AT_Jump,
        AT_Jumpattack,
        AT_Rest,

        AT_Throw1_1,
        AT_Throw1_2,
        AT_Throw2_1,
        AT_Throw2_2,

        AT_Dash,
        AT_Dashattack,
        AT_Getitem,
        AT_Buff,

        AT_SimpleRest,
        AT_SimpleMove

    }
}

