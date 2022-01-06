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
    void Start()
    {
        BinaryAniCompiler.LoadAni("");

        string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/stay.ani";
        SetAni(aniPath);
    }
    void SetAni(string aniPath)
    {
        fAnimNode = new FAnimSystem.FAnimNode();

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

        if (Input.GetKeyDown(KeyCode.A))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/attack1.ani";
            SetAni(aniPath);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/attack2.ani";
            SetAni(aniPath);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/attack2.ani";
            SetAni(aniPath);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/HopSmashReady.ani";
            SetAni(aniPath);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/HopSmash.ani";
            SetAni(aniPath);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            string aniPath = Application.dataPath + "/../PvfRoot/character/swordman/animation/Chargecrashexupper.ani";
            SetAni(aniPath);
        }

    }

    void OnGUI()
    {
        if (frame == null)
            return;

        foreach (Rect rect in frame.DamageBoxXY)
        {
            //EditorGUITools.DrawRect(rect, Color.cyan);
            DebugDarwRect(rect, Color.cyan);
        }

        foreach (Rect rect in frame.DamageBoxXZ)
        {
            //EditorGUITools.DrawRect(rect, Color.green);
            DebugDarwRect(rect, Color.green);
        }

        foreach (Rect rect in frame.AttackBoxXY)
        {
            //EditorGUITools.DrawRect(rect, Color.red);
            DebugDarwRect(rect, Color.magenta);
        }

        foreach (Rect rect in frame.AttackBoxXZ)
        {
            //EditorGUITools.DrawRect(rect, Color.magenta);
            DebugDarwRect(rect, Color.red);
        }
    }

    void DebugDarwRect(Rect rect, Color color)
    {
        //lian(AB)
        Debug.DrawLine(rect.min, new Vector2(rect.xMax, rect.yMin), color);
        //lian(AD)
        Debug.DrawLine(rect.min, new Vector2(rect.xMin, rect.yMax), color);
        //lian(CB)
        Debug.DrawLine(rect.max, new Vector2(rect.xMax, rect.yMin), color);
        //lian(CD)
        Debug.DrawLine(rect.max, new Vector2(rect.xMin, rect.yMax), color);
    }

}
