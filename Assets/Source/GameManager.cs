using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float mEndTime;
    private float mTime;
    private int mScore;
    [SerializeField] TextMeshProUGUI counter;
    [SerializeField] TextMeshProUGUI scoreText;


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

　　public void ResetTime()
    {
        mTime = 0;
    }

    public void ResetScore()
    {
        mScore = 0;
    }

    public void AddScore(int add)
    {
        mScore += add;
    }

}
