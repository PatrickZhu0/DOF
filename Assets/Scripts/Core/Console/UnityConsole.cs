using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameDLL
{
    public class UnityConsole : IConsole
    {
        private static ILogger iLog = Debug.unityLogger;

        public bool LogEnable
        {
            get
            {
                return iLog.logEnabled;
            }
            set
            {
                iLog.logEnabled = value;
            }
        }

        public void Log(GLog.Type type, object message)
        {
            switch (type)
            {
                case GLog.Type.Error:
                    Debug.LogError(message);
                    break;
                case GLog.Type.Assert:
                    Debug.LogAssertion(message);
                    break;
                case GLog.Type.Warning:
                    Debug.LogWarning(message);
                    break;
                case GLog.Type.Log:
                    Debug.Log(message);
                    break;
            }
        }

        public void Log(GLog.Type type, object message, Object context)
        {
            switch (type)
            {
                case GLog.Type.Error:
                    Debug.LogError(message, context);
                    break;
                case GLog.Type.Assert:
                    Debug.LogAssertion(message, context);
                    break;
                case GLog.Type.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case GLog.Type.Log:
                    Debug.Log(message, context);
                    break;
            }
        }

        public void LogFormat(GLog.Type type, string message, params object[] args)
        {
            switch (type)
            {
                case GLog.Type.Error:
                    Debug.LogErrorFormat(message, args);
                    break;
                case GLog.Type.Assert:
                    Debug.LogAssertionFormat(message, args);
                    break;
                case GLog.Type.Warning:
                    Debug.LogWarningFormat(message, args);
                    break;
                case GLog.Type.Log:
                    Debug.LogFormat(message, args);
                    break;
            }
        }

        public void LogFormat(GLog.Type type, Object context, string message, params object[] args)
        {
            switch (type)
            {
                case GLog.Type.Error:
                    Debug.LogErrorFormat(context, message, args);
                    break;
                case GLog.Type.Assert:
                    Debug.LogAssertionFormat(context, message, args);
                    break;
                case GLog.Type.Warning:
                    Debug.LogWarningFormat(context, message, args);
                    break;
                case GLog.Type.Log:
                    Debug.LogFormat(context, message, args);
                    break;
            }
        }

        public void LogTag(GLog.Type type, object message, string tag)
        {
            iLog.Log((LogType)type, tag, message);
        }

        public void LogColor(Color color, string message, params object[] args)
        {
            if (message == null)
                Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> null</color>");
            else if (args == null || args.Length == 0)
                Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> " + message + "</color>");
            else
                Debug.LogFormat("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> " + message + "</color>", args);
        }

        public void LogException(Exception exc)
        {
            Debug.LogException(exc);
        }

        public void LogException(Exception exc, Object context)
        {
            Debug.LogException(exc, context);
        }

    }
}
