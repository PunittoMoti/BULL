using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static SValue;


public class CPlayer : MonoBehaviour, IMatador
{
    // ���͎�t�ݒ�
    [SerializeField] private InputAction mStickUp;
    [SerializeField] private InputAction mStickRight;
    [SerializeField] private InputAction mStickDwon;
    [SerializeField] private InputAction mStickLeft;

    //�X�e�b�N���͗p�ϐ�
    private STICK_STATUS mStickStatus;//�X�e�b�N���͂̏��
    private float mStickValueUp;//�X�e�b�N���͒l�F��
    private float mStickValueRight;//�X�e�b�N���͒l�F�E
    private float mStickValueDwon;//�X�e�b�N���͒l�F��
    private float mStickValueLeft;//�X�e�b�N���͒l�F��

    //�X�^���p�ϐ�
    private bool mIsStun;//�X�^���t���O
    [SerializeField] private float mMaxStunTime;//�X�^���ő厞��
    private float mNowStunTime;//�X�^���o�ߎ���


    // ���̗͂L����
    private void OnEnable()
    {
        // InputAction��L����
        // ��������Ȃ��Ɠ��͂��󂯎��Ȃ����Ƃɒ���
        mStickUp?.Enable();
        mStickRight?.Enable();
        mStickDwon?.Enable();
        mStickLeft?.Enable();
    }

    // ���̖͂�����
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
        mIsStun = false;
        mNowStunTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mIsStun)
        {
            // �X�e�b�N�̓��͒l��ǂݍ���
            mStickValueUp = mStickUp.ReadValue<float>();
            mStickValueRight = mStickRight.ReadValue<float>();
            mStickValueDwon = mStickDwon.ReadValue<float>();
            mStickValueLeft = mStickLeft.ReadValue<float>();

            //�X�e�b�N���͔���
            StickInput();
        }
        else
        {
            //�X�^���I����
            if (mNowStunTime >= mMaxStunTime)
            {
                Debug.Log("���G�I��");
                mNowStunTime = 0.0f;
                mIsStun = false;
            }
            //�X�^����
            else
            {
                mNowStunTime += Time.deltaTime;
            }
        }

    }

    //�X�e�b�N���͔���
    void StickInput()
    {
        //��
        if (mStickValueUp == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.UP;
            // ���͒l�����O�o��

        }
        //�E��
        else if (mStickValueRight == 1.0f && mStickValueUp == 1.0f)
        {
            mStickStatus = STICK_STATUS.RIGHTUP;
            // ���͒l�����O�o��

        }
        //�E
        else if (mStickValueRight == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.RIGHT;
            // ���͒l�����O�o��

        }
        //�E��
        else if (mStickValueRight == 1.0f && mStickValueDwon == 1.0f)
        {
            mStickStatus = STICK_STATUS.RIGHTDWON;
            // ���͒l�����O�o��

        }
        //��
        else if (mStickValueDwon == 1.0f && mStickValueRight == 0.0f && mStickValueLeft == 0.0f)
        {
            mStickStatus = STICK_STATUS.DWON;
            // ���͒l�����O�o��

        }
        //����
        else if (mStickValueLeft == 1.0f && mStickValueDwon == 1.0f)
        {
            mStickStatus = STICK_STATUS.LEFTDWON;
            // ���͒l�����O�o��

        }
        //��
        else if (mStickValueLeft == 1.0f && mStickValueUp == 0.0f && mStickValueDwon == 0.0f)
        {
            mStickStatus = STICK_STATUS.LEFT;
            // ���͒l�����O�o��

        }
        //����
        else if (mStickValueLeft == 1.0f && mStickValueUp == 1.0f)
        {
            mStickStatus = STICK_STATUS.LEFTUP;
            // ���͒l�����O�o��

        }
        else
        {
            mStickStatus = STICK_STATUS.NOTINPUT;
        }
        if (mStickStatus != STICK_STATUS.NOTINPUT)
        {
            //Debug.Log($"�X�e�b�N�̏�� : {mStickStatus}");
        }

    }

    //�X�^����Ԏ擾
    public bool GetPlayerStunFlag()
    {
        return mIsStun;
    }

    //�X�^���J�n��
    public void StartStun()
    {
        //�X�R�A����


        //�X�^���t���O�I��
        mIsStun = true;
    }
}
