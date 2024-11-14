using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;


public class CBull : MonoBehaviour
{
    public enum BULL_STATUS
    {
        SONAR,//索敵
        SET_ATTACK,//突撃準備
        ATTACK,//突撃
        CHECK_EVASION,//回避入力チェック
        CHECK_JUSTEVASION,//ギリギリ回避入力チェック
        EVASION,//回避後突撃
        FAILUREEVASION,//回避失敗
        STUN_ATTACK,//攻撃側スタン
        STUN_DAMAGE //攻撃された側のスタン
    }

    //共通変数(子クラスでも使用)
    protected GameObject mPrayer;//プレイヤー
    protected GameObject mGameManager;//ゲームマネージャー
    protected GameObject mAttackPoints;//全ての突撃位置
    protected GameObject[] target;//突撃位置候補
    protected GameObject mSelectAttackPoint;//選択された突撃位置
    protected GameObject mSelectAttackEndPoint;//突撃終了位置



    protected bool mGetAttackPoint;
    protected bool mAttackSet;
    protected bool mCheckEvasion;

    protected float mSpeed;
    protected float mSpeedOrigin = 3.0f;
    protected float mSpeedMagnification = 1.0f;
    protected float mSetAttackTime = 3.0f;
    protected float mNowSetAttackTime;
    protected float mAttackStunTime = 5.0f;
    protected float mDamageStunTime = 3.0f;
    protected float mNowStunTime;

    protected float[] mSpeedList = new float[] { 1, 1.1f, 1.3f, 1.6f, 2.0f };
    protected int mSpeedListCount;
    protected int mStickStatuStackCount;


    protected BULL_STATUS bullStatus = BULL_STATUS.SONAR;
    protected STICK_STATUS mCheckStickStatus;
    protected STICK_STATUS[] mCheckStickStatuStack;






    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        mGameManager = GameObject.Find("GameManager");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowSetAttackTime = 0.0f;
        mSpeedListCount = 0;
        mSpeed = mSpeedOrigin;
        mCheckEvasion = false;
        mStickStatuStackCount = 0;
    }

    //突撃位置索敵処理
    protected void TargetSonar()
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

    //突撃準備
    protected void SetAttack()
    {
        if (!mGetAttackPoint)
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];

            if (mSelectAttackPoint.GetComponent<AttackPointObject>().GetIsUse())
            {
                //状態遷移
                bullStatus = BULL_STATUS.SONAR;
                return;
            }

            //準備Point使用中に変更
            mSelectAttackPoint.GetComponent<AttackPointObject>().IsUseNo();

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

            //準備Point使用中に変更
            mSelectAttackPoint.GetComponent<AttackPointObject>().IsUseOFF();

            //状態遷移
            bullStatus = BULL_STATUS.ATTACK;
        }

    }

    protected void SpeedUp()
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

    protected void SpeedDown()
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

    protected void EvasionCheck()
    {
        if (!mCheckEvasion) return;

        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        switch (mCheckStickStatuStack[0])
        {
            case STICK_STATUS.UP:

                if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHT, STICK_STATUS.DWON, STICK_STATUS.LEFT }, { STICK_STATUS.LEFT, STICK_STATUS.DWON, STICK_STATUS.RIGHT } };

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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[1] == STICK_STATUS.DWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[1] == STICK_STATUS.LEFT)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[1] == STICK_STATUS.UP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHT)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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

    protected void StackStickInputStack()
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

    protected void RollEvasion(STICK_STATUS[,] checkList)
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
            mSelectAttackEndPoint = instantAttackPointObject.GetRollEvasionEndPoint();
            //スピードアップ
            SpeedUp();
            mGameManager.GetComponent<GameManager>().AddScoreRoll();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            SpeedDown();
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    protected void ReverseRollEvasion(STICK_STATUS[,] checkList)
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
            mGameManager.GetComponent<GameManager>().AddScoreRoll();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            SpeedDown();
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    public BULL_STATUS GetBullStatus()
    {
        return bullStatus;
    }


}
