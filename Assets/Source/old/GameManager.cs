using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float mEndTime;
    private float mTime;
    public static int mScore;
    [SerializeField] TextMeshProUGUI counter;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] private int mNormalScore;
    [SerializeField] private int mRollScore;
    [SerializeField] private int mReduceScore;


    // Start is called before the first frame update
    void Start()
    {
        mTime = 0;
        mScore = 0;
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
            scoreText.text = mScore.ToString();
        }
    }

    public float GetTime()
    {
        return mTime;
    }


    public void ResetTime()
    {
        mTime = 0;
    }

    public void ResetScore()
    {
        mScore = 0;
    }

    public void AddScoreNormal()
    {
        mScore += mNormalScore;

        if (mScore >= 99999999)
        {
            mScore = 99999999;
        }

    }

    public void AddScoreRoll()
    {
        mScore += mRollScore;

        if (mScore >= 99999999)
        {
            mScore = 99999999;
        }

    }

    public void ReduceScoreNormal()
    {
        mScore -= mNormalScore;

        if (mScore <= 0)
        {
            mScore = 0;
        }

    }

}
