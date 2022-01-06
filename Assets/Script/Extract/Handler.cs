using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Extract1
{
    /// <summary>
    ///     IMG操作类
    /// </summary>
    public abstract class Handler
    {
        private static readonly Dictionary<ImgVersion, Type> Dic = new Dictionary<ImgVersion, Type>();

        static Handler()
        {
            Regisity();
        }

        public Album Album;

        public Handler(Album album)
        {
            Album = album;
        }

        public static List<ImgVersion> Versions => Dic.Keys.ToList();

        public ImgVersion Version { get; } = ImgVersion.Ver2;

        public static Handler CreateHandler(ImgVersion version, Album album)
        {
            var type = Dic[version];
            return type.CreateInstance(album) as Handler;
        }

        /// <summary>
        ///     从流初始化(默认读取)
        /// </summary>
        /// <param name="stream"></param>
        public abstract void CreateFromStream(Stream stream);

        /// <summary>
        ///     将字节集转换为图片
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract Texture2D ConvertToTexture2D(Picture entity);

        /// <summary>
        ///     注册版本处理器
        /// </summary>
        /// <param name="version"></param>
        /// <param name="type"></param>
        public static void Regisity(ImgVersion version, Type type)
        {
            if (Dic.ContainsKey(version))
            {
                Dic.Remove(version);
            }
            Dic.Add(version, type);
        }

        public static void Regisity()
        {
            Regisity(ImgVersion.Other, typeof(OtherHandler));
            //Regisity(ImgVersion.Ver1, typeof(FirstHandler));
            Regisity(ImgVersion.Ver2, typeof(SecondHandler));
            Regisity(ImgVersion.Ver4, typeof(FourthHandler));
            //Regisity(ImgVersion.Ver5, typeof(FifthHandler));
            Regisity(ImgVersion.Ver6, typeof(SixthHandler));
        }

        public Texture2D FromArray(byte[] data, IntVec2 size, ColorBits bits)
        {
            Texture2D texture = new Texture2D(size.Width, size.Height);
            var ms = new MemoryStream(data);
            int length = size.Width * size.Height;
            Color32[] color32s = new Color32[length];
            for (var i = size.Height - 1; i >= 0; i--)
            { 
                for (var j = 0; j < size.Width; j++)
                {
                    Colors.ReadColor(ms, bits, color32s, i * size.Width + j);
                }
            }
            ms.Close();
            texture.SetPixels32(color32s);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// 将byte数组以Format32bppArgb格式输出到texture
        /// </summary>
        /// <param name="data"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Texture2D FromArray(byte[] data, IntVec2 size)
        {
            Texture2D texture = new Texture2D(size.Width, size.Height);
            var ms = new MemoryStream(data);
            int length = size.Width * size.Height;
            Color32[] color32s = new Color32[length];
            for (var i = length - 1; i >= 0; i--)
            {
                byte[] bf;
                ms.Read(4, out bf);
                color32s[i] = new Color32(bf[1], bf[2], bf[3], bf[0]);
            }
            ms.Close();
            texture.filterMode = FilterMode.Trilinear;
            texture.SetPixels32(color32s);
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }
    }
}