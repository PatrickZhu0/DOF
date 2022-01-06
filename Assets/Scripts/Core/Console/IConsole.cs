using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDLL
{
    public interface IConsole
    {
        bool LogEnable { get; set; }

        void Log(GLog.Type type, object message);

        void Log(GLog.Type type, object message, Object context);

        void LogFormat(GLog.Type type, string message, params object[] args);

        void LogFormat(GLog.Type type, Object context, string message, params object[] args);

        void LogTag(GLog.Type type, object message, string tag);

        void LogColor(Color color, string message, params object[] args);

        void LogException(Exception exc);

        void LogException(Exception exc, Object context);
    }
}
