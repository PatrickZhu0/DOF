using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

namespace Extract1
{
    public class SecondHandler : Handler
    {
        public SecondHandler(Album album) : base(album) { }

        public override Texture2D ConvertToTexture2D(Picture entity)
        {
            var data = entity.Data;
            var type = entity.Type;
            var size = entity.Width * entity.Height * (type == ColorBits.ARGB_8888 ? 4 : 2);
            if (entity.CompressMode == CompressMode.ZLIB)
            {
                data = Zlib.Decompress(data, size);
            }
            return FromArray(data, entity.Size, type);
        }

        public override void CreateFromStream(Stream stream)
        {
            var dic = new Dictionary<Picture, int>();
            var pos = stream.Position + Album.IndexLength;
            for (var i = 0; i < Album.Count; i++)
            {
                var image = new Picture(Album);
                image.Index = Album.List.Count;
                image.Type = (ColorBits)stream.ReadInt();
                Album.List.Add(image);
                if (image.Type == ColorBits.LINK)
                {
                    dic.Add(image, stream.ReadInt());
                    continue;
                }
                image.CompressMode = (CompressMode)stream.ReadInt();
                image.Width = stream.ReadInt();
                image.Height = stream.ReadInt();
                image.Length = stream.ReadInt();
                image.X = stream.ReadInt();
                image.Y = stream.ReadInt();
                image.CanvasWidth = stream.ReadInt();
                image.CanvasHeight = stream.ReadInt();
            }
            if (stream.Position < pos)
            {
                Album.List.Clear();
                return;
            }
            foreach (var image in Album.List.ToArray())
            {
                if (image.Type == ColorBits.LINK)
                {
                    if (dic.ContainsKey(image) && dic[image] < Album.List.Count && dic[image] > -1 &&
                        dic[image] != image.Index)
                    {
                        image.Target = Album.List[dic[image]];
                        image.Size = image.Target.Size;
                        image.CanvasSize = image.Target.CanvasSize;
                        image.Location = image.Target.Location;
                    }
                    else
                    {
                        Album.List.Clear();
                        return;
                    }
                    continue;
                }
                if (image.CompressMode == CompressMode.NONE)
                {
                    image.Length = image.Width * image.Height * (image.Type == ColorBits.ARGB_8888 ? 4 : 2);
                }

                var data = new byte[image.Length];
                stream.Read(data);
                image.Data = data;
            }
        }
    }
}