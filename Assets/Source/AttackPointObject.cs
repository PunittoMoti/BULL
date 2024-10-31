using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class AttackPointObject : MonoBehaviour
{
    [SerializeField] private GameObject mAttackEndPoint;
    [SerializeField] private STICK_STATUS mAnswerStickStatusNormal;

    public GameObject GetAttackEndPoint()
    {
        return mAttackEndPoint;
    }

    public STICK_STATUS GetAnswerStickStatusNormal()
    {
        return mAnswerStickStatusNormal;
    }
}
