using System;
using UnityEngine;
 
public class DistanceController : MonoBehaviour
{

    [SerializeField] private GameObject player;
    [SerializeField] private TextMesh counter;

    void Start()
    {

    }

    void Update()
    {
        /* ターゲットのポジションを取得 */
        Vector3 targetPos = this.transform.position;

        /* プレイヤーのポジションを取得 */
        Vector3 playerPos = player.transform.position;

        /* ターゲットとプレイヤーの距離を取得 */
        float dis = Vector3.Distance(targetPos, playerPos);

        Debug.Log("距離:" + dis);
    }
}