using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
