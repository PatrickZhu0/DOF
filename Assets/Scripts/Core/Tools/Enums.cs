using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDLL
{
    public enum OffsetType
    {
        NONE = 0,       //
        POINT = 1,      //相对目标的本地坐标的相对偏移，最后结果受原始物体的缩放影响
        DIRECTION = 2,  //相对目标的本地坐标的绝对偏移，最后结果不受原始物体的缩放影响
        VECTOR = 3,     //相对目标的本地坐标的绝对偏移，最后结果不受位置影响但受缩放影响
        WORLD = 4,      //不受缩放与旋转的影响，世界坐标偏移
        LOOKAT = 5,     //自动朝向目标
    }



}
