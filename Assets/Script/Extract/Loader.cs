using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Extract1;
public static class Loader
{
    public static void LoadSprites(string fileName)
    {
        List<Album> albums = NpkCoder.Load(Application.streamingAssetsPath + string.Format("\\NPK\\{0}.npk",fileName));

        Album saber = albums[0];


        Picture entity = saber.List[0];
        Sprite sp = Sprite.Create(entity.Texture, new Rect(0, 0, entity.Width, entity.Height), Vector2.zero);
    }
}