using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static SValue;

public class PlayerObject : MonoBehaviour
{
    // Actionをインスペクターから編集できるようにする
    [SerializeField] private InputAction mStickUp;
    [SerializeField] private InputAction mStickRight;
    [SerializeField] private InputAction mStickDwon;
    [SerializeField] private InputAction mStickLeft;




    private STICK_STATUS mStickStatus;
    [SerializeField] private float mMaxStunTime;
    private float mNowStunTime;
    private float mStickValueUp;
    private float mStickValueRight;
    private float mStickValueDwon;
    private float mStickValueLeft;

    private bool mIsControl; 


    // 有効化
    private void OnEnable()
    {
        // InputActionを有効化
        // これをしないと入力を受け取れないことに注意
        mStickUp?.Enable();
        mStickRight?.Enable();
        mStickDwon?.Enable();
        mStickLeft?.Enable();
    }

    // 無効化
    private void OnDisable()
    {
        // 自身が無効化されるタイミングなどで
        // Actionを無効化する必要がある
        mStickUp?.Disable();
        mStickRight?.Disable();
        mStickDwon?.Disable();
        mStickLeft?.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        mStickStatus = STICK_STATUS.NOTINPUT;
        mIsControl = false;
        mNowStunTime = 0.0f;
    }

    private void Update()
    {
        if (!mIsControl)
        {
            // ステックの入力値を読み込む
            mStickValueUp = mStickUp.ReadValue<float>();
            mStickValueRight = mStickRight.ReadValue<float>();
            mStickValueDwon = mStickDwon.ReadValue<float>();
            mStickValueLeft = mStickLeft.ReadValue<float>();
            StickInput();
        }
        else
        {
            if(mNowStunTime>= mMaxStunTime)
            {
                Debug.Log("無敵終了");
                mNowStunTime = 0.0f;
                mIsControl = false;
            }
            else
            {
                mNowStunTime += Time.deltaTime;
            }
        }



    }

    void StickInput()
    {
        //上
        if(mStickValueUp == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.UP;
            // 入力値をログ出力

        }
        //右上
        else if (mStickValueRight == 1.0f && mStickValueUp == 1.0f)
        {
            mStickStatus = STICK_STATUS.RIGHTUP;
            // 入力値をログ出力

        }
        //右
        else if (mStickValueRight == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.RIGHT;
            // 入力値をログ出力

        }
        //右下
        else if (mStickValueRight == 1.0f && mStickValueDwon == 1.0f)
        {
            mStickStatus = STICK_STATUS.RIGHTDWON;
            // 入力値をログ出力

        }
        //下
        else if (mStickValueDwon == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.DWON;
            // 入力値をログ出力

        }
        //左下
        else if (mStickValueLeft == 1.0f && mStickValueDwon == 1.0f)
        {
            mStickStatus = STICK_STATUS.LEFTDWON;
            // 入力値をログ出力

        }
        //左
        else if (mStickValueLeft == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.LEFT;
            // 入力値をログ出力

        }
        //左上
        else if (mStickValueLeft == 1.0f && mStickValueUp == 1.0f)
        {
            mStickStatus = STICK_STATUS.LEFTUP;
            // 入力値をログ出力

        }
        else
        {
            mStickStatus = STICK_STATUS.NOTINPUT;
        }
        if(mStickStatus != STICK_STATUS.NOTINPUT)
        {
            //Debug.Log($"ステックの状態 : {mStickStatus}");
        }


    }

    //スティック入力状態取得
    public STICK_STATUS GetmStickStatus()
    {
        return mStickStatus;
    }

    public bool GetPlayerControlFlag()
    {
        return mIsControl;
    }

    //回避失敗時のプレイヤースタン
    public void StunPlayer()
    {
        Debug.Log("回避不能");
        mIsControl = true;
    }

}
