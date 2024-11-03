using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class AttackPointObject : MonoBehaviour
{
    [SerializeField] private GameObject mAttackEndPoint;
    [SerializeField] private GameObject mRollEvasionEndPoint;//回転回避のエンドポイント（時計回り）
    [SerializeField] private GameObject mReverseRollEvasionAttackEndPoint;//回転回避のエンドポイント（反時計回り）
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
