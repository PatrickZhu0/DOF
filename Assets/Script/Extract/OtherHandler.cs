using System.Drawing;
using System.IO;
using UnityEngine;

namespace Extract1 {
    /// <summary>
    ///     其他类型文件的处理
    /// </summary>
    public class OtherHandler : Handler {
        private byte[] _data = new byte[0];
        public OtherHandler(Album album) : base(album) { }

        public override Texture2D ConvertToTexture2D(Picture entity) {
            return null;
        }

        public override void CreateFromStream(Stream stream)
        {
            stream.Read((int)Album.IndexLength, out _data);
            Album.Data = _data;
        }
    }
}