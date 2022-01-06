using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Extract1
{
    public static class NpkCoder
    {
        public const string NPK_FlAG = "NeoplePack_Bill";
        public const string IMG_FLAG = "Neople Img File";
        public const string IMAGE_FLAG = "Neople Image File";

        public const string IMAGE_DIR = "ImagePacks2";

        public const string SOUND_DIR = "SoundPacks";

        private const string KEY_HEADER = "puchikon@neople dungeon and fighter ";

        public static Encoding Encoding = Encoding.UTF8;

        private static byte[] key;

        private static byte[] Key
        {
            get
            {
                if (key != null)
                {
                    return key;
                }
                var cs = new byte[256];
                int length = Encoding.GetBytes(KEY_HEADER, 0, KEY_HEADER.Length, cs, 0);
                var ds = Encoding.GetBytes("DNF");
                for (var i = length; i < 255; i++)
                {
                    cs[i] = ds[i % 3];
                }
                cs[255] = 0;
                return key = cs;
            }
        }

        /// <summary>
        ///     读取img路径
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static string ReadPath(this Stream stream)
        {
            var data = new byte[256];
            var i = 0;
            while (i < 256)
            {
                data[i] = (byte)(stream.ReadByte() ^ Key[i]);
                if (data[i] == 0)
                {
                    break;
                }
                i++;
            }
            stream.Seek(255 - i); //防止因加密导致的文件名读取错误
            return Encoding.GetString(data, 0, i);
        }

        public static List<Album> ReadInfo(Stream stream)
        {
            var flag = stream.ReadString();
            var List = new List<Album>();
            if (flag != NPK_FlAG)
            {
                return List;
            }
            var count = stream.ReadInt();
            for (var i = 0; i < count; i++)
            {
                List.Add(new Album
                {
                    Offset = stream.ReadInt(),
                    Length = stream.ReadInt(),
                    Path = stream.ReadPath()
                });
            }
            return List;
        }

        /// <summary>
        ///     读取IMG或NPK
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="file">当格式非NPK时，需要得到文件名</param>
        /// <returns></returns>
        public static List<Album> ReadNpk(Stream stream, string file)
        {
            var List = new List<Album>();
            var flag = stream.ReadString();
            if (flag == NPK_FlAG)
            {
                //当文件是NPK时
                stream.Seek(0, SeekOrigin.Begin);
                List.AddRange(ReadInfo(stream));
                if (List.Count > 0)
                {
                    stream.Seek(32);
                }
            }
            else
            {
                var album = new Album();
                if (file != null)
                {
                    album.Path = file.GetSuffix();
                }
                List.Add(album);
            }
            for (var i = 0; i < List.Count; i++)
            {
                var length = i < List.Count - 1 ? List[i + 1].Offset : stream.Length;
                ReadImg(stream, List[i], length);
            }
            return List;
        }

        public static List<Album> ReadNpk(Stream stream)
        {
            return ReadNpk(stream, null);
        }

        public static void ReadImg(Stream stream, Album album, long length)
        {
            stream.Seek(album.Offset, SeekOrigin.Begin);
            var albumFlag = stream.ReadString();
            if (albumFlag == IMG_FLAG)
            {
                album.IndexLength = stream.ReadLong();
                album.Version = (ImgVersion)stream.ReadInt();
                album.Count = stream.ReadInt();
                album.InitHandle(stream);
            }
            else
            {
                if (albumFlag == IMAGE_FLAG)
                {
                    album.Version = ImgVersion.Ver1;
                }
                else
                {
                    if (length < 0)
                    {
                        length = stream.Length;
                    }
                    album.Version = ImgVersion.Other;
                    stream.Seek(album.Offset, SeekOrigin.Begin);
                    if (album.Name.ToLower().EndsWith(".ogg"))
                    {
                        album.Version = ImgVersion.Other;
                        album.IndexLength = length - stream.Position;
                    }
                }
                album.InitHandle(stream);
            }
        }



        #region 读取一个IMG
        public static void ReadImg(Stream stream, Album album)
        {
            ReadImg(stream, album, -1);
        }

        public static Album ReadImg(Stream stream, string path)
        {
            return ReadImg(stream, path, -1);
        }


        public static Album ReadImg(Stream stream, string path, long length)
        {
            var album = new Album()
            {
                Path = path
            };
            ReadImg(stream, album, length);
            return album;
        }


        public static void ReadImg(byte[] data, Album album)
        {
            ReadImg(data, album, -1);
        }


        public static Album ReadImg(byte[] data, string path)
        {
            return ReadImg(data, path, -1);
        }

        public static void ReadImg(byte[] data, Album album, long length)
        {
            using (var ms = new MemoryStream(data))
            {
                ReadImg(ms, album, length);
            }
        }

        public static Album ReadImg(byte[] data, string path, long length)
        {
            using (var ms = new MemoryStream(data))
            {
                return ReadImg(ms, path, length);
            }
        }

        #endregion



        public static List<Album> Find(IEnumerable<Album> Items, params string[] args)
        {
            return Find(Items, false, args);
        }

        public static List<Album> Find(IEnumerable<Album> Items, bool allCheck, params string[] args)
        {
            var list = new List<Album>(Items.Where(item =>
            {
                if (!allCheck && args.Length == 0)
                {
                    return true;
                }
                if (allCheck && !args[0].Equals(item.Name))
                {
                    return false;
                }
                return args.All(arg => item.Path.Contains(arg));
            }));
            return list;
        }


        /// <summary>
        ///     根据文件路径得到NPK名
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public static string GetFilePath(this Album file)
        {
            var path = file.Path;
            var index = path.LastIndexOf("/");
            if (index > -1)
            {
                path = path.Substring(0, index);
            }
            path = path.Replace("/", "_");
            path += ".NPK";
            return path;
        }



        #region 加载
        public static List<Album> Load(string file)
        {
            return Load(false, file);
        }

        public static List<Album> Load(bool onlyPath, string file)
        {
            var list = new List<Album>();
            if (Directory.Exists(file))
            {
                return Load(onlyPath, Directory.GetFiles(file));
            }
            if (!File.Exists(file))
            {
                return list;
            }
            using (var stream = File.OpenRead(file))
            {
                if (onlyPath)
                {
                    return ReadInfo(stream);
                }
                var enums = ReadNpk(stream, file);
                return enums;
            }
        }

        public static List<Album> Load(bool onlyPath, params string[] files)
        {
            var List = new List<Album>();
            foreach (var file in files)
            {
                List.AddRange(Load(onlyPath, file));
            }
            return List;
        }

        public static List<Album> Load(params string[] files)
        {
            return Load(false, files);
        }


        public static Album LoadWithPath(string file, string name)
        {
            using (var stream = File.OpenRead(file))
            {
                return LoadWithPath(stream, name);
            }
        }

        public static Album LoadWithPath(Stream stream, string name)
        {
            var list = LoadWithPathArray(stream, name);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        public static List<Album> LoadWithPathArray(Stream stream, params string[] paths)
        {
            return LoadAll(stream, e => paths.Contains(e.Path));
        }

        public static List<Album> LoadAll(string file, Predicate<Album> predicate)
        {
            using (var fs = File.OpenRead(file))
            {
                return LoadAll(fs, predicate);
            }
        }

        public static List<Album> LoadAll(Stream stream, Predicate<Album> predicate)
        {
            var list = ReadInfo(stream);
            list = list.FindAll(predicate);
            foreach (var al in list)
            {
                stream.Seek(al.Offset, SeekOrigin.Begin);
                ReadImg(stream, al, stream.Length);
            }
            return list;
        }

        #endregion


    }
}