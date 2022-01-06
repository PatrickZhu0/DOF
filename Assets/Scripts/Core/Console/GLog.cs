using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace GameDLL
{
    public class GLog
    {
        public enum Type
        {
            Error = LogType.Error,
            Assert = LogType.Assert,
            Warning = LogType.Warning,
            Log = LogType.Log,
            Exception = LogType.Exception,

            Message,
            Others = -1,
        }




        public const uint LogErrorFlag = 1 << 0;
        public const uint LogAssertFlag = 1 << 1;
        public const uint LogWarningFlag = 1 << 2;
        public const uint LogInfoFlag = 1 << 3;
        public const uint LogExceptionFlag = 1 << 4;
        public const uint LogAllFlag = uint.MaxValue;

        private static uint logFlag = LogAllFlag;

        //private static ILogger iLog = Debug.unityLogger;

        private static IConsole defaultConsole = new UnityConsole();

        private static bool assertionsOpened = false;

        private static bool checkAssertions = true;

        public static bool CheckAssertions
        {
            get
            {
                return checkAssertions;
            }
            set
            {
                checkAssertions = value;
            }
        }

        public static IConsole customConsole;

        static IConsole console
        {
            get
            {
                if (customConsole != null)
                    return customConsole;
                return defaultConsole;
            }
        }

        static GLog()
        {
            AssertionsCheck();
        }

        [System.Diagnostics.Conditional("UNITY_ASSERTIONS")]
        private static void AssertionsCheck()
        {
            assertionsOpened = true;
        }

        /// <summary>
        /// 设置Unity打印级别
        /// <para>级别Error Error(True) Assert(False) Warning(False) Log(False) Exception(True)</para>
        /// <para>级别Assert Error(True) Assert(True) Warning(False) Log(False) Exception(True)</para>
        /// <para>级别Warning Error(True) Assert(True) Warning(True) Log(False) Exception(True)</para>
        /// <para>级别Log Error(True) Assert(True) Warning(True) Log(True) Exception(True)</para>
        /// <para>级别Exception Error(False) Assert(False) Warning(False) Log(False) Exception(True)</para>
        /// </summary>
        public static void SetUnityLogLevel(LogType type)
        {
            Debug.unityLogger.filterLogType = type;
        }

        /// <summary>
        /// 设置Unity打印开关
        /// </summary>
        public static bool LogEnable
        {
            get
            {
                return console.LogEnable;
                //return iLog.logEnabled;
            }
            set
            {
                console.LogEnable = value;
                //iLog.logEnabled = value;
            }
        }

        /// <summary>
        /// 添加本地打印信息类型
        /// </summary>
        public static void AddLocalLogFlag(uint flag)
        {
            logFlag |= flag;
        }

        /// <summary>
        /// 移除本地打印信息类型
        /// </summary>
        public static void RemoveLocalLogFlag(uint flag)
        {
            logFlag &= ~flag;
        }

        /// <summary>
        /// 本地打印信息类型是否打开
        /// </summary>
        public static bool IsLocalLogOpen(uint flag)
        {
            return (logFlag & flag) != 0;
        }

        /// <summary>
        /// 设置本地打印信息类型
        /// </summary>
        public static void SetLocalLogFlag(uint flag)
        {
            logFlag = flag;
        }

        /// <summary>
        /// 获取本地打印信息类型
        /// </summary>
        public static uint GetLocalLogFlag()
        {
            return logFlag;
        }

        /// <summary>
        /// 打印错误信息
        /// </summary>
        public static void Error(object message)
        {
            if (IsLocalLogOpen(LogErrorFlag))
                console.Log(Type.Error, message);
            //Debug.LogError(message);
        }

        /// <summary>
        /// 打印错误信息及消息对象
        /// </summary>
        public static void Error(object message, Object context)
        {
            if (IsLocalLogOpen(LogErrorFlag))
                console.Log(Type.Error, message, context);
            //Debug.LogError(message, context);
        }

        /// <summary>
        /// 打印格式化错误信息
        /// </summary>
        public static void ErrorFormat(string message, params object[] args)
        {
            if (IsLocalLogOpen(LogErrorFlag))
                console.LogFormat(Type.Error, message, args);
            //Debug.LogErrorFormat(message, args);
        }

        /// <summary>
        /// 打印格式化错误信息及消息对象
        /// </summary>
        public static void ErrorFormat(Object context, string message, params object[] args)
        {
            if (IsLocalLogOpen(LogErrorFlag))
                console.LogFormat(Type.Error, context, message, args);
            //Debug.LogErrorFormat(context, message, args);
        }

        /// <summary>
        /// 打印带tag错误信息
        /// </summary>
        public static void TagError(object message, string tag)
        {
            if (IsLocalLogOpen(LogErrorFlag))
                console.LogTag(Type.Error, message, tag);
            //iLog.LogError(tag, message);
        }

        /// <summary>
        /// 打印断言信息
        /// </summary>
        public static void Assertion(object message)
        {
            if (IsLocalLogOpen(LogAssertFlag))
                console.Log(Type.Assert, message);
            //Debug.LogAssertion(message);
        }

        /// <summary>
        /// 打印断言信息及消息对象
        /// </summary>
        public static void Assertion(object message, Object context)
        {
            if (IsLocalLogOpen(LogAssertFlag))
                console.Log(Type.Assert, message, context);
            //Debug.LogAssertion(message, context);
        }

        /// <summary>
        /// 打印格式化断言信息
        /// </summary>
        public static void AssertionFormat(string message, params object[] args)
        {
            if (IsLocalLogOpen(LogAssertFlag))
                console.LogFormat(Type.Assert, message, args);
            //Debug.LogAssertionFormat(message, args);
        }

        /// <summary>
        /// 打印格式化断言信息及消息对象
        /// </summary>
        public static void AssertionFormat(Object context, string message, params object[] args)
        {
            if (IsLocalLogOpen(LogAssertFlag))
                console.LogFormat(Type.Assert, context, message, args);
            //Debug.LogAssertionFormat(context, message, args);
        }

        /// <summary>
        /// 打印警告信息
        /// </summary>
        public static void Warning(object message)
        {
            if (IsLocalLogOpen(LogWarningFlag))
                console.Log(Type.Warning, message);
            //Debug.LogWarning(message);
        }

        /// <summary>
        /// 打印警告信息及消息对象
        /// </summary>
        public static void Warning(object message, Object context)
        {
            if (IsLocalLogOpen(LogWarningFlag))
                console.Log(Type.Warning, message, context);
            //Debug.LogWarning(message, context);
        }

        /// <summary>
        /// 打印格式化警告信息
        /// </summary>
        public static void WarningFormat(string message, params object[] args)
        {
            if (IsLocalLogOpen(LogWarningFlag))
                console.LogFormat(Type.Warning, message, args);
            //Debug.LogWarningFormat(message, args);
        }

        /// <summary>
        /// 打印格式化警告信息及消息对象
        /// </summary>
        public static void WarningFormat(Object context, string message, params object[] args)
        {
            if (IsLocalLogOpen(LogWarningFlag))
                console.LogFormat(Type.Warning, context, message, args);
            //Debug.LogWarningFormat(context, message, args);
        }

        /// <summary>
        /// 打印带tag警告信息
        /// </summary>
        public static void TagWarning(object message, string tag)
        {
            if (IsLocalLogOpen(LogWarningFlag))
                console.LogTag(Type.Warning, message, tag);
            //iLog.LogWarning(tag, message);
        }

        /// <summary>
        /// 打印日志信息
        /// </summary>
        public static void Log(object message)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.Log(Type.Log, message);
            //Debug.Log(message);
        }

        /// <summary>
        /// 打印日志信息及消息对象
        /// </summary>
        public static void Log(object message, Object context)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.Log(Type.Log, message, context);
            //Debug.Log(message, context);
        }

        /// <summary>
        /// 打印格式化日志信息
        /// </summary>
        public static void LogFormat(string message, params object[] args)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.LogFormat(Type.Log, message, args);
            //Debug.LogFormat(message, args);
        }

        /// <summary>
        /// 打印格式化日志信息及消息对象
        /// </summary>
        public static void LogFormat(Object context, string message, params object[] args)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.LogFormat(Type.Log, context, message, args);
            //Debug.LogFormat(context, message, args);
        }

        /// <summary>
        /// 打印带颜色格式化日志信息
        /// </summary>
        public static void LogColor(Color color, string message, params object[] args)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.LogColor(color, message, args);
            //{
            //    if (message == null)
            //        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> null</color>");
            //    else if (args == null || args.Length == 0)
            //        Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> " + message + "</color>");
            //    else
            //        Debug.LogFormat("<color=#" + ColorUtility.ToHtmlStringRGB(color) + "> " + message + "</color>", args);
            //}
        }

        /// <summary>
        /// 打印带tag日志信息
        /// </summary>
        public static void TagInfo(object message, string tag)
        {
            if (IsLocalLogOpen(LogInfoFlag))
                console.LogTag(Type.Log, message, tag);
            //iLog.Log(tag, message);
        }

        /// <summary>
        /// 打印异常信息
        /// </summary>
        public static void Exception(Exception exc)
        {
            if (IsLocalLogOpen(LogExceptionFlag))
                console.LogException(exc);
            //Debug.LogException(exc);
        }

        /// <summary>
        /// 打印异常信息及消息对象
        /// </summary>
        public static void Exception(Exception exc, Object context)
        {
            if (IsLocalLogOpen(LogExceptionFlag))
                console.LogException(exc, context);
            //Debug.LogException(exc, context);
        }

        const string WatchStr = "<watch>{0}</watch>\n{1}";
        public static void Watch(string name, object message)
        {
            console.LogFormat(Type.Log, WatchStr, name, message);
            //Debug.LogFormat(WatchStr, name, message);
        }

        public static void Watch(Object context, string name, object message)
        {
            console.LogFormat(Type.Log, context, WatchStr, name, message);
            //Debug.LogFormat(context, WatchStr, name, message);
        }

        public static void Print(string message, Type type = Type.Log)
        {
            console.Log(type, message);
        }

        /// <summary>
        /// 断言设置开关，raise为true将中断后续代码执行
        /// </summary>
        public static bool RaiseAssert
        {
            get
            {
                return Assert.raiseExceptions;
            }
            set
            {
                Assert.raiseExceptions = value;
            }
        }

        /// <summary>
        /// 检测是否打开了UNITY_ASSERTIONS宏并打印提示
        /// </summary>
        static void CheckAssert()
        {
            if (checkAssertions && !assertionsOpened)
                Warning("Plase set the symbol 'UNITY_ASSERTIONS' for script compilation");
        }

        /// <summary>
        /// 如果obj不为空触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsNullObject(Object obj, string message)
        {
            CheckAssert();
            Assert.IsNull(obj, message);
        }

        /// <summary>
        /// 如果value不为空触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsNull<T>(T value, string message) where T : class
        {
            CheckAssert();
            Assert.IsNull(value, message);
        }

        /// <summary>
        /// 如果obj为空触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsNotNullObject(Object obj, string message)
        {
            CheckAssert();
            Assert.IsNotNull(obj, message);
        }

        /// <summary>
        /// 如果value为空触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsNotNull<T>(T value, string message) where T : class
        {
            CheckAssert();
            Assert.IsNotNull(value, message);
        }

        /// <summary>
        /// 如果condition为false触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsTrue(bool condition, string message)
        {
            CheckAssert();
            Assert.IsTrue(condition, message);
        }

        /// <summary>
        /// 如果condition为true触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void IsFalse(bool condition, string message)
        {
            CheckAssert();
            Assert.IsFalse(condition, message);
        }

        /// <summary>
        /// 如果expected与actual对象不相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreEqualObject(Object expected, Object actual, string message)
        {
            CheckAssert();
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        /// 如果expected与actual不相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            CheckAssert();
            Assert.AreEqual(expected, actual, message);
        }

        /// <summary>
        /// 自定义比较器，如果expected与actual不相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreEqualComparer<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            CheckAssert();
            Assert.AreEqual(expected, actual, message, comparer);
        }

        /// <summary>
        /// 如果expected与actual对象相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreNotEqualObject(Object expected, Object actual, string message)
        {
            CheckAssert();
            Assert.AreNotEqual(expected, actual, message);
        }

        /// <summary>
        /// 如果expected与actual相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreNotEqual<T>(T expected, T actual, string message)
        {
            CheckAssert();
            Assert.AreNotEqual(expected, actual, message);
        }

        /// <summary>
        /// 自定义比较器，如果expected与actual相等触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreNotEqualComparer<T>(T expected, T actual, string message, IEqualityComparer<T> comparer)
        {
            CheckAssert();
            Assert.AreNotEqual(expected, actual, message, comparer);
        }

        /// <summary>
        /// 如果expected与actual误差大等于0.00001f触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreApproximatelyEqual(float expected, float actual, string message)
        {
            CheckAssert();
            Assert.AreApproximatelyEqual(expected, actual, message);
        }

        /// <summary>
        /// 如果expected与actual误差大等于tolerance触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            CheckAssert();
            Assert.AreApproximatelyEqual(expected, actual, tolerance, message);
        }

        /// <summary>
        /// 如果expected与actual误差小等于0.00001f触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreNotApproximatelyEqual(float expected, float actual, string message)
        {
            CheckAssert();
            Assert.AreNotApproximatelyEqual(expected, actual, message);
        }

        /// <summary>
        /// 如果expected与actual误差小等于tolerance触发断言
        /// 编译时需要加入宏UNITY_ASSERTIONS
        /// </summary>
        public static void AreNotApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            CheckAssert();
            Assert.AreNotApproximatelyEqual(expected, actual, tolerance, message);
        }

        /// <summary>
        /// 导出Int值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int IntValue(LogType type)
        {
            return (int)type;
        }
    }
}
