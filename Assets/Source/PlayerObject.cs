using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObject : MonoBehaviour
{
    private enum STICK_STATUS
    {
        NOTINPUT,//入力なし
        UP,//上
        RIGHTUP,//右斜め上
        RIGHT,//右
        RIGHTDWON,//右斜め上
        DWON,//下
        LEFTDWON,//左
        LEFT,//左
        LEFTUP//左
    }


    // Actionをインスペクターから編集できるようにする
    [SerializeField] private InputAction mStickUp;
    [SerializeField] private InputAction mStickRight;
    [SerializeField] private InputAction mStickDwon;
    [SerializeField] private InputAction mStickLeft;


    private STICK_STATUS mStickStatus;
    private float mStickValueUp;
    private float mStickValueRight;
    private float mStickValueDwon;
    private float mStickValueLeft;


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
    }

    private void Update()
    {
        // ステックの入力値を読み込む
        mStickValueUp = mStickUp.ReadValue<float>();
        mStickValueRight = mStickRight.ReadValue<float>();
        mStickValueDwon = mStickDwon.ReadValue<float>();
        mStickValueLeft = mStickLeft.ReadValue<float>();

        StickInput();
    }

    void StickInput()
    {
        //上
        if(mStickValueUp == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.UP;
            // 入力値をログ出力
            Debug.Log($"ステックの状態 : {mStickStatus}");

        }
        //右
        else if (mStickValueRight == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.RIGHT;
            // 入力値をログ出力
            Debug.Log($"ステックの状態 : {mStickStatus}");

        }
        //下
        else if(mStickValueDwon == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.DWON;
            // 入力値をログ出力
            Debug.Log($"ステックの状態 : {mStickStatus}");

        }
        //左
        else if (mStickValueLeft == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.LEFT;
            // 入力値をログ出力
            Debug.Log($"ステックの状態 : {mStickStatus}");

        }
        else
        {
            mStickStatus = STICK_STATUS.NOTINPUT;
        }

    }
}
