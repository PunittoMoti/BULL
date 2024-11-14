using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;


public class CBull : MonoBehaviour
{
    public enum BULL_STATUS
    {
        SONAR,//���G
        SET_ATTACK,//�ˌ�����
        ATTACK,//�ˌ�
        CHECK_EVASION,//�����̓`�F�b�N
        CHECK_JUSTEVASION,//�M���M�������̓`�F�b�N
        EVASION,//�����ˌ�
        FAILUREEVASION,//������s
        STUN_ATTACK,//�U�����X�^��
        STUN_DAMAGE //�U�����ꂽ���̃X�^��
    }

    //���ʕϐ�(�q�N���X�ł��g�p)
    protected GameObject mPrayer;//�v���C���[
    protected GameObject mGameManager;//�Q�[���}�l�[�W���[
    protected GameObject mAttackPoints;//�S�Ă̓ˌ��ʒu
    protected GameObject[] target;//�ˌ��ʒu���
    protected GameObject mSelectAttackPoint;//�I�����ꂽ�ˌ��ʒu
    protected GameObject mSelectAttackEndPoint;//�ˌ��I���ʒu



    protected bool mGetAttackPoint;
    protected bool mAttackSet;
    protected bool mCheckEvasion;

    protected float mSpeed;
    protected float mSpeedOrigin = 3.0f;
    protected float mSpeedMagnification = 1.0f;
    protected float mSetAttackTime = 3.0f;
    protected float mNowSetAttackTime;
    protected float mAttackStunTime = 5.0f;
    protected float mDamageStunTime = 3.0f;
    protected float mNowStunTime;

    protected float[] mSpeedList = new float[] { 1, 1.1f, 1.3f, 1.6f, 2.0f };
    protected int mSpeedListCount;
    protected int mStickStatuStackCount;


    protected BULL_STATUS bullStatus = BULL_STATUS.SONAR;
    protected STICK_STATUS mCheckStickStatus;
    protected STICK_STATUS[] mCheckStickStatuStack;






    // Start is called before the first frame update
    void Start()
    {
        mPrayer = GameObject.Find("Player");
        mGameManager = GameObject.Find("GameManager");
        target = new GameObject[3];
        mGetAttackPoint = false;
        mAttackSet = false;
        mNowSetAttackTime = 0.0f;
        mSpeedListCount = 0;
        mSpeed = mSpeedOrigin;
        mCheckEvasion = false;
        mStickStatuStackCount = 0;
    }

    //�ˌ��ʒu���G����
    protected void TargetSonar()
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

    //�ˌ�����
    protected void SetAttack()
    {
        if (!mGetAttackPoint)
        {
            mSelectAttackPoint = target[Random.Range(1, 3)];

            if (mSelectAttackPoint.GetComponent<AttackPointObject>().GetIsUse())
            {
                //��ԑJ��
                bullStatus = BULL_STATUS.SONAR;
                return;
            }

            //����Point�g�p���ɕύX
            mSelectAttackPoint.GetComponent<AttackPointObject>().IsUseNo();

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

            //����Point�g�p���ɕύX
            mSelectAttackPoint.GetComponent<AttackPointObject>().IsUseOFF();

            //��ԑJ��
            bullStatus = BULL_STATUS.ATTACK;
        }

    }

    protected void SpeedUp()
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

    protected void SpeedDown()
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

    protected void EvasionCheck()
    {
        if (!mCheckEvasion) return;

        PlayerObject playerObject = mPrayer.gameObject.GetComponent<PlayerObject>();

        switch (mCheckStickStatuStack[0])
        {
            case STICK_STATUS.UP:

                if (mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
                    bullStatus = BULL_STATUS.EVASION;
                }
                else
                {
                    STICK_STATUS[,] checkList = new STICK_STATUS[2, 3] { { STICK_STATUS.RIGHT, STICK_STATUS.DWON, STICK_STATUS.LEFT }, { STICK_STATUS.LEFT, STICK_STATUS.DWON, STICK_STATUS.RIGHT } };

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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON || mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[1] == STICK_STATUS.DWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFT || mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[1] == STICK_STATUS.LEFTDWON)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP || mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[1] == STICK_STATUS.LEFT)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.UP || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[1] == STICK_STATUS.LEFTUP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP || mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[1] == STICK_STATUS.UP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHT || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHTUP)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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
                if (mCheckStickStatuStack[1] == STICK_STATUS.RIGHTDWON || mCheckStickStatuStack[1] == STICK_STATUS.DWON || mCheckStickStatuStack[1] == STICK_STATUS.RIGHT)
                {
                    mGameManager.GetComponent<GameManager>().AddScoreNormal();
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

    protected void StackStickInputStack()
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

    protected void RollEvasion(STICK_STATUS[,] checkList)
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
            mSelectAttackEndPoint = instantAttackPointObject.GetRollEvasionEndPoint();
            //�X�s�[�h�A�b�v
            SpeedUp();
            mGameManager.GetComponent<GameManager>().AddScoreRoll();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            SpeedDown();
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    protected void ReverseRollEvasion(STICK_STATUS[,] checkList)
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
            mGameManager.GetComponent<GameManager>().AddScoreRoll();
            bullStatus = BULL_STATUS.EVASION;
        }

        if (count >= 2)
        {
            SpeedDown();
            bullStatus = BULL_STATUS.FAILUREEVASION;
        }
    }

    public BULL_STATUS GetBullStatus()
    {
        return bullStatus;
    }


}
