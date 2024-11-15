using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;


public class CNormalBull : CBull
{
    // Update is called once per frame
    void Update()
    {
        switch (bullStatus)
        {
            //索敵
            case BULL_STATUS.SONAR:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.white;
                TargetSonar();
                break;
            //突撃準備
            case BULL_STATUS.SET_ATTACK:
                SetAttack();
                break;
            //突撃
            case BULL_STATUS.ATTACK:
                Attack();
                break;
            //回避判定チェック
            case BULL_STATUS.CHECK_EVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.red;
                StackStickInputStack();
                Attack();
                EvasionCheck();
                break;
            //回避判定チェック
            case BULL_STATUS.CHECK_JUSTEVASION:
                Debug.Log("ギリギリ");
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.gray;
                StackStickInputStack();
                Attack();
                EvasionCheck();
                break;
            //回避後
            case BULL_STATUS.EVASION:
                Attack();
                break;
            //回避失敗
            case BULL_STATUS.FAILUREEVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.blue;
                Attack();
                break;

            //ぶつけられた時のスタン
            case BULL_STATUS.STUN_ATTACK:
                if (mNowStunTime <= mAttackStunTime)
                {
                    mNowStunTime += Time.deltaTime;
                }
                else
                {
                    mNowStunTime = 0.0f;
                    bullStatus = BULL_STATUS.SONAR;
                }
                break;
            //ぶつかった時のスタン
            case BULL_STATUS.STUN_DAMAGE:
                if (mNowStunTime <= mDamageStunTime)
                {
                    mNowStunTime += Time.deltaTime;
                }
                else
                {
                    mNowStunTime = 0.0f;
                    bullStatus = BULL_STATUS.SONAR;
                }
                break;
        }

    }

    //当たった瞬間
    void OnTriggerEnter(Collider collision)
    {
        switch (bullStatus)
        {
            case BULL_STATUS.ATTACK:
                if (collision.gameObject.name == "BULL")
                {
                    if (collision.gameObject.GetComponent<CBull>().GetBullStatus() == BULL_STATUS.EVASION)
                    {
                        bullStatus = BULL_STATUS.STUN_DAMAGE;
                    }

                }

                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //プレイヤーのスタンフラグ有効化
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;
            case BULL_STATUS.CHECK_EVASION:
                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //プレイヤーのスタンフラグ有効化
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;
            case BULL_STATUS.CHECK_JUSTEVASION:
                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //プレイヤーのスタンフラグ有効化
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;

            case BULL_STATUS.EVASION:
                if (collision.gameObject.name == "BULL")
                {
                    if (collision.gameObject.GetComponent<CBull>().GetBullStatus() == BULL_STATUS.ATTACK)
                    {
                        bullStatus = BULL_STATUS.STUN_ATTACK;
                    }

                }
                break;
        }




    }

    //当たり続けている間
    void OnTriggerStay(Collider collision)
    {
        switch (bullStatus)
        {
            case BULL_STATUS.ATTACK:
                if (collision.gameObject.name == "Player")
                {
                    //プレイヤーの入力がBULLが持っている受付入力と一致していれば
                    if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == mCheckStickStatus)
                    {
                        bullStatus = BULL_STATUS.CHECK_EVASION;
                        //スティック入力状態初期化
                        mCheckStickStatuStack = new STICK_STATUS[8];

                        for (int i = 0; i < mCheckStickStatuStack.Length; i++)
                        {
                            mCheckStickStatuStack[i] = STICK_STATUS.NOTINPUT;
                        }

                        mCheckStickStatuStack[0] = mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus();
                        mStickStatuStackCount = 0;
                        Debug.Log("スタック[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
                        mCheckEvasion = true;
                    }
                }

                if (collision.gameObject.name == "JustArea")
                {
                    //プレイヤーの入力がBULLが持っている受付入力と一致していれば
                    if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == mCheckStickStatus)
                    {
                        bullStatus = BULL_STATUS.CHECK_JUSTEVASION;
                        //スティック入力状態初期化
                        mCheckStickStatuStack = new STICK_STATUS[8];

                        for (int i = 0; i < mCheckStickStatuStack.Length; i++)
                        {
                            mCheckStickStatuStack[i] = STICK_STATUS.NOTINPUT;
                        }

                        mCheckStickStatuStack[0] = mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus();
                        mStickStatuStackCount = 0;
                        Debug.Log("スタック[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
                        mCheckEvasion = true;
                    }

                }
                break;
        }
    }


    void Attack()
    {
        //突撃中
        if (mSelectAttackEndPoint.transform.position != this.transform.position)
        {
            //スタート位置、ターゲットの座標、速度
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackEndPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //突撃完了
        else if (mSelectAttackEndPoint.transform.position == this.transform.position)
        {
            mAttackSet = false;
            mGetAttackPoint = false;
            //Debug.Log("凸完了" + mSelectAttackPoint);

            //状態遷移
            bullStatus = BULL_STATUS.SONAR;
        }

    }

}
