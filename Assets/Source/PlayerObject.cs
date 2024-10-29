using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerObject : MonoBehaviour
{
    private enum STICK_STATUS
    {
        NOTINPUT,//���͂Ȃ�
        UP,//��
        RIGHTUP,//�E�΂ߏ�
        RIGHT,//�E
        RIGHTDWON,//�E�΂ߏ�
        DWON,//��
        LEFTDWON,//��
        LEFT,//��
        LEFTUP//��
    }


    // Action���C���X�y�N�^�[����ҏW�ł���悤�ɂ���
    [SerializeField] private InputAction mStickUp;
    [SerializeField] private InputAction mStickRight;
    [SerializeField] private InputAction mStickDwon;
    [SerializeField] private InputAction mStickLeft;


    private STICK_STATUS mStickStatus;
    private float mStickValueUp;
    private float mStickValueRight;
    private float mStickValueDwon;
    private float mStickValueLeft;


    // �L����
    private void OnEnable()
    {
        // InputAction��L����
        // ��������Ȃ��Ɠ��͂��󂯎��Ȃ����Ƃɒ���
        mStickUp?.Enable();
        mStickRight?.Enable();
        mStickDwon?.Enable();
        mStickLeft?.Enable();
    }

    // ������
    private void OnDisable()
    {
        // ���g�������������^�C�~���O�Ȃǂ�
        // Action�𖳌�������K�v������
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
        // �X�e�b�N�̓��͒l��ǂݍ���
        mStickValueUp = mStickUp.ReadValue<float>();
        mStickValueRight = mStickRight.ReadValue<float>();
        mStickValueDwon = mStickDwon.ReadValue<float>();
        mStickValueLeft = mStickLeft.ReadValue<float>();

        StickInput();
    }

    void StickInput()
    {
        //��
        if(mStickValueUp == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.UP;
            // ���͒l�����O�o��
            Debug.Log($"�X�e�b�N�̏�� : {mStickStatus}");

        }
        //�E
        else if (mStickValueRight == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.RIGHT;
            // ���͒l�����O�o��
            Debug.Log($"�X�e�b�N�̏�� : {mStickStatus}");

        }
        //��
        else if(mStickValueDwon == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.DWON;
            // ���͒l�����O�o��
            Debug.Log($"�X�e�b�N�̏�� : {mStickStatus}");

        }
        //��
        else if (mStickValueLeft == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.LEFT;
            // ���͒l�����O�o��
            Debug.Log($"�X�e�b�N�̏�� : {mStickStatus}");

        }
        else
        {
            mStickStatus = STICK_STATUS.NOTINPUT;
        }

    }
}
