using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class BullObject : MonoBehaviour
{
    private enum BULL_STATUS
    {
        SONAR,//索敵
        SET_ATTACK,//突撃準備
        ATTACK,//突撃
        CHECK_EVASION,//回避入力チェック
        EVASION,//回避後突撃
        FAILUREEVASION,//回避失敗
        STUN_ATTACK,//攻撃側スタン
        STUN_DAMAGE //攻撃された側のスタン
    }

    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private GameObject mSelectAttackEndPoint;
    private bool mGetAttackPoint;
    private bool mAttackSet;
    private bool mCheckEvasion;

    private float mSpeed;
    private float mSpeedOrigin = 3.0f;
    private float mSpeedMagnification =1.0f;
    private float mSetAttackTime = 3.0f;
    private float mNowSetAttackTime;
    private float mAttackStunTime = 5.0f;
    private float mDamageStunTime = 3.0f;
    private float mNowStunTime;

    private float[] mSpeedList = new float[] { 1, 1.1f, 1.3f, 1.6f, 2.0f };
    private int mSpeedListCount;
    private int mStickStatuStackCount;



    private BULL_STATUS bullStatus = BULL_STATUS.SONAR;
    private STICK_STATUS mCheckStickStatus;
    private STICK_STATUS[] mCheckStickStatuStack;


    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowSetAttackTime = 0.0f;
        mSpeedListCount = 0;
        mSpeed = mSpeedOrigin;
        mCheckEvasion = false;
        mStickStatuStackCount = 0;
    }

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
                StackStickInputStack();
                Attack();
                EvasionCheck();
                break;
            //回避後
            case BULL_STATUS.EVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.red;
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
                    Debug.Log("ATK");
                    bullStatus = BULL_STATUS.STUN_DAMAGE;

                }

                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //プレイヤーのスタンフラグ有効化
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
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
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;
            case BULL_STATUS.EVASION:
                if (collision.gameObject.name == "BULL")
                {
                    Debug.Log("EVASION");
                    bullStatus = BULL_STATUS.STUN_ATTACK;

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

                        for(int i=0;i< mCheckStickStatuStack.Length; i++)
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

    //離れたとき
    void OnTriggerExit(Collider collision)
    {
        /*
        switch (bullStatus)
        {
            case BULL_STATUS.CHECK_EVASION:
                if (collision.gameObject.name == "Player")
                {
                    mCheckEvasion = false;
                    bullStatus = BULL_STATUS.ATTACK;
                }
                break;
        }
        */
    }


    void TargetSonar()
    {
        /* ターゲットのポジションを取得 */
        Vector3 targetPos = this.transform.position;

        /* プレイヤーのポジションを取得 */
        Vector3 playerPos = mPrayer.transform.position;

        /* ターゲットとプレイヤーの距離を取得 */
        float dis = Vector3.Distance(targetPos, playerPos);

        //Debug.Log("距離:" + dis);

        mAttackPoints = mPrayer.transform.Find("AttackPoins").gameObject;

        // 子オブジェクトを格納する配列作成
        var children = new GameObject[mAttackPoints.transform.childCount];


        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < children.Length; ++i)
        {
            children[i] = mAttackPoints.transform.GetChild(i).gameObject;
        }

        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < children.Length; ++i)
        {
            float instantDis = Vector3.Distance(targetPos, children[i].transform.position);

            //確認が0でなければ
            if (i != 0)
            {
                float targetDis = Vector3.Distance(targetPos, target[0].transform.position);

                if (targetDis >= instantDis)
                {
                    target[2] = target[1];
                    target[1] = target[0];
                    target[0] = children[i];
                }
                else
                {
                    if (target[1] != null)
                    {
                        targetDis = Vector3.Distance(targetPos, target[1].transform.position);
                        if (targetDis >= instantDis)
                        {
                            target[2] = target[1];
                            target[1] = children[i];
                        }
                        else if (target[2] != null)
                        {
                            targetDis = Vector3.Distance(targetPos, target[2].transform.position);
                            if (targetDis >= instantDis)
                            {
                                target[2] = children[i];
                            }

                        }
                        else
                        {
                            target[1] = children[i];
                        }
                    }
                    else
                    {
                        target[1] = children[i];
                    }
                }
            }
            else
            {
                target[0] = mAttackPoints.transform.GetChild(i).gameObject;
            }

        }

        //状態遷移
        bullStatus = BULL_STATUS.SET_ATTACK;
    }

    void SetAttack()
    {
        if (!mGetAttackPoint)
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];
            mGetAttackPoint = true;
            //対応する回避入力を取得
            mCheckStickStatus = mSelectAttackPoint.GetComponent<AttackPointObject>().GetAnswerStickStatusNormal();

            Debug.Log("対応回避入力：" + mCheckStickStatus);
        }
        // 突撃準備位置まで移動する
        else if (!mAttackSet && mSelectAttackPoint.transform.position != this.transform.position)
        {
            //スタート位置、ターゲットの座標、速度
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //突撃準備位置に到着した場合、準備を始める
        else if (!mAttackSet && mSelectAttackPoint.transform.position == this.transform.position)
        {
            mAttackSet = true;
            mNowSetAttackTime = 0.0f;
        }
        //突撃準備位置で準備時間分待機する
        else if (mAttackSet && mNowSetAttackTime <= mSetAttackTime / mSpeedMagnification)
        {
            mNowSetAttackTime += Time.deltaTime;
        }
        //突撃準備完了+突撃目標取得
        else if (mAttackSet && mNowSetAttackTime >= mSetAttackTime / mSpeedMagnification)
        {
            //移動先Objectを取得
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackEndPoint = instantAttackPointObject.GetAttackEndPoint();

            //フラグ初期化
            mGetAttackPoint = false;
            mAttackSet = false;

            //状態遷移
            bullStatus = BULL_STATUS.ATTACK;
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

    void SpeedUp()
    {
        mSpeedListCount += 1;

        if (mSpeedListCount < 5)
        {
            mSpeedMagnification = mSpeedList[mSpeedListCount];
            mSpeed = mSpeedOrigin * mSpeedMagnification;

        }
        else
        {
            mSpeedMagnification += 0.5f;
            mSpeed = mSpeedOrigin * mSpeedMagnification;

        }
    }

    void SpeedDown()
    {
        if (mSpeedListCount > 0)
        {
            mSpeedListCount -= 1;
        }

        if (mSpeedListCount < 5)
        {
            mSpeedMagnification = mSpeedList[mSpeedListCount];
            mSpeed = mSpeedOrigin * mSpeedMagnification;

        }
        else
        {
            mSpeedMagnification -= 0.5f;
            mSpeed = mSpeedOrigin * mSpeedMagnification;

        }
    }

    void EvasionCheck()
    {
        if (!mCheckEvasion) return;

        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        switch (mCheckStickStatuStack[0])
        {
            case STICK_STATUS.UP:

                if(mCheckStickStatuStack[1] == STICK_STATUS.DWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2,3] { { STICK_STATUS.RIGHT, STICK_STATUS.DWON, STICK_STATUS.LEFT } , { STICK_STATUS.LEFT, STICK_STATUS.DWON, STICK_STATUS.RIGHT } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[2] == STICK_STATUS.RIGHT)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[2] == STICK_STATUS.LEFT)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHTUP:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP }, { STICK_STATUS.LEFTUP, STICK_STATUS.LEFT, STICK_STATUS.RIGHTDWON } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTDWON)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[2] == STICK_STATUS.LEFTUP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHT:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.DWON, STICK_STATUS.LEFT, STICK_STATUS.UP }, { STICK_STATUS.UP, STICK_STATUS.LEFT, STICK_STATUS.DWON } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[2] == STICK_STATUS.DWON)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[2] == STICK_STATUS.UP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHTDWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP, STICK_STATUS.RIGHTUP }, { STICK_STATUS.RIGHTUP, STICK_STATUS.LEFTUP, STICK_STATUS.LEFTDWON } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[2] == STICK_STATUS.LEFTDWON)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTUP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.DWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.UP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }

                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFT, STICK_STATUS.UP, STICK_STATUS.RIGHT }, { STICK_STATUS.RIGHT, STICK_STATUS.UP, STICK_STATUS.LEFT } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[2] == STICK_STATUS.LEFT)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[2] == STICK_STATUS.RIGHT)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.LEFTDWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFTUP, STICK_STATUS.RIGHTUP, STICK_STATUS.RIGHTDWON }, { STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[2] == STICK_STATUS.LEFTUP)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTDWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }
                break;
            case STICK_STATUS.LEFT:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.UP, STICK_STATUS.RIGHT, STICK_STATUS.DWON }, { STICK_STATUS.DWON, STICK_STATUS.RIGHT, STICK_STATUS.UP } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[2] == STICK_STATUS.UP)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[2] == STICK_STATUS.DWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }


                break;
            case STICK_STATUS.LEFTUP:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHTUP, STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON }, { STICK_STATUS.LEFTDWON, STICK_STATUS.RIGHTDWON, STICK_STATUS.RIGHTUP } };

                    //時計回りの回転回避アクションの場合
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTUP)
                    {
                        RollEvasion(checkList);
                    }
                    //反時計回りの回転回避アクションの場合
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[2] == STICK_STATUS.LEFTDWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }
                break;


        }
    }

    void StackStickInputStack()
    {
        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        //最新にスタックされている状態と違うかつスティック入力状態が中央でなければ
        if (playerObject.GetmStickStatus() != mCheckStickStatuStack[mStickStatuStackCount] && playerObject.GetmStickStatus() != STICK_STATUS.NOTINPUT)
        {
            //スタックカウント上昇
            mStickStatuStackCount += 1;
            //次に入力された状態を取得
            mCheckStickStatuStack[mStickStatuStackCount] = playerObject.GetmStickStatus();
            Debug.Log("スタック[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
        }
    }

    void RollEvasion(STICK_STATUS[,] checkList)
    {
        int count = 0;
        int listCount = 0;
        for (int i = 1; i < mCheckStickStatuStack.Length && count < 2 && listCount < 3; i++)
        {
            if (mCheckStickStatuStack[i] == checkList[0, listCount])
            {
                listCount++;
                count = 0;
            }
            else if (mCheckStickStatuStack[i] == STICK_STATUS.NOTINPUT)
            {
                break;
            }
            else
            {
                count++;
            }

        }


        if (listCount >= 3)

        {
            Debug.Log("回転回避成功:"+ mSelectAttackPoint);

            //進行方向変更
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackEndPoint = instantAttackPointObject.GetRollEvasionEndPoint();
            //スピードアップ
            SpeedUp();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    void ReverseRollEvasion(STICK_STATUS[,] checkList)
    {
        int count = 0;
        int listCount = 0;
        for (int i = 1; i < mCheckStickStatuStack.Length && count < 2 && listCount < 3; i++)
        {
            if (mCheckStickStatuStack[i] == checkList[0, listCount])
            {
                listCount++;
                count = 0;
            }
            else if (mCheckStickStatuStack[i] == STICK_STATUS.NOTINPUT)
            {
                break;
            }
            else
            {
                count++;
            }

        }


        if (listCount >= 3)

        {
            Debug.Log("回転回避成功:" + mSelectAttackPoint);

            //進行方向変更
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackEndPoint = instantAttackPointObject.GetReverseRollEvasionAttackEndPoint();
            //スピードアップ
            SpeedUp();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }
}

/*
 索敵実装	
○	索敵判定実装
	索敵状態復帰実装
突進実装	
	突撃準備実装
	直進突撃実装
スタン実装	
	回避後の衝突した闘牛スタン実装
	衝突された闘牛スタン実装
速度変化実装	
	回転回避時のスピードアップ
	回避失敗時のスピードダウン
 */

/*
コードメモ：TargetSonar
概要：索敵機能のテストコード

必要機能
α版
○・プレイヤーとの距離を取得する
○・プレイヤー/NPCのAttackPointsオブジェクトを取得する
○・AttackPoints以下にあるオブジェクトの距離を取得する
○・AttackPoints以下にあるオブジェクトの距離を比較する
○・AttackPoints以下にあるオブジェクトで近いものを3つ取得する

マスター版
・NPCとの距離を取得する
・プレイヤーとNPCの距離を比較する

 */

/*
 コードメモ；SetAttack/Attack
 概要：突撃のための準備移動と突進
 
必要機能
α版
○・取得したAttackPointsのうちランダムに1つ取得する
○・ランダムに取得したAttackPointsに移動する
○・移動後待機する
○・突撃終点を取得する
○・突撃終点に向かって突撃する
 */

/*
  コードメモ；SelectStun
 概要：突撃のための準備移動と突進
 
必要機能
α版
・どちらに充てられたか確認
・待機する
 */