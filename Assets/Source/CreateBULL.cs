using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBULL : MonoBehaviour
{
    private enum BULLTYPE
    {
        NORMAL
    }

    [SerializeField] private BULLTYPE[] mBulls;
    [SerializeField] private float[] mSpawnTimes;

    private GameObject mGameManager;

    int mSpawnCount;


    // Use this for initialization
    void Start()
    {
        mSpawnCount = 0;
        mGameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (mBulls.Length <= mSpawnCount) return;

        float time = mGameManager.GetComponent<GameManager>().GetTime();

        if (time >= mSpawnTimes[mSpawnCount])
        {
            SpawnBull(mBulls[mSpawnCount]);
            mSpawnCount += 1;
        }
    }

    //��������
    void SpawnBull(BULLTYPE bullType)
    {
        switch (bullType)
        {
            case BULLTYPE.NORMAL:
                // Cube�v���n�u��GameObject�^�Ŏ擾
                GameObject obj = (GameObject)Resources.Load("BULL");
                // Cube�v���n�u�����ɁA�C���X�^���X�𐶐��A
                Instantiate(obj, this.gameObject.transform.position, Quaternion.identity);
                break;
        }
    }

}

//�^�C���擾
//�^�C�����Ƃɐ���
