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

    // Start is called before the first frame update
    void Start()
    {
        fAnimNode = new FAnimSystem.FAnimNode();
        BinaryAniCompiler.LoadAni("", ref fAnimNode);
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
