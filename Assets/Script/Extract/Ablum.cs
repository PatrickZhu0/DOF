using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Extract1
{
    /// <summary>
    ///     img文件
    /// </summary>
    public sealed class Album
    {

        private int _tabindex;

        /// <summary>
        ///     贴图个数
        /// </summary>
        public int Count;

        /// <summary>
        ///     文件数据
        /// </summary>
        public byte[] Data;

        public Album()
        {
            Handler = Handler.CreateHandler(Version, this);
        }

        /// <summary>
        ///     处理器
        /// </summary>
        public Handler Handler { get; set; }

        /// <summary>
        ///     文件数据长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        ///     文件所在偏移量
        /// </summary>
        public int Offset { set; get; }

        /// <summary>
        ///     贴图列表
        /// </summary>
        public List<Picture> List { get; } = new List<Picture>();

        /// <summary>
        ///     索引信息长度
        /// </summary>
        public long IndexLength { set; get; }

        /// <summary>
        ///     文件路径
        /// </summary>
        public string Path { set; get; } = string.Empty;

        public string Name
        {
            get => "";
            set => Path = Path.Replace(Name, value);
        }

        /// <summary>
        ///     文件版本
        /// </summary>
        public ImgVersion Version { get; set; } = ImgVersion.Ver2;

        /// <summary>
        ///     色表索引
        /// </summary>
        public int TableIndex
        {
            set
            {
                if (_tabindex != value)
                {
                    Refresh();
                    _tabindex = Math.Min(value, Tables.Count - 1);
                }
            }
            get => _tabindex;
        }

        /// <summary>
        ///     重置图片
        /// </summary>
        public void Refresh()
        {
            List.ForEach(e => e.Texture = null);
        }

        public List<Color32> CurrentTable
        {
            get
            {
                if (TableIndex > -1 && TableIndex < Tables.Count)
                {
                    return Tables[TableIndex];
                }
                return new List<Color32>();
            }
        }

        /// <summary>
        ///     色表
        /// </summary>
        public List<List<Color32>> Tables { set; get; } = new List<List<Color32>> { new List<Color32>() };


        /// <summary>
        /// 重定向目标
        /// </summary>
        public Album Target { set; get; }

        public Picture this[int index]
        {
            get => List[index];
            set
            {
                if (index < List.Count)
                {
                    List[index] = value;
                }
                else
                {
                    List.Add(value);
                }
            }
        }


        public override string ToString()
        {
            return Name;
        }


        /// <summary>
        ///     初始化处理器
        /// </summary>
        /// <param name="stream"></param>
        public void InitHandle(Stream stream)
        {
            Handler = Handler.CreateHandler(Version, this);
            if (Handler != null && stream != null) Handler.CreateFromStream(stream);
        }


        /// <summary>
        ///     根据路径判断唯一
        /// </summary>
        /// <param name="al"></param>
        /// <returns></returns>
        public bool Equals(Album al)
        {
            return Path.Equals(al?.Path);
        }

        /// <summary>
        ///     从流初始化
        /// </summary>
        /// <param name="stream"></param>
        public void CreateFromStream(Stream stream)
        {
            Handler.CreateFromStream(stream);
        }

        /// <summary>
        ///     解析贴图
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Texture2D ConvertToTexture2D(Picture entity)
        {
            return Handler.ConvertToTexture2D(entity);
        }
    }
}