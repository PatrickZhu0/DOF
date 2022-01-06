using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
public class FrameByFramePlay : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    FileInfo fInfo0;

    public bool isFlip;
    public string address;
    public int imageCount;
    public float frameRate;
    //ani文件中的[IMAGE POS]
    public Vector2 offset;
    public int fristFrame = 0;

    public string[] sArray;
    public int[] anchorArray;
    public Vector2[] anchorVectorArray;
    public Texture2D[] textureArray;
    public Sprite[] spriteArray;
    public Sprite[] spriteArray_Flip;
    public int currentFrameIndex;
    public int startFrame;
    public int endFrame;


    private float dt;
    private string s = "";

    void OnEnable()
    {
        textureArray = new Texture2D[imageCount];
        spriteArray = new Sprite[imageCount];
        spriteArray_Flip = new Sprite[imageCount];
        anchorVectorArray = new Vector2[imageCount];

        for (int i = 0, j = fristFrame; i < imageCount; i++, j++)
        {
            textureArray[i] = (Texture2D)Resources.Load(address + "/" + j);
            spriteArray[i] = Resources.Load<Sprite>(address + "/" + j);
        }

        spriteRenderer = transform.GetComponent<SpriteRenderer>();

        TextAsset textasset = Resources.Load(address + "/x") as TextAsset;
        s = textasset.text;

        sArray = s.Split(new char[2] { ' ', '\n' });

        anchorArray = new int[sArray.Length];

        for (int i = 0; i < sArray.Length; i++)
        {
            anchorArray[i] = Convert.ToInt32(sArray[i]);
        }

        for (int i = anchorArray.Length / 2 - 1; i > -1; i--)
        {
            anchorArray[i * 2] = anchorArray[i * 2] - Convert.ToInt32(offset.x);
            anchorArray[i * 2 + 1] = anchorArray[i * 2 + 1] - Convert.ToInt32(offset.y);
        }


        for (int i = 0; i < anchorVectorArray.Length; i++)
        {
            anchorVectorArray[i].x = anchorArray[i * 2];
            anchorVectorArray[i].y = anchorArray[i * 2 + 1];
        }

        for (int i = 0; i < spriteArray.Length; i++)
        {
            Debug.LogErrorFormat("{0}, {1} {2}, {3}", spriteArray[i].rect.x, spriteArray[i].rect.y, spriteArray[i].rect.width, spriteArray[i].rect.height);
            spriteArray[i] = Sprite.Create(textureArray[i], spriteArray[i].rect, new Vector2(0.5f - (anchorVectorArray[i].x + spriteArray[i].rect.width / 2) / spriteArray[i].rect.width, 0.5f + (anchorVectorArray[i].y + spriteArray[i].rect.height / 2) / spriteArray[i].rect.height), 1.0f);
        }

        currentFrameIndex = 0;
    }

    void Update()
    {
        dt += Time.deltaTime;
        if (dt >= frameRate && currentFrameIndex >= endFrame - startFrame)
        {
            currentFrameIndex = 0;
            dt = 0;
        }

        if (dt >= frameRate)
        {
            spriteRenderer.sprite = spriteArray[currentFrameIndex + startFrame - 1];
            if (endFrame > startFrame)
            {
                currentFrameIndex++;
            }
            dt = 0;
        }

    }

    public void ResetSprite()
    {
        dt = 0;
        frameRate = 0;
        startFrame = 0;
        endFrame = 0;
        currentFrameIndex = 0;
    }
}
