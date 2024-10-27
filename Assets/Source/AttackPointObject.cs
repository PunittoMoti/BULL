using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointObject : MonoBehaviour
{
    [SerializeField] private GameObject mAttackEndPoint; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetAttackEndPoint()
    {
        return mAttackEndPoint;
    }
}
