using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ResultGameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;


    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = GameManager.mScore.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
