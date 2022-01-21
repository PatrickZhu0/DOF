using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MotionType
{
    None = 0,
    Waiting = 1,
    Move,
    Sit,
    Damage1,
    Damage2,
    Down,
    Overturn,
    Jump,
    Jumpattack,
}

public class SpriteAnimator
{
    LinkedList<Sprite> m_SpriteList = new LinkedList<Sprite>();

    Dictionary<MotionType, SpriteMotion> m_MotionContainer = new Dictionary<MotionType, SpriteMotion>();

    Dictionary<string, int> motionNames = new Dictionary<string, int>();


    public bool CollectData(string path)
    {
        //path =
        m_MotionContainer[MotionType.Waiting] = ExtractAction(path, "Animation/Stay.ani");
        m_MotionContainer[MotionType.Move]    = ExtractAction(path, "Animation/Move.ani");
        m_MotionContainer[MotionType.Sit]     = ExtractAction(path, "Animation/Sit.ani");
        m_MotionContainer[MotionType.Damage1] = ExtractAction(path, "Animation/Damage1.ani");
        m_MotionContainer[MotionType.Damage2] = ExtractAction(path, "Animation/Damage2.ani");


        return true;
    }
    public SpriteMotion ExtractAction(string path, string name)
    {



        return new SpriteMotion();
    }


}
