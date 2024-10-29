using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullObject : MonoBehaviour
{
    private enum BULL_STATUS
    {
        SONAR,//���G
        SET_ATTACK,//�ˌ�����
        ATTACK,//�ˌ�
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
    }

    // Update is called once per frame
    void Update()
    {
        switch (bullStatus)
        {
            case BULL_STATUS.SONAR:
                TargetSonar();
                break;
            case BULL_STATUS.SET_ATTACK:
                SetAttack();
                break;
            case BULL_STATUS.ATTACK:
                Attack();
                break;
            case BULL_STATUS.EVASION:
                Attack();
                break;
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

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.name == "BULL")
        {
            switch (bullStatus)
            {
                case BULL_STATUS.ATTACK:
                    Debug.Log("ATK");
                    bullStatus = BULL_STATUS.STUN_DAMAGE;
                    break;
                case BULL_STATUS.EVASION:
                    Debug.Log("EVASION");
                    bullStatus = BULL_STATUS.STUN_ATTACK;
                    break;
            }
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
            //Debug.Log("�����_���Q�b�g" + mSelectAttackPoint);
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
        else if (mAttackSet && mNowSetAttackTime <= mSetAttackTime)
        {
            mNowSetAttackTime += Time.deltaTime;
        }
        //�ˌ���������+�ˌ��ڕW�擾
        else if (mAttackSet && mNowSetAttackTime >= mSetAttackTime)
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