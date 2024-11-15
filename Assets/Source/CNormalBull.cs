using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;


public class CNormalBull : CBull
{
    // Update is called once per frame
    void Update()
    {
        switch (bullStatus)
        {
            //���G
            case BULL_STATUS.SONAR:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.white;
                TargetSonar();
                break;
            //�ˌ�����
            case BULL_STATUS.SET_ATTACK:
                SetAttack();
                break;
            //�ˌ�
            case BULL_STATUS.ATTACK:
                Attack();
                break;
            //��𔻒�`�F�b�N
            case BULL_STATUS.CHECK_EVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.red;
                StackStickInputStack();
                Attack();
                EvasionCheck();
                break;
            //��𔻒�`�F�b�N
            case BULL_STATUS.CHECK_JUSTEVASION:
                Debug.Log("�M���M��");
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.gray;
                StackStickInputStack();
                Attack();
                EvasionCheck();
                break;
            //�����
            case BULL_STATUS.EVASION:
                Attack();
                break;
            //������s
            case BULL_STATUS.FAILUREEVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.blue;
                Attack();
                break;

            //�Ԃ���ꂽ���̃X�^��
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
            //�Ԃ��������̃X�^��
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

    //���������u��
    void OnTriggerEnter(Collider collision)
    {
        switch (bullStatus)
        {
            case BULL_STATUS.ATTACK:
                if (collision.gameObject.name == "BULL")
                {
                    if (collision.gameObject.GetComponent<CBull>().GetBullStatus() == BULL_STATUS.EVASION)
                    {
                        bullStatus = BULL_STATUS.STUN_DAMAGE;
                    }

                }

                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //�v���C���[�̃X�^���t���O�L����
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;
            case BULL_STATUS.CHECK_EVASION:
                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //�v���C���[�̃X�^���t���O�L����
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;
            case BULL_STATUS.CHECK_JUSTEVASION:
                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //�v���C���[�̃X�^���t���O�L����
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
                        SpeedDown();
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
                }
                break;

            case BULL_STATUS.EVASION:
                if (collision.gameObject.name == "BULL")
                {
                    if (collision.gameObject.GetComponent<CBull>().GetBullStatus() == BULL_STATUS.ATTACK)
                    {
                        bullStatus = BULL_STATUS.STUN_ATTACK;
                    }

                }
                break;
        }




    }

    //�����葱���Ă����
    void OnTriggerStay(Collider collision)
    {
        switch (bullStatus)
        {
            case BULL_STATUS.ATTACK:
                if (collision.gameObject.name == "Player")
                {
                    //�v���C���[�̓��͂�BULL�������Ă����t���͂ƈ�v���Ă����
                    if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == mCheckStickStatus)
                    {
                        bullStatus = BULL_STATUS.CHECK_EVASION;
                        //�X�e�B�b�N���͏�ԏ�����
                        mCheckStickStatuStack = new STICK_STATUS[8];

                        for (int i = 0; i < mCheckStickStatuStack.Length; i++)
                        {
                            mCheckStickStatuStack[i] = STICK_STATUS.NOTINPUT;
                        }

                        mCheckStickStatuStack[0] = mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus();
                        mStickStatuStackCount = 0;
                        Debug.Log("�X�^�b�N[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
                        mCheckEvasion = true;
                    }
                }

                if (collision.gameObject.name == "JustArea")
                {
                    //�v���C���[�̓��͂�BULL�������Ă����t���͂ƈ�v���Ă����
                    if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == mCheckStickStatus)
                    {
                        bullStatus = BULL_STATUS.CHECK_JUSTEVASION;
                        //�X�e�B�b�N���͏�ԏ�����
                        mCheckStickStatuStack = new STICK_STATUS[8];

                        for (int i = 0; i < mCheckStickStatuStack.Length; i++)
                        {
                            mCheckStickStatuStack[i] = STICK_STATUS.NOTINPUT;
                        }

                        mCheckStickStatuStack[0] = mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus();
                        mStickStatuStackCount = 0;
                        Debug.Log("�X�^�b�N[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
                        mCheckEvasion = true;
                    }

                }
                break;
        }
    }


    void Attack()
    {
        //�ˌ���
        if (mSelectAttackEndPoint.transform.position != this.transform.position)
        {
            //�X�^�[�g�ʒu�A�^�[�Q�b�g�̍��W�A���x
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackEndPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //�ˌ�����
        else if (mSelectAttackEndPoint.transform.position == this.transform.position)
        {
            mAttackSet = false;
            mGetAttackPoint = false;
            //Debug.Log("�ʊ���" + mSelectAttackPoint);

            //��ԑJ��
            bullStatus = BULL_STATUS.SONAR;
        }

    }

}
