using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using pvfLoaderXinyu;
using UnityEngine;

public class PvfTest : MonoBehaviour
{
    //static string pvfFilename = "C:/Users/unknown/Desktop/Documents/DNFTools/Script.pvf";
    // Start is called before the first frame update

    public SpriteRenderer spriteRenderer;

    public string imgPath = "npkroot/character/swordman/saber";

    void Start()
    {
        //var pvf = new Pvf(pvfFilename);//初始化pvf文件，进行读取操作

        ////string fileContent = pvf.getPvfFileByPath("character/swordman/swordman.chr", Encoding.UTF8); 

        //string stayContent = pvf.unpackAniFileByPath("character/fighter/animation/attack2.ani");

        //pvf.dispose();//不用了就释放掉
        //Debug.LogError(stayContent);

        BinaryAniCompiler.LoadAni("");

        Extract1.Album album = ResourcesManager.Instance.loadImg(imgPath);
        Debug.LogError(album.Count);
        for (int i = 0; i < album.Count; i++)
        {
            Extract1.Picture picture = album.List[0];

            Rect rect = new Rect(0, 0, picture.Width, picture.Height);
            spriteRenderer.sprite = Sprite.Create(picture.Texture, rect, new Vector2(0.5f - (picture.X + rect.width / 2) / rect.width, 0.5f + (picture.Y + rect.height / 2) / rect.height), 1.0f);
        }

        //spriteRenderer. = album.List[0].Texture;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
