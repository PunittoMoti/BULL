using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSonar : MonoBehaviour
{
    private GameObject mPrayer;
    private GameObject mAttackPoints;




    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
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
        var  target = new GameObject[3];

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
            if (i != 0) {
                float targetDis = Vector3.Distance(targetPos, target[0].transform.position);
                
                if(targetDis >= instantDis) {
                    target[2] = target[1];
                    target[1] = target[0];
                    target[0] = children[i];
                }
                else
                {
                    if (target[1] != null) {
                        targetDis = Vector3.Distance(targetPos, target[1].transform.position);
                        if (targetDis >= instantDis) {
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
            else{
                target[0] = mAttackPoints.transform.GetChild(i).gameObject;
            }

            Debug.Log(children[i]+":"+ instantDis);

            
        }

        // 0�`��-1�܂ł̎q�����Ԃɔz��Ɋi�[
        for (var i = 0; i < target.Length; ++i) 
        {
            Debug.Log("target["+i+"]"+target[i]);
        }
    }
}


/*
�R�[�h����
�T�v�F���G�@�\�̃e�X�g�R�[�h

�K�v�@�\
����
���E�v���C���[�Ƃ̋������擾����
���E�v���C���[/NPC��AttackPoints�I�u�W�F�N�g���擾����
���EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�̋������擾����
�EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�̋������r����
�EAttackPoints�ȉ��ɂ���I�u�W�F�N�g�ŋ߂����̂�3�擾����

�}�X�^�[��
�ENPC�Ƃ̋������擾����
�E�v���C���[��NPC�̋������r����

 */