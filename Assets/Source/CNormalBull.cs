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
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.red;
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
