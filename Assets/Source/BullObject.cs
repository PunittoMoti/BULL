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
        FAILUREEVASION,//������s
        STUN_ATTACK,//�U�����X�^��
        STUN_DAMAGE //�U�����ꂽ���̃X�^��
    }

    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private GameObject mSelectAttackEndPoint;
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
    private int mStickStatuStackCount;



    private BULL_STATUS bullStatus = BULL_STATUS.SONAR;
    private STICK_STATUS mCheckStickStatus;
    private STICK_STATUS[] mCheckStickStatuStack;


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
        mStickStatuStackCount = 0;
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

                if (!mPrayer.gameObject.GetComponent<PlayerObject>().GetPlayerControlFlag())
                {
                    if (collision.gameObject.name == "OutArea")
                    {
                        //�v���C���[�̃X�^���t���O�L����
                        mPrayer.gameObject.GetComponent<PlayerObject>().StunPlayer();
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
                        bullStatus = BULL_STATUS.FAILUREEVASION;
                    }
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
                        //�X�e�B�b�N���͏�ԏ�����
                        mCheckStickStatuStack = new STICK_STATUS[8];

                        for(int i=0;i< mCheckStickStatuStack.Length; i++)
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

    //���ꂽ�Ƃ�
    void OnTriggerExit(Collider collision)
    {
        /*
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
        */
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
            mSelectAttackEndPoint = instantAttackPointObject.GetAttackEndPoint();

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

        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        switch (mCheckStickStatuStack[0])
        {
            case STICK_STATUS.UP:

                if(mCheckStickStatuStack[1] == STICK_STATUS.DWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2,3] { { STICK_STATUS.RIGHT, STICK_STATUS.DWON, STICK_STATUS.LEFT } , { STICK_STATUS.LEFT, STICK_STATUS.DWON, STICK_STATUS.RIGHT } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[2] == STICK_STATUS.RIGHT)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[2] == STICK_STATUS.LEFT)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHTUP:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP }, { STICK_STATUS.LEFTUP, STICK_STATUS.LEFT, STICK_STATUS.RIGHTDWON } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTDWON)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[2] == STICK_STATUS.LEFTUP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHT:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.DWON, STICK_STATUS.LEFT, STICK_STATUS.UP }, { STICK_STATUS.UP, STICK_STATUS.LEFT, STICK_STATUS.DWON } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[2] == STICK_STATUS.DWON)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[2] == STICK_STATUS.UP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.RIGHTDWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP, STICK_STATUS.RIGHTUP }, { STICK_STATUS.RIGHTUP, STICK_STATUS.LEFTUP, STICK_STATUS.LEFTDWON } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[2] == STICK_STATUS.LEFTDWON)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTUP)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.DWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.UP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }

                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFT, STICK_STATUS.UP, STICK_STATUS.RIGHT }, { STICK_STATUS.RIGHT, STICK_STATUS.UP, STICK_STATUS.LEFT } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[2] == STICK_STATUS.LEFT)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[2] == STICK_STATUS.RIGHT)
                    {
                        ReverseRollEvasion(checkList);
                    }

                }
                break;
            case STICK_STATUS.LEFTDWON:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.LEFTUP, STICK_STATUS.RIGHTUP, STICK_STATUS.RIGHTDWON }, { STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON, STICK_STATUS.LEFTUP } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[2] == STICK_STATUS.LEFTUP)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTDWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }
                break;
            case STICK_STATUS.LEFT:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.UP, STICK_STATUS.RIGHT, STICK_STATUS.DWON }, { STICK_STATUS.DWON, STICK_STATUS.RIGHT, STICK_STATUS.UP } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[2] == STICK_STATUS.UP)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[2] == STICK_STATUS.DWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }


                break;
            case STICK_STATUS.LEFTUP:
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON)
                {
                    SpeedUp();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHTUP, STICK_STATUS.RIGHTDWON, STICK_STATUS.LEFTDWON }, { STICK_STATUS.LEFTDWON, STICK_STATUS.RIGHTDWON, STICK_STATUS.RIGHTUP } };

                    //���v���̉�]����A�N�V�����̏ꍇ
                    if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[2] == STICK_STATUS.RIGHTUP)
                    {
                        RollEvasion(checkList);
                    }
                    //�����v���̉�]����A�N�V�����̏ꍇ
                    else if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[2] == STICK_STATUS.LEFTDWON)
                    {
                        ReverseRollEvasion(checkList);

                    }

                }
                break;


        }
    }

    void StackStickInputStack()
    {
        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        //�ŐV�ɃX�^�b�N����Ă����ԂƈႤ���X�e�B�b�N���͏�Ԃ������łȂ����
        if (playerObject.GetmStickStatus() != mCheckStickStatuStack[mStickStatuStackCount] && playerObject.GetmStickStatus() != STICK_STATUS.NOTINPUT)
        {
            //�X�^�b�N�J�E���g�㏸
            mStickStatuStackCount += 1;
            //���ɓ��͂��ꂽ��Ԃ��擾
            mCheckStickStatuStack[mStickStatuStackCount] = playerObject.GetmStickStatus();
            Debug.Log("�X�^�b�N[" + mStickStatuStackCount + "]:" + mCheckStickStatuStack[mStickStatuStackCount]);
        }
    }

    void RollEvasion(STICK_STATUS[,] checkList)
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
            Debug.Log("��]��𐬌�:"+ mSelectAttackPoint);

            //�i�s�����ύX
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackEndPoint = instantAttackPointObject.GetRollEvasionEndPoint();
            //�X�s�[�h�A�b�v
            SpeedUp();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    void ReverseRollEvasion(STICK_STATUS[,] checkList)
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
            Debug.Log("��]��𐬌�:" + mSelectAttackPoint);

            //�i�s�����ύX
            AttackPointObject instantAttackPointObject;
            instantAttackPointObject = mSelectAttackPoint.GetComponent<AttackPointObject>();
            mSelectAttackEndPoint = instantAttackPointObject.GetReverseRollEvasionAttackEndPoint();
            //�X�s�[�h�A�b�v
            SpeedUp();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            bullStatus = BULL_STATUS.FAILUREEVASION;
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