using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDLL
{
    /// <summary>
    /// by Rick
    /// 仅存在于内存中的寄存数据
    /// Lua可能因为重启而无法记录内存数据，靠C#层来寄存下
    /// </summary>
    public static class MemData
    {
        static Dictionary<string, int> sIntTable = new Dictionary<string, int>();
        static Dictionary<string, string> sStringTable = new Dictionary<string, string>();
        static Dictionary<string, bool> sBooleanTable = new Dictionary<string, bool>();
        static Dictionary<string, float> sFloatTable = new Dictionary<string, float>();

        #region 整形数据
        public static void SetInt(string key, int value)
        {
            sIntTable[key] = value;
        }

        public static bool HasInt(string key)
        {
            return sIntTable.ContainsKey(key);
        }

        public static void DeleteInt(string key)
        {
            if(!HasInt(key))
            {
                GLog.ErrorFormat("DeleteInt key {0} not set yet", key);
            }
            sIntTable.Remove(key);
        }

        public static int GetInt(string key)
        {
            int o = 0;
            if(!sIntTable.TryGetValue(key, out o))
            {
                GLog.ErrorFormat("GetInt key {0} not set yet", key);
            }
            return o;
        }
        #endregion

        #region 字符串数据
        public static void SetString(string key, string value)
        {
            sStringTable[key] = value;
        }

        public static bool HasString(string key)
        {
            return sStringTable.ContainsKey(key);
        }

        public static void DeleteString(string key)
        {
            if (!HasString(key))
            {
                GLog.ErrorFormat("DeleteString key {0} not set yet", key);
            }
            sStringTable.Remove(key);
        }

        public static string GetString(string key)
        {
            string o = string.Empty;
            if(!sStringTable.TryGetValue(key, out o))
            {
                GLog.ErrorFormat("GetString key {0} not set yet", key);
            }
            return o;
        }
        #endregion

        #region 布尔数据
        public static void SetBool(string key, bool value)
        {
            sBooleanTable[key] = value;
        }

        public static bool HasBoolean(string key)
        {
            return sBooleanTable.ContainsKey(key);
        }

        public static void DeleteBoolean(string key)
        {
            if (!HasBoolean(key))
            {
                GLog.ErrorFormat("DeleteBoolean key {0} not set yet", key);
            }
            sBooleanTable.Remove(key);
        }

        public static bool GetBoolean(string key)
        {
            bool o = false;
            if (!sBooleanTable.TryGetValue(key, out o))
            {
                GLog.ErrorFormat("GetInt key {0} not set yet", key);
            }
            return o;
        }
        #endregion

        #region 浮点数数据
        public static void SetFloat(string key, float value)
        {
            sFloatTable[key] = value;
        }

        public static bool HasFloat(string key)
        {
            return sFloatTable.ContainsKey(key);
        }

        public static void DeleteFloat(string key)
        {
            if (!HasFloat(key))
            {
                GLog.ErrorFormat("DeleteFloat key {0} not set yet", key);
            }
            sFloatTable.Remove(key);
        }

        public static float GetFloat(string key)
        {
            float o = 0;
            if (!sFloatTable.TryGetValue(key, out o))
            {
                GLog.ErrorFormat("GetFloat key {0} not set yet", key);
            }
            return o;
        }
        #endregion
    }
}
