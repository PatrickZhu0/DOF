using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesManager : GameDLL.Singleton<ResourcesManager>
{

    public Extract1.Album loadImg(string path)
    {
        TextAsset ts = Resources.Load<TextAsset>(path);

        //Stream stream = new MemoryStream(ts.bytes);

        Extract1.Album album = new Extract1.Album();

        Extract1.NpkCoder.ReadImg(ts.bytes, album);


        //Extract1.Album album = Extract1.NpkCoder.ReadImg(stream);

        return album;
    }


}
