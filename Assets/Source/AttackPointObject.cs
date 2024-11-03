using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class AttackPointObject : MonoBehaviour
{
    [SerializeField] private GameObject mAttackEndPoint;
    [SerializeField] private GameObject mRollEvasionEndPoint;//��]����̃G���h�|�C���g�i���v���j
    [SerializeField] private GameObject mReverseRollEvasionAttackEndPoint;//��]����̃G���h�|�C���g�i�����v���j
    [SerializeField] private STICK_STATUS mAnswerStickStatusNormal;

    public GameObject GetAttackEndPoint()
    {
        return mAttackEndPoint;
    }

    public GameObject GetRollEvasionEndPoint()
    {
        return mRollEvasionEndPoint;
    }

    public GameObject GetReverseRollEvasionAttackEndPoint()
    {
        return mReverseRollEvasionAttackEndPoint;
    }


    public STICK_STATUS GetAnswerStickStatusNormal()
    {
        return mAnswerStickStatusNormal;
    }
}
