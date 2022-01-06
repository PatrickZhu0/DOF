using System;

namespace GameDLL
{
    [Flags]
    public enum CommandMode
    {
        GUIMode = 1 << 0,

        UGUIMode = 1 << 1,

        WindowsTerminal = 1 << 2,

        All = -1,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ConsoleCommandAttribute : Attribute
    {
        /// <summary>
        /// 命令名称
        /// </summary>
        public string commandName { get; set; }
        /// <summary>
        /// 命令帮助信息
        /// </summary>
        public string commandHelp { get; set; }
        /// <summary>
        /// 命令会跟随其他命令执行
        /// </summary>
        public string afterCommand { get; set; }
        /// <summary>
        /// 命令别名
        /// </summary>
        public string[] aliasNames { get; set; }
        /// <summary>
        /// 命令所依赖控制台模式
        /// </summary>
        public CommandMode commandMode { get; set; }

        public ConsoleCommandAttribute(string commandName, CommandMode commandMode = CommandMode.All, string commandHelp = null, string afterCommand = null, params string[] aliasNames)
        {
            this.commandName = commandName;
            this.commandMode = commandMode;
            this.commandHelp = commandHelp;
            this.afterCommand = afterCommand;
            this.aliasNames = aliasNames;
        }

        public ConsoleCommandAttribute(string commandName, string commandHelp, params string[] aliasNames)
        {
            this.commandName = commandName;
            commandMode = CommandMode.All;
            this.commandHelp = commandHelp;
            this.aliasNames = aliasNames;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class CommandFullValueAttribute : Attribute
    {
    }
}
