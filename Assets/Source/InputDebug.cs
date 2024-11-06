using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SValue;

public class InputDebug : MonoBehaviour
{
    STICK_STATUS mInputStatus;
    GameObject mPoint;
    // Start is called before the first frame update
    void Start()
    {
        mPoint = transform.Find("InputDebugPoint").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        mInputStatus = this.gameObject.GetComponent<PlayerObject>().GetmStickStatus();

        switch (mInputStatus)
        {
            case STICK_STATUS.NOTINPUT:
                mPoint.gameObject.transform.position = new Vector3(0, 0, 0);
                break;
            case STICK_STATUS.UP:
                mPoint.gameObject.transform.position = new Vector3(0, 0, 2f);
                break;
            case STICK_STATUS.RIGHTUP:
                mPoint.gameObject.transform.position = new Vector3(1.5f, 0, 1.5f);
                break;
            case STICK_STATUS.RIGHT:
                mPoint.gameObject.transform.position = new Vector3(2f, 0, 0);
                break;
            case STICK_STATUS.RIGHTDWON:
                mPoint.gameObject.transform.position = new Vector3(1.5f, 0, -1.5f);
                break;
            case STICK_STATUS.DWON:
                mPoint.gameObject.transform.position = new Vector3(0, 0, -2f);
                break;
            case STICK_STATUS.LEFTDWON:
                mPoint.gameObject.transform.position = new Vector3(-1.5f, 0, -1.5f);
                break;
            case STICK_STATUS.LEFT:
                mPoint.gameObject.transform.position = new Vector3(-2f, 0, 0f);
                break;
            case STICK_STATUS.LEFTUP:
                mPoint.gameObject.transform.position = new Vector3(-1.5f, 0, 1.5f);
                break;
        }
    }
}
