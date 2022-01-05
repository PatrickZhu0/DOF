using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

namespace Extract1 {
    /// <summary>
    ///     Ver4处理器
    /// </summary>
    public class FourthHandler : SecondHandler {
        public FourthHandler(Album album) : base(album) { }

        public override Texture2D ConvertToTexture2D(Picture entity) {
            var data = entity.Data;
            var size = entity.Width * entity.Height;
            if (entity.Type == ColorBits.ARGB_1555 && entity.CompressMode == CompressMode.ZLIB) {
                data = Zlib.Decompress(data, size);
                var table = Album.CurrentTable;
                if (table.Count > 0) {
                    using (var os = new MemoryStream()) {
                        foreach (var b in data) {
                            var j = b % table.Count;
                            os.WriteColor(table[j], ColorBits.ARGB_8888);
                        }
                        data = os.ToArray();
                    }
                    return base.FromArray(data, entity.Size);
                }
            }
            return base.FromArray(data, entity.Size);
        }


        public override void CreateFromStream(Stream stream)
        {
            var size = stream.ReadInt();
            var Table = new List<Color32>(Colors.ReadPalette(stream, size));
            Album.Tables = new List<List<Color32>>();
            Album.Tables.Add(Table);
            base.CreateFromStream(stream);
        }
    }
}