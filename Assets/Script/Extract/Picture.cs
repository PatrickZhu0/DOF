using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extract1
{
    /// <summary>
    ///      img中的一个序列帧文件
    /// </summary>
    public class Picture
    {
        private Texture2D _image;
        //private Sprite _image;

        /// <summary>
        ///     帧域宽高
        /// </summary>
        public IntVec2 CanvasSize = new IntVec2(1, 1);

        /// <summary>
        ///     压缩类型
        /// </summary>
        public CompressMode CompressMode = CompressMode.NONE;

        /// <summary>
        ///     贴图在V2,V4时的数据
        /// </summary>
        public byte[] Data = new byte[2];

        /// <summary>
        ///     贴图在img中的下标
        /// </summary>
        public int Index;

        /// <summary>
        ///     数据长度
        /// </summary>
        public int Length = 2;

        /// <summary>
        ///     贴图坐标
        /// </summary>
        public IntVec2 Location = new IntVec2(0,0);

        /// <summary>
        ///     存储该贴图的img
        /// </summary>
        public Album Parent;

        /// <summary>
        ///     贴图宽高
        /// </summary>
        public IntVec2 Size = new IntVec2(1, 1);

        /// <summary>
        ///     当贴图为链接贴图时所指向的贴图
        /// </summary>
        public Picture Target;

        public Picture() { }

        public Picture(Album parent)
        {
            Parent = parent;
        }

        /// <summary>
        ///     色位
        /// </summary>
        public ColorBits Type { set; get; } = ColorBits.ARGB_1555;

        /// <summary>
        ///     贴图内容
        /// </summary>
        public Texture2D Texture {
            get {
                if (Type == ColorBits.LINK)
                {
                    return Target?.Texture;
                }
                if (IsOpen)
                {
                    return _image;
                }
                return _image = Parent.ConvertToTexture2D(this); //使用父容器解析
            }
            set {
                _image = value;
                if (value != null)
                {
                    Size = new IntVec2(value.width,value.height);
                }
            }
        }

        public bool IsOpen => _image != null;

        public int X {
            set => Location.X = value;
            get => Location.X;
        }

        public int Y {
            set => Location.Y = value;
            get => Location.Y;
        }

        public int Width {
            set => Size.X = value;
            get => Size.X;
        }

        public int Height {
            set => Size.Y = value;
            get => Size.Y;
        }

        public int CanvasWidth {
            set => CanvasSize = new IntVec2(value, CanvasHeight);
            get => CanvasSize.X;
        }

        public int CanvasHeight {
            set => CanvasSize = new IntVec2(CanvasWidth, value);
            get => CanvasSize.Y;
        }

        /// <summary>
        ///     文件版本
        /// </summary>
        public ImgVersion Version => Parent.Version;

        public bool Hidden => Width * Height == 1 && CompressMode == CompressMode.NONE;


        public void Load()
        {
            _image = Parent.ConvertToTexture2D(this); //使用父容器
        }
    }
}