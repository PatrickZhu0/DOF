using System;

namespace GameDLL
{
    /// <summary>
    /// 管理器接口
    /// </summary>
    public interface IManager : IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init(IGame game);

        /// <summary>
        /// 重置
        /// </summary>
        void Reset();

        ///// <summary>
        ///// 销毁
        ///// </summary>
        //void Dispose();
    }
}