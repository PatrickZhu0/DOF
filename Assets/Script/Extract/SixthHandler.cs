using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extract1;
using System.IO;
using System.Linq;

public class SixthHandler : SecondHandler
{
    public SixthHandler(Album album) : base(album) { }

    public override Texture2D ConvertToTexture2D(Picture entity)
    {
        var data = entity.Data;
        var size = entity.Width * entity.Height;
        if (entity.Type == ColorBits.ARGB_1555 && entity.CompressMode == CompressMode.ZLIB)
        {
            data = Zlib.Decompress(data, size);
            var table = Album.CurrentTable;
            if (table.Count > 0)
            {
                using (var os = new MemoryStream())
                {
                    foreach (var i in data)
                    {
                        os.WriteColor(table[i % table.Count], ColorBits.ARGB_8888);
                    }
                    data = os.ToArray();
                }
                return FromArray(data, entity.Size);
            }
        }
        return base.ConvertToTexture2D(entity);
    }

    public override void CreateFromStream(Stream stream)
    {
        var size = stream.ReadInt();
        Album.Tables = new List<List<Color32>>();
        for (var i = 0; i < size; i++)
        {
            var count = stream.ReadInt();
            var table = Colors.ReadPalette(stream, count);
            Album.Tables.Add(table.ToList());
        }
        base.CreateFromStream(stream);
    }

}
