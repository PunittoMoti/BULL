using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullObject : MonoBehaviour
{
    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private bool mGetAttackPoint;
    private bool mAttackSet;
    private float mSpeed = 3.0f;
    private float mStanTime = 3.0f;
    private float mNowStanTime;



    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowStanTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        TargetSonar();

        if (!mGetAttackPoint) 
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];
            mGetAttackPoint = true;
        }
        else
        {
            if (!mAttackSet && mSelectAttackPoint.transform.position != this.transform.position)
            {
                //スタート位置、ターゲットの座標、速度
                transform.position = Vector3.MoveTowards(
                  transform.position,
                  mSelectAttackPoint.transform.position,
                  mSpeed * Time.deltaTime);
            }
            else if (!mAttackSet && mSelectAttackPoint.transform.position == this.transform.position)
            {
                mAttackSet = true;
                mNowStanTime = 0.0f;
            }
            else if (mAttackSet && mNowStanTime <= mStanTime)
            {
                mNowStanTime += Time.deltaTime;
            }
            else if (mAttackSet && mNowStanTime >= mStanTime)
            {
                mAttackSet = false;
                mGetAttackPoint = false;
            }
        }

        Debug.Log(mSelectAttackPoint);
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

            //Debug.Log(children[i] + ":" + instantDis);


        }

        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < target.Length; ++i)
        {
            //Debug.Log("target[" + i + "]" + target[i]);
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
 コードメモ；Attack
 概要：突撃のための準備移動と突進
 
必要機能
α版
○・取得したAttackPointsのうちランダムに1つ取得する
○・ランダムに取得したAttackPointsに移動する
○・移動後待機する
・プレイヤーに向かって突撃する(通り過ぎる)


 */