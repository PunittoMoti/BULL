using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float mEndTime;
    private float mTime;
    [SerializeField] TextMeshProUGUI counter;

    // Start is called before the first frame update
    void Start()
    {
        mTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(mTime> mEndTime)
        {
            SceneManager.LoadScene("Result");
        }
        else
        {
            mTime += Time.deltaTime;
            Debug.Log("確認　" + counter);
            // 小数2桁にして表示
            counter.text = mTime.ToString("F2");
        }
    }

　　public void ResetTime()
    {
        mTime = 0;
    }
}
