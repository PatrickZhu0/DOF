using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed class UserManager
{
    public DamageDelegation OnDamage;
    public GainMoneyDelegation OnGainMoney;

    private LinkedListDictionary<int, UserInfo> m_Users = new LinkedListDictionary<int, UserInfo>();
    private Queue<UserInfo> m_UnusedUsers = new Queue<UserInfo>();
    private int m_UserPoolSize = 1024;

    private const int c_StartId = 1;
    private const int c_MaxIdNum = 19999;
    private int m_NextInfoId = c_StartId;

    public UserManager(int poolSize)
    {
        m_UserPoolSize = poolSize;
    }

    public LinkedListDictionary<int, UserInfo> Users {
        get { return m_Users; }
    }

    public UserInfo AddUser(int resId)
    {
        //UserInfo user = NewUserInfo();
        //user.LoadData(resId);
        //m_Users.AddLast(user.GetId(), user);
        //return user;
        return new UserInfo(resId);
    }

    public UserInfo AddUser(int id, int resId)
    {
        //UserInfo user = NewUserInfo(id);
        //user.LoadData(resId);
        //m_Users.AddLast(user.GetId(), user);
        //return user;
        return new UserInfo(resId);
    }

}
