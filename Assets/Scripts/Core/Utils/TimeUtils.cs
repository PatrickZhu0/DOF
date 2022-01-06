using System;

namespace GameDLL
{
    /// <summary>
    /// 时间工具
    /// TODO:需要考虑玩家对系统的时间操作，需要改为服务器的时间作为修正
    /// </summary>
    public class TimeUtils
    {
        private static DateTime _startDateTime;
        private static bool _initStart = false;
        /// <summary>
        /// 获取起始时间戳
        /// </summary>
        public static DateTime StartDateTime
        {
            get
            {
                if (!_initStart)
                {
                    _startDateTime = DateTime.Parse("1970-01-01");
                    _initStart = true;
                }
                return _startDateTime;
            }
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        public static DateTime GetNowDateTime()
        {
            return DateTime.Now;
        }


        /// <summary>
        /// 获取当前时间戳，单位秒
        /// </summary>
        public static long GetNowTimeStamp()
        {
            return (GetNowDateTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 获取时间戳，单位秒
        /// </summary>
        public static long GetTimeStamp(DateTime date)
        {
            return (date.Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 获取当前时间戳，单位毫秒
        /// </summary>
        public static long GetNowTimeStampMS()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 获取时间戳，单位毫秒
        /// </summary>
        public static long GetTimeStampMS(DateTime date)
        {
            return (date.Ticks - 621355968000000000) / 10000;
        }
    }
}