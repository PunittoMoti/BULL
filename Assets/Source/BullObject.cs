using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class BullObject : MonoBehaviour
{
    private enum BULL_STATUS
    {
        SONAR,//���G
        SET_ATTACK,//�ˌ�����
        ATTACK,//�ˌ�
        CHECK_EVASION,//�����̓`�F�b�N
        EVASION,//�����ˌ�
        STUN_ATTACK,//�U�����X�^��
        STUN_DAMAGE //�U�����ꂽ���̃X�^��
    }

    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private bool mGetAttackPoint;
    private bool mAttackSet;
    private bool mCheckEvasion;

    private float mSpeed;
    private float mSpeedOrigin = 3.0f;
    private float mSpeedMagnification =1.0f;
    private float mSetAttackTime = 3.0f;
    private float mNowSetAttackTime;
    private float mAttackStunTime = 5.0f;
    private float mDamageStunTime = 3.0f;
    private float mNowStunTime;

    private float[] mSpeedList = new float[] { 1, 1.1f, 1.3f, 1.6f, 2.0f };
    private int mSpeedListCount;


    private BULL_STATUS bullStatus = BULL_STATUS.SONAR;
    private STICK_STATUS mCheckStickStatus;

    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowSetAttackTime = 0.0f;
        mSpeedListCount = 0;
        mSpeed = mSpeedOrigin;
        mCheckEvasion = false;
    }

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
                EvasionCheck();
                Attack();
                break;
            //�����
            case BULL_STATUS.EVASION:
                transform.Find("Model").gameObject.GetComponent<Renderer>().material.color = Color.red;
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
                    Debug.Log("ATK");
                    bullStatus = BULL_STATUS.STUN_DAMAGE;

                }
                break;
            case BULL_STATUS.EVASION:
                if (collision.gameObject.name == "BULL")
                {
                    Debug.Log("EVASION");
                    bullStatus = BULL_STATUS.STUN_ATTACK;

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
                        mCheckEvasion = true;
                    }
                }
                break;
        }
    }

    //���ꂽ�Ƃ�
    void OnTriggerExit(Collider collision)
    {
        switch (bullStatus)
        {
            case BULL_STATUS.CHECK_EVASION:
                if (collision.gameObject.name == "Player")
                {
                    mCheckEvasion = false;
                    bullStatus = BULL_STATUS.ATTACK;
                }
                break;
        }
    }


    void TargetSonar()
    {
        /* �^�[�Q�b�g�̃|�W�V�������擾 */
        Vector3 targetPos = this.transform.position;

        /* �v���C���[�̃|�W�V�������擾 */
        Vector3 playerPos = mPrayer.transform.position;

        /* �^�[�Q�b�g�ƃv���C���[�̋������擾 */
        float dis = Vector3.Distance(targetPos, playerPos);

        //Debug.Log("����:" + dis);

        mAttackPoints = mPrayer.transform.Find("AttackPoins").gameObject;

        // �q�I�u�W�F�N�g���i�[����z��쐬
        var children = new GameObject[mAttackPoints.transform.childCount];


        // 0�`��-1�܂ł̎q�����Ԃɔz��Ɋi�[
        for (var i = 0; i < children.Length; ++i)
        {
            children[i] = mAttackPoints.transform.GetChild(i).gameObject;
        }

        // 0�`��-1�܂ł̎q�����Ԃɔz��Ɋi�[
        for (var i = 0; i < children.Length; ++i)
        {
            float instantDis = Vector3.Distance(targetPos, children[i].transform.position);

            //�m�F��0�łȂ����
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

        //��ԑJ��
        bullStatus = BULL_STATUS.SET_ATTACK;
    }

    void SetAttack()
    {
        if (!mGetAttackPoint)
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];
            mGetAttackPoint = true;
            //�Ή���������͂��擾
            mCheckStickStatus = mSelectAttackPoint.GetComponent<AttackPointObject>().GetAnswerStickStatusNormal();

            Debug.Log("�Ή������́F" + mCheckStickStatus);
        }
        // �ˌ������ʒu�܂ňړ�����
        else if (!mAttackSet && mSelectAttackPoint.transform.position != this.transform.position)
        {
            //�X�^�[�g�ʒu�A�^�[�Q�b�g�̍��W�A���x
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //�ˌ������ʒu�ɓ��������ꍇ�A�������n�߂�
        else if (!mAttackSet && mSelectAttackPoint.transform.position == this.transform.position)
        {
            mAttackSet = true;
            mNowSetAttackTime = 0.0f;
        }
        //�ˌ������ʒu�ŏ������ԕ��ҋ@����
        else if (mAttackSet && mNowSetAttackTime <= mSetAttackTime / mSpeedMagnification)
        {
            mNowSetAttackTime += Time.deltaTime;
        }
        //�ˌ���������+�ˌ��ڕW�擾
        else if (mAttackSet && mNowSetAttackTime >= mSetAttackTime / mSpeedMagnification)
        {
            //�ړ���Object���擾
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackPoint = instantAttackPointObject.GetAttackEndPoint();

            //�t���O������
            mGetAttackPoint = false;
            mAttackSet = false;

            //��ԑJ��
            bullStatus = BULL_STATUS.ATTACK;
        }

    }

    void Attack()
    {
        //�ˌ���
        if (mSelectAttackPoint.transform.position != this.transform.position)
        {
            //�X�^�[�g�ʒu�A�^�[�Q�b�g�̍��W�A���x
            transform.position = Vector3.MoveTowards(
              transform.position,
              mSelectAttackPoint.transform.position,
              mSpeed * Time.deltaTime);
        }
        //�ˌ�����
        else if (mSelectAttackPoint.transform.position == this.transform.position)
        {
            mAttackSet = false;
            mGetAttackPoint = false;
            //Debug.Log("�ʊ���" + mSelectAttackPoint);

            //��ԑJ��
            bullStatus = BULL_STATUS.SONAR;
        }

    }

    void SpeedUp()
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

    void SpeedDown()
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

    void EvasionCheck()
    {
        if (!mCheckEvasion) return;

        switch (mCheckStickStatus)
        {
            case STICK_STATUS.UP:
                if(mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.DWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.RIGHTUP:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.LEFTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.RIGHT:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.LEFT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.RIGHTDWON:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.LEFTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.DWON:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.UP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.LEFTDWON:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.RIGHTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.LEFT:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.RIGHT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;
            case STICK_STATUS.LEFTUP:
                if (mPrayer.gameObject.GetComponent<PlayerObject>().GetmStickStatus() == STICK_STATUS.RIGHTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                break;


        }
    }


}

/*
 ���G����	
��	���G�������
	���G��ԕ��A����
�ːi����	
	�ˌ���������
	���i�ˌ�����
�X�^������	
	�����̏Փ˂��������X�^������
	�Փ˂��ꂽ�����X�^������
���x�ω�����	
	��]������̃X�s�[�h�A�b�v
	������s���̃X�s�[�h�_�E��
 */

/*
�R�[�h�����FTargetSonar
�T�v�F���G�@�\�̃e�X�g�R�[�h

�K�v�@�\
����
���E�v���C���[�Ƃ̋������擾����
���E�v���C���[/NPC��AttackPoints�I�u�W�F�N�g���擾����
���EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�̋������擾����
���EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�̋������r����
���EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�ŋ߂����̂�3�擾����

�}�X�^�[��
�ENPC�Ƃ̋������擾����
�E�v���C���[��NPC�̋������r����

 */

/*
 �R�[�h�����GSetAttack/Attack
 �T�v�F�ˌ��̂��߂̏����ړ��Ɠːi
 
�K�v�@�\
����
���E�擾����AttackPoints�̂��������_����1�擾����
���E�����_���Ɏ擾����AttackPoints�Ɉړ�����
���E�ړ���ҋ@����
���E�ˌ��I�_���擾����
���E�ˌ��I�_�Ɍ������ēˌ�����
 */

/*
  �R�[�h�����GSelectStun
 �T�v�F�ˌ��̂��߂̏����ړ��Ɠːi
 
�K�v�@�\
����
�E�ǂ���ɏ[�Ă�ꂽ���m�F
�E�ҋ@����
 */