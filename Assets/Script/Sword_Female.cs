using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Female : MonoBehaviour
{
    public bool left = false;
    public enum HeroAnimationState
    {
        站立,
        行走,
        奔跑,
        跳跃准备,
        跳跃起,
        跳跃顶,
        跳跃落,
        跳跃底,
        跳跃结束,
        后跳,
        后跳斩,
        后跳斩结束,
        攻击一,
        攻击二,
        攻击三,
        上挑,
        三段斩一,
        三段斩一后摇,
        三段斩二,
        三段斩二后摇,
        三段斩三,
        三段斩三后摇
    }
    public HeroAnimationState heroAnimationState = HeroAnimationState.站立;

    public HeroAnimationState heroAnimationState_Prev = HeroAnimationState.站立;

    public float moveSpeed = 100.0f;
    public float AtkSpeed = 20f;
    public float jumpForce = 5.0f;
    public float height;
    public Vector3 xyz;

    public bool isSkill = false;
    public bool isJump = false;
    public bool isJumpBack = false;
    public bool isMove = false;
    public bool isAtk = false;
    public bool isUpperSlash = false;
    public bool isTripleSlash = false;

    public bool canTurn = true;
    public bool canJump = true;
    public bool canJumpBack = true;
    public bool canMove = true;
    public bool canAtk = true;
    public bool canUpperSlash = true;
    public bool canTripleSlash = true;



    float doubleClickToRun_Timer;
    float jumpPositionY;
    float jumpReadyTime;
    float jumpForce_Local;

    public int tripleSlashCount = 3;
    public int tripleSlashCount_Local;

    public GameObject tripleSlash02_pre;

    void Awake()
    {
        Application.targetFrameRate = 120;
    }

    void Start()
    {
        xyz = transform.position;
        xyz.z = 0;
        jumpForce_Local = jumpForce;
    }

    void Update()
    {
        if (heroAnimationState == HeroAnimationState.上挑 ||
            heroAnimationState == HeroAnimationState.后跳斩 ||
            heroAnimationState == HeroAnimationState.后跳斩结束 ||
            heroAnimationState == HeroAnimationState.三段斩一 ||
            heroAnimationState == HeroAnimationState.三段斩一后摇 ||
            heroAnimationState == HeroAnimationState.三段斩三 ||
            heroAnimationState == HeroAnimationState.三段斩三后摇 ||
            heroAnimationState == HeroAnimationState.三段斩二 ||
            heroAnimationState == HeroAnimationState.三段斩二后摇
        )
        {
            isSkill = true;
        }
        else
        {
            isSkill = false;
        }

        if (heroAnimationState == HeroAnimationState.后跳 ||
            heroAnimationState == HeroAnimationState.跳跃准备 ||
            heroAnimationState == HeroAnimationState.跳跃底 ||
            heroAnimationState == HeroAnimationState.跳跃结束 ||
            heroAnimationState == HeroAnimationState.跳跃落 ||
            heroAnimationState == HeroAnimationState.跳跃起 ||
            heroAnimationState == HeroAnimationState.跳跃顶)
        {
            isJump = true;
        }
        else
        {
            isJump = false;
        }

        Turn_Logic();
        Move_Logic();
        Jump_Logic();
        JumpBack_Logic();
        Atk_Logic();
        UpperSlash_Logic();
        TripleSlash_Logic();



        foreach (Transform t in gameObject.transform)
        {
            t.GetComponent<SpritePlayCtrl>().isFlip = left;
        }


        PlayAnimation(heroAnimationState);
        heroAnimationState_Prev = heroAnimationState;
        transform.position = new Vector3(xyz.x, xyz.y + xyz.z, 0);

    }


    void Turn_Logic()
    {
        if (!isAtk && !isJumpBack && !isSkill)
        {
            canTurn = true;
        }
        else
        {
            canTurn = false;
        }

        if (canTurn && Input.GetKeyDown(KeyCode.LeftArrow))
        {

            left = true;
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().isFlip = true;

        }
        else if (canTurn && Input.GetKeyDown(KeyCode.RightArrow))
        {

            left = false;
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().isFlip = false;

        }

    }



    void Move_Logic()
    {
        if (!isJump && !isAtk && !isUpperSlash && !isJumpBack && !isSkill)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }

        if (canMove && heroAnimationState != HeroAnimationState.奔跑 && heroAnimationState != HeroAnimationState.行走 && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if (Time.time - doubleClickToRun_Timer >= 0.25f)
            {
                heroAnimationState = HeroAnimationState.行走;
                isMove = true;
                doubleClickToRun_Timer = Time.time;
            }
            else
            {
                heroAnimationState = HeroAnimationState.奔跑;
                isMove = true;
                doubleClickToRun_Timer = Time.time;
            }
        }

        if (heroAnimationState == HeroAnimationState.行走)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                xyz.x += moveSpeed * BoolToInt(left) * -1 * Time.deltaTime;
                if (Input.GetKey(KeyCode.UpArrow))
                    xyz.y += moveSpeed * Time.deltaTime * 0.5f;
                else if (Input.GetKey(KeyCode.DownArrow))
                    xyz.y += moveSpeed * Time.deltaTime * -0.5f;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
                xyz.y += moveSpeed * Time.deltaTime * 0.5f;
            else if (Input.GetKey(KeyCode.DownArrow))
                xyz.y += moveSpeed * Time.deltaTime * -0.5f;
            else
            {
                heroAnimationState = HeroAnimationState.站立;
                isMove = false;
            }
        }
        else if (heroAnimationState == HeroAnimationState.奔跑)
        {

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {

                xyz.x += moveSpeed * BoolToInt(left) * -2 * Time.deltaTime;


            }
            else
            {
                heroAnimationState = HeroAnimationState.站立;
                isMove = false;
            }

            if (Input.GetKey(KeyCode.UpArrow))
                xyz.y += moveSpeed * Time.deltaTime * 0.5f;
            else if (Input.GetKey(KeyCode.DownArrow))
                xyz.y += moveSpeed * Time.deltaTime * -0.5f;


        }




    }



    void Jump_Logic()
    {

        if (!isAtk && !isJump && !isUpperSlash && !isJumpBack && !isSkill)
        {
            canJump = true;
        }
        else
        {

            canJump = false;

        }


        if (canJump && Input.GetKeyDown(KeyCode.C))
        {
            jumpPositionY = xyz.y;
            jumpForce_Local = jumpForce;
            jumpReadyTime = 0;
            PlayAnimation(heroAnimationState = HeroAnimationState.跳跃准备);
            isJump = true;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_jump", false, false);
        }

        if (heroAnimationState == HeroAnimationState.跳跃准备)
        {

            jumpReadyTime += Time.deltaTime;
            if (jumpReadyTime > 0.2)
            {
                heroAnimationState = HeroAnimationState.跳跃起;
                jumpReadyTime = 0;

            }


        }


        if (heroAnimationState == HeroAnimationState.跳跃起 || heroAnimationState == HeroAnimationState.跳跃顶)
        {

            jumpForce_Local -= Time.deltaTime * 10;
            if (jumpForce_Local > 0)
            {
                xyz.z += jumpForce_Local * Time.deltaTime * 80;
                if (AnimationFlag(2, 0, false))
                {
                    heroAnimationState = HeroAnimationState.跳跃顶;

                }


            }
            else
            {
                heroAnimationState = HeroAnimationState.跳跃落;
            }


            if (Input.GetKey(KeyCode.RightArrow))
            {

                left = false;
                xyz.x += moveSpeed * Time.deltaTime * 2;

            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {

                left = true;
                xyz.x -= moveSpeed * Time.deltaTime * 2;

            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {

                xyz.y += moveSpeed * Time.deltaTime * 0.5f;

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {

                xyz.y -= moveSpeed * Time.deltaTime * 0.5f;

            }







        }


        if (heroAnimationState == HeroAnimationState.跳跃落 || heroAnimationState == HeroAnimationState.跳跃底)
        {

            jumpForce_Local += Time.deltaTime * 10;
            if (xyz.z > 0)
            {

                xyz.z -= jumpForce_Local * Time.deltaTime * 80;
                if (AnimationFlag(2, 0, false))
                {

                    heroAnimationState = HeroAnimationState.跳跃底;

                }

            }
            else
            {

                heroAnimationState = HeroAnimationState.跳跃结束;
                xyz.z = 0;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {

                left = false;
                xyz.x += moveSpeed * Time.deltaTime * 2;

            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {

                left = true;
                xyz.x -= moveSpeed * Time.deltaTime * 2;

            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {

                xyz.y += moveSpeed * Time.deltaTime * 0.5f;

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {

                xyz.y -= moveSpeed * Time.deltaTime * 0.5f;

            }

        }


        if (heroAnimationState == HeroAnimationState.跳跃结束)
        {


            jumpReadyTime += Time.deltaTime;
            if (jumpReadyTime > 0.08)
            {

                jumpReadyTime = 0;
                heroAnimationState = HeroAnimationState.站立;
                isJump = false;

            }

        }


    }


    void JumpBack_Logic()
    {

        if (!isJump && !isUpperSlash && !isJumpBack)
        {

            canJumpBack = true;

        }
        else
        {

            canJumpBack = false;

        }


        if (canJumpBack && Input.GetKeyDown(KeyCode.V))
        {

            jumpForce_Local = jumpForce / 2;
            PlayAnimation(heroAnimationState = HeroAnimationState.后跳);
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_back", false, false);

            isJumpBack = true;


        }


        if (heroAnimationState == HeroAnimationState.后跳 && Input.GetKeyDown(KeyCode.X))
        {

            heroAnimationState = HeroAnimationState.后跳斩;
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑武器/beamswda_01", false, false);
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_atk_03", false, false);



        }

        if (heroAnimationState == HeroAnimationState.后跳斩 && AnimationFlag(4, 0, false))
        {


            heroAnimationState = HeroAnimationState.后跳斩结束;


        }

        if (heroAnimationState == HeroAnimationState.后跳 || heroAnimationState == HeroAnimationState.后跳斩 || heroAnimationState == HeroAnimationState.后跳斩结束)
        {

            jumpForce_Local -= Time.deltaTime * 10;
            xyz.z += jumpForce_Local * Time.deltaTime * 80;
            xyz.x -= moveSpeed * 2f * Time.deltaTime * -BoolToInt(left);
            if (jumpForce_Local <= -jumpForce / 2)
            {

                xyz.z = 0;
                heroAnimationState = HeroAnimationState.站立;
                isJumpBack = false;

            }

        }

    }


    void Atk_Logic()
    {

        if (!isAtk && !isJump && !isJumpBack && !isSkill)
        {

            canAtk = true;

        }
        else
        {

            canAtk = false;

        }


        if (heroAnimationState != HeroAnimationState.攻击一 && heroAnimationState != HeroAnimationState.攻击二 && heroAnimationState != HeroAnimationState.攻击三)
        {

            isAtk = false;

        }


        if (canAtk && Input.GetKeyDown(KeyCode.X))
        {

            heroAnimationState = HeroAnimationState.攻击一;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_atk_01", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑武器/beamswda_01", false, false);

            isAtk = true;

        }





        if (heroAnimationState == HeroAnimationState.攻击一 && Input.GetKeyDown(KeyCode.X) && AnimationFlag(9, 4, true))
        {

            heroAnimationState = HeroAnimationState.攻击二;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_atk_02", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑武器/beamswda_02", false, false);



        }
        else if (heroAnimationState == HeroAnimationState.攻击一 && AnimationFlag(9, 0, false))
        {

            heroAnimationState = HeroAnimationState.站立;
            isAtk = false;

        }
        else if (heroAnimationState == HeroAnimationState.攻击二 && Input.GetKeyDown(KeyCode.X) && AnimationFlag(10, 5, true))
        {

            heroAnimationState = HeroAnimationState.攻击三;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_atk_03", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑武器/beamswda_03", false, false);



        }
        else if (heroAnimationState == HeroAnimationState.攻击二 && AnimationFlag(10, 0, false))
        {

            heroAnimationState = HeroAnimationState.站立;
            isAtk = false;

        }
        else if (heroAnimationState == HeroAnimationState.攻击三 && AnimationFlag(8, 0, false))
        {

            heroAnimationState = HeroAnimationState.站立;
            isAtk = false;

        }
    }


    void UpperSlash_Logic()
    {

        if (!isJump && !isSkill)
        {
            canUpperSlash = true;
        }
        else
        {
            canUpperSlash = false;
        }

        if (Input.GetKeyDown(KeyCode.Z) && canUpperSlash)
        {

            heroAnimationState = HeroAnimationState.上挑;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_upslash", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑效果/ryusim_ready", false, false);

            isUpperSlash = true;
            isAtk = false;
        }

        if (heroAnimationState == HeroAnimationState.上挑 && AnimationFlag(8, 0, false))
        {

            heroAnimationState = HeroAnimationState.站立;
            isUpperSlash = false;

        }
    }

    void TripleSlash_Logic()
    {

        if (!isSkill && Input.GetKeyDown(KeyCode.F))
        {

            heroAnimationState = HeroAnimationState.三段斩一;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_triple1", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑效果/gorecross_atk3", false, false);





            //			transform.GetChild(1).GetComponent<SpritePlayCtrl> ().PlaySpriteAnimation (1 , 4 , AtkSpeed , left);


            if (Input.GetKey(KeyCode.LeftArrow))
            {

                left = true;

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {

                left = false;

            }
        }


        if (heroAnimationState == HeroAnimationState.三段斩一 && AnimationFlag(1, 2, false))
        {
            GameObject ts02 = Instantiate(tripleSlash02_pre);
            ts02.transform.parent = transform;
            ts02.transform.localPosition = Vector3.zero;
            ts02.GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(1, 4, AtkSpeed, left);
        }

        if (heroAnimationState == HeroAnimationState.三段斩一 && AnimationFlag(5, 0, false))
        {
            heroAnimationState = HeroAnimationState.三段斩一后摇;
        }

        if (heroAnimationState == HeroAnimationState.三段斩一后摇 && AnimationFlag(0, 1, false))
        {
            heroAnimationState = HeroAnimationState.站立;
        }
        else if (heroAnimationState == HeroAnimationState.三段斩一后摇 && Input.GetKeyDown(KeyCode.F))
        {
            heroAnimationState = HeroAnimationState.三段斩二;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_triple2", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑效果/gorecross_atk3", false, false);
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                left = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                left = false;
            }
        }

        if (heroAnimationState == HeroAnimationState.三段斩二 && AnimationFlag(4, 0, false))
        {
            heroAnimationState = HeroAnimationState.三段斩二后摇;
        }

        if (heroAnimationState == HeroAnimationState.三段斩二后摇 && AnimationFlag(0, 1, false))
        {
            heroAnimationState = HeroAnimationState.站立;
        }
        else if (heroAnimationState == HeroAnimationState.三段斩二后摇 && Input.GetKeyDown(KeyCode.F))
        {
            heroAnimationState = HeroAnimationState.三段斩三;
            Sound_ctrl.PlaySound("sounds/char/swordman/sm_triple3", false, false);
            Sound_ctrl.PlaySound("ogg/勇士/男鬼剑效果/gorecross_atk3", false, false);

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                left = true;

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                left = false;
            }
        }

        if (heroAnimationState == HeroAnimationState.三段斩三 && AnimationFlag(3, 0, false))
        {
            heroAnimationState = HeroAnimationState.三段斩三后摇;
        }

        if (heroAnimationState == HeroAnimationState.三段斩三后摇 && AnimationFlag(0, 1, false))
        {
            heroAnimationState = HeroAnimationState.站立;
        }

        if (heroAnimationState == HeroAnimationState.三段斩一 ||
            heroAnimationState == HeroAnimationState.三段斩二 ||
            heroAnimationState == HeroAnimationState.三段斩三
        )
        {
            xyz.x += moveSpeed * Time.deltaTime * -BoolToInt(left) * 4f;
        }
        else if (heroAnimationState == HeroAnimationState.三段斩一后摇 ||
          heroAnimationState == HeroAnimationState.三段斩二后摇 ||
          heroAnimationState == HeroAnimationState.三段斩三后摇)
        {
            xyz.x += moveSpeed * Time.deltaTime * -BoolToInt(left) * 0.5f;
        }
    }



    public void PlayAnimation(HeroAnimationState sta)
    {
        if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.站立)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(186, 189, 5, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.行走)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(14, 23, moveSpeed / 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.奔跑)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(152, 159, moveSpeed / 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃准备)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(37, 37, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃起)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(38, 38, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃顶)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(39, 40, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃落)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(40, 41, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃底)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(42, 42, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.跳跃结束)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(133, 133, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.后跳)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(130, 133, 10, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.后跳斩)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(134, 138, AtkSpeed * 1.0f, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.后跳斩结束)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(139, 139, -1, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.攻击一)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(50, 60, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.攻击二)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(11, 21, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.攻击三)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(34, 42, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.上挑)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(34, 42, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩一)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(189, 194, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩一后摇)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(194, 195, AtkSpeed / 7, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩二)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(195, 199, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩二后摇)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(199, 200, AtkSpeed / 7, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩三)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(200, 203, AtkSpeed, left);
        else if (heroAnimationState_Prev != heroAnimationState && sta == HeroAnimationState.三段斩三后摇)
            transform.GetChild(0).GetComponent<SpritePlayCtrl>().PlaySpriteAnimation(204, 205, AtkSpeed / 7, left);
    }


    public int BoolToInt(bool b)
    {
        return b ? 1 : -1;
    }


    public bool AnimationFlag(int prev, int current, bool mode)
    {

        if (!mode && transform.GetChild(0).GetComponent<SpritePlayCtrl>().prevFrameIndex == prev && transform.GetChild(0).GetComponent<SpritePlayCtrl>().currentFrameIndex == current)
        {
            return true;
        }
        else if (mode && transform.GetChild(0).GetComponent<SpritePlayCtrl>().prevFrameIndex <= prev && transform.GetChild(0).GetComponent<SpritePlayCtrl>().currentFrameIndex >= current)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
