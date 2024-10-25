using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSonar : MonoBehaviour
{
    private GameObject mPrayer;
    private GameObject mAttackPoints;




    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
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
        var  target = new GameObject[3];

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
            if (i != 0) {
                float targetDis = Vector3.Distance(targetPos, target[0].transform.position);
                
                if(targetDis >= instantDis) {
                    target[2] = target[1];
                    target[1] = target[0];
                    target[0] = children[i];
                }
                else
                {
                    if (target[1] != null) {
                        targetDis = Vector3.Distance(targetPos, target[1].transform.position);
                        if (targetDis >= instantDis) {
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
            else{
                target[0] = mAttackPoints.transform.GetChild(i).gameObject;
            }

            Debug.Log(children[i]+":"+ instantDis);

            
        }

        // 0～個数-1までの子を順番に配列に格納
        for (var i = 0; i < target.Length; ++i) 
        {
            Debug.Log("target["+i+"]"+target[i]);
        }
    }
}


/*
コードメモ
概要：索敵機能のテストコード

必要機能
α版
○・プレイヤーとの距離を取得する
○・プレイヤー/NPCのAttackPointsオブジェクトを取得する
○・AttackPoints以下にあるオブジェクトの距離を取得する
・AttackPoints以下にあるオブジェクトの距離を比較する
・AttackPoints以下にあるオブジェクトで近いものを3つ取得する

マスター版
・NPCとの距離を取得する
・プレイヤーとNPCの距離を比較する

 */