using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BullAttack : MonoBehaviour
{
    [SerializeField] GameObject target;
    private float speed = 3.0f;

    void Update()
    {
        //スタート位置、ターゲットの座標、速度
        transform.position = Vector3.MoveTowards(
          transform.position,
          target.transform.position,
          speed * Time.deltaTime);
    }
}
