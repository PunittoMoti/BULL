using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullObject : MonoBehaviour
{
    private enum BULL_STATUS
    {
        SONAR,//索敵
        SET_ATTACK,//突撃準備
        ATTACK,//突撃
        EVASION,//回避後突撃
        STUN_ATTACK,//攻撃側スタン
        STUN_DAMAGE //攻撃された側のスタン
    }

    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private bool mGetAttackPoint;
    private bool mAttackSet;

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


    private BULL_STATUS bullStatus = BULL_STATUS.SONAR;

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
    }

    // Update is called once per frame
    void Update()
    {
        switch (bullStatus)
        {
            case BULL_STATUS.SONAR:
                TargetSonar();
                break;
            case BULL_STATUS.SET_ATTACK:
                SetAttack();
                break;
            case BULL_STATUS.ATTACK:
                Attack();
                break;
            case BULL_STATUS.EVASION:
                Attack();
                break;
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

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.name == "BULL")
        {
            switch (bullStatus)
            {
                case BULL_STATUS.ATTACK:
                    Debug.Log("ATK");
                    bullStatus = BULL_STATUS.STUN_DAMAGE;
                    break;
                case BULL_STATUS.EVASION:
                    Debug.Log("EVASION");
                    bullStatus = BULL_STATUS.STUN_ATTACK;
                    break;
            }
        }
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
            //Debug.Log("ランダムゲット" + mSelectAttackPoint);
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
        else if (mAttackSet && mNowSetAttackTime <= mSetAttackTime)
        {
            mNowSetAttackTime += Time.deltaTime;
        }
        //突撃準備完了+突撃目標取得
        else if (mAttackSet && mNowSetAttackTime >= mSetAttackTime)
        {
            //移動先Objectを取得
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackPoint = instantAttackPointObject.GetAttackEndPoint();

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
        if (mSelectAttackPoint.transform.position != this.transform.position)
        {
            //スタート位置、ターゲットの座標、速度
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //突撃完了
        else if (mSelectAttackPoint.transform.position == this.transform.position)
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