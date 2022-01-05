using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Extract1 {
    public class FirstHandler : SecondHandler
    {
        public FirstHandler(Album album) : base(album) { }

        public override void CreateFromStream(Stream stream)
        {
            Album.IndexLength = stream.ReadInt();
            stream.Seek(2);
            Album.Version = (ImgVersion)stream.ReadInt();
            Album.Count = stream.ReadInt();
            var dic = new Dictionary<Picture, int>();
            for (var i = 0; i < Album.Count; i++)
            {
                var image = new Picture(Album)
                {
                    Index = Album.List.Count,
                    Type = (ColorBits)stream.ReadInt()
                };
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
                if (image.CompressMode == CompressMode.NONE)
                {
                    image.Length = image.Size.Width * image.Size.Height * (image.Type == ColorBits.ARGB_8888 ? 4 : 2);
                }
                var data = new byte[image.Length];
                stream.Read(data);
                image.Data = data;
            }
            foreach (var image in Album.List)
            {
                if (image.Type == ColorBits.LINK)
                {
                    if (dic.ContainsKey(image) && dic[image] > -1 && dic[image] < Album.List.Count &&
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
                }
            }
        }
    }
}