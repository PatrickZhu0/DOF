using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameDLL.Resource.Statistics
{
    /// <summary>
    /// 精灵类资源的计数统计
    /// </summary>
    public class SpriteUsageStatistics
    {
        /// <summary>
        /// 门面
        /// </summary>
        readonly StatisticsFacade _facade;

        /// <summary>
        /// 所有精灵的内部计数统计，其实可以不用
        /// </summary>
        Dictionary<int, int> usageDic = new Dictionary<int, int>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="facade"></param>
        public SpriteUsageStatistics(StatisticsFacade facade)
        {
            _facade = facade;
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="sprite"></param>
        void AddRef(Sprite sprite)
        {            
            int instId = sprite.GetInstanceID();
            int currentCount = 0;
            if(usageDic.TryGetValue(instId, out currentCount))
            {
                if(currentCount<0)
                {
                    GLog.ErrorFormat("SpriteUsageStatistics.AddRef, Sprite count error: count < 0 on AddRef: {0}, {1}", sprite.name, currentCount);
                }
            }
            int nextCount = currentCount + 1;
            usageDic[instId] = nextCount;
            _facade.LogVerboseFormat("SpriteUsageStatistics.AddRef, {0}, id={1}, count {2} -> {3}", sprite.name, instId, currentCount, nextCount);
            //呼叫真正的计数
            _facade.AssetAddRef(instId);
        }

        /// <summary>
        /// 释放引用
        /// </summary>
        /// <param name="sprite"></param>
        void ReleaseRef(Sprite sprite)
        {
            int instId = sprite.GetInstanceID();
            if(usageDic.ContainsKey(instId))
            {
                int oldCount = usageDic[instId];
                int currentCount = --usageDic[instId];
                if (currentCount < 0)
                {
                    GLog.ErrorFormat("Sprite count error: count < 0 on ReleaseRef: {0}, {1}", sprite.name, currentCount);
                }
                _facade.LogVerboseFormat("SpriteUsageStatistics.ReleaseRef, {0}, id={1}, count {2} -> {3}", sprite.name, instId, oldCount, currentCount);
            }
            else
            {
                GLog.ErrorFormat("SpriteUsageStatistics.ReleaseRef error: {0}, id={1} not exist in usageDic", sprite.name, instId);
            }
            //呼叫真正的计数
            _facade.AssetReleaseRef(instId, true);
        }

        /// <summary>
        /// 对应用层暴露的统一设置方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sprite"></param>
        public void SetSprite(Image image, Sprite sprite)
        {
            if (image.sprite)
            {
                ReleaseRef(image.sprite);
            }
            image.sprite = sprite;
            if (sprite)
            {
                AddRef(sprite);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一设置方法
        /// </summary>
        /// <param name="image"></param>
        /// <param name="spriteInfo"></param>
        public void SetCustomSprite<T, S>(T image, S spriteInfo, Func<T, S, Sprite> setSprite) where T : Image
        {
            if (image.sprite)
            {
                ReleaseRef(image.sprite);
            }
            Sprite sprite = setSprite(image, spriteInfo);
            if (sprite)
            {
                AddRef(sprite);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一初始化方法
        /// </summary>
        /// <param name="image"></param>
        public void OnImageCreate(Image image)
        {
            if (image.sprite)
            {
                AddRef(image.sprite);
            }
        }

        /// <summary>
        /// 对应用层暴露的统一释放方法
        /// </summary>
        /// <param name="image"></param>
        public void OnImageDestroy(Image image)
        {
            if(image.sprite)
            {
                ReleaseRef(image.sprite);
            }
        }
    }
}
