using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullObject : MonoBehaviour
{
    private GameObject mPrayer;
    private GameObject mAttackPoints;
    private GameObject[] target;
    private GameObject mSelectAttackPoint;
    private bool mGetAttackPoint;
    private bool mAttackSet;
    private float mSpeed = 3.0f;
    private float mStanTime = 3.0f;
    private float mNowStanTime;



    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowStanTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        TargetSonar();

        if (!mGetAttackPoint) 
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];
            mGetAttackPoint = true;
        }
        else
        {
            if (!mAttackSet && mSelectAttackPoint.transform.position != this.transform.position)
            {
                //�X�^�[�g�ʒu�A�^�[�Q�b�g�̍��W�A���x
                transform.position = Vector3.MoveTowards(
                  transform.position,
                  mSelectAttackPoint.transform.position,
                  mSpeed * Time.deltaTime);
            }
            else if (!mAttackSet && mSelectAttackPoint.transform.position == this.transform.position)
            {
                mAttackSet = true;
                mNowStanTime = 0.0f;
            }
            else if (mAttackSet && mNowStanTime <= mStanTime)
            {
                mNowStanTime += Time.deltaTime;
            }
            else if (mAttackSet && mNowStanTime >= mStanTime)
            {
                mAttackSet = false;
                mGetAttackPoint = false;
            }
        }

        Debug.Log(mSelectAttackPoint);
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

            //Debug.Log(children[i] + ":" + instantDis);


        }

        // 0�`��-1�܂ł̎q�����Ԃɔz��Ɋi�[
        for (var i = 0; i < target.Length; ++i)
        {
            //Debug.Log("target[" + i + "]" + target[i]);
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
 �R�[�h�����GAttack
 �T�v�F�ˌ��̂��߂̏����ړ��Ɠːi
 
�K�v�@�\
����
���E�擾����AttackPoints�̂��������_����1�擾����
���E�����_���Ɏ擾����AttackPoints�Ɉړ�����
���E�ړ���ҋ@����
�E�v���C���[�Ɍ������ēˌ�����(�ʂ�߂���)


 */