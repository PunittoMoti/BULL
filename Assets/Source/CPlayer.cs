using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static SValue;


public class CPlayer : MonoBehaviour, IMatador
{
    // 入力受付設定
    [SerializeField] private InputAction mStickUp;
    [SerializeField] private InputAction mStickRight;
    [SerializeField] private InputAction mStickDwon;
    [SerializeField] private InputAction mStickLeft;

    //ステック入力用変数
    private STICK_STATUS mStickStatus;//ステック入力の状態
    private float mStickValueUp;//ステック入力値：上
    private float mStickValueRight;//ステック入力値：右
    private float mStickValueDwon;//ステック入力値：下
    private float mStickValueLeft;//ステック入力値：左

    //スタン用変数
    private bool mIsStun;//スタンフラグ
    [SerializeField] private float mMaxStunTime;//スタン最大時間
    private float mNowStunTime;//スタン経過時間


    // 入力の有効化
    private void OnEnable()
    {
        // InputActionを有効化
        // これをしないと入力を受け取れないことに注意
        mStickUp?.Enable();
        mStickRight?.Enable();
        mStickDwon?.Enable();
        mStickLeft?.Enable();
    }

    // 入力の無効化
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
        mIsStun = false;
        mNowStunTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mIsStun)
        {
            // ステックの入力値を読み込む
            mStickValueUp = mStickUp.ReadValue<float>();
            mStickValueRight = mStickRight.ReadValue<float>();
            mStickValueDwon = mStickDwon.ReadValue<float>();
            mStickValueLeft = mStickLeft.ReadValue<float>();

            //ステック入力判定
            StickInput();
        }
        else
        {
            //スタン終了時
            if (mNowStunTime >= mMaxStunTime)
            {
                Debug.Log("無敵終了");
                mNowStunTime = 0.0f;
                mIsStun = false;
            }
            //スタン中
            else
            {
                mNowStunTime += Time.deltaTime;
            }
        }

    }

    //ステック入力判定
    void StickInput()
    {
        //上
        if (mStickValueUp == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
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
        if (mStickStatus != STICK_STATUS.NOTINPUT)
        {
            //Debug.Log($"ステックの状態 : {mStickStatus}");
        }

    }

    //スタン状態取得
    public bool GetPlayerStunFlag()
    {
        return mIsStun;
    }

    //スタン開始時
    public void StartStun()
    {
        //スコア減少


        //スタンフラグオン
        mIsStun = true;
    }
}
