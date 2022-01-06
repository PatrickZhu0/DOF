using FAnimSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    [SerializeField]
    private SpriteArray spriteArray;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    FAnimSystem.FAnimNode fAnimNode;


    public string imgPath = "npkroot/character/swordman/saber";

    // Start is called before the first frame update
    void Start()
    {
        fAnimNode = new FAnimSystem.FAnimNode();

        string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/rest.ani";
        BinaryAniCompiler.LoadAni(aniPath, ref fAnimNode);


        Extract1.Album album = ResourcesManager.Instance.loadImg(imgPath);
        spriteArray.sprites = new Sprite[album.Count];


        for (int i = 0; i < fAnimNode.FrameMax; i++)
        {
            FFrame frame = fAnimNode.Frames[i];
            int spriteIndex = frame.ImgFrameIdx;

            Extract1.Picture picture = album.List[spriteIndex];
            Rect rect = Rect.zero;
            rect.width = picture.Width;
            rect.height = picture.Height;
            Vector2 ancher = new Vector2();
            Debug.LogError(spriteIndex + "  " + picture.X + "  " + picture.Y);
            Debug.LogError(frame.ImagePosX + "  " + frame.ImagePosY + "  " + picture.Width + "  " + picture.Height);
            ancher.x = 0.5f - (frame.ImagePosX + picture.X + rect.width / 2) / rect.width;
            ancher.y = 0.5f + (frame.ImagePosY + picture.Y + rect.height / 2) / rect.height;
            spriteArray.sprites[spriteIndex] = Sprite.Create(picture.Texture, rect, ancher, 1.0f);
        }

    }

    double lastTime = 0;
    FFrame frame;
    void Update()
    {
        double curTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        if (lastTime == 0 || (curTime - lastTime) >= frame.Delay)
        {
            lastTime = curTime;
            frame = fAnimNode.getCurFrame();
        }
        spriteRenderer.sprite = spriteArray.sprites[frame.ImgFrameIdx];
    }
}
