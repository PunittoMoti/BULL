using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;


public class ResultGameManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] private InputAction mReturnButton;

    // 有効化
    private void OnEnable()
    {
        // InputActionを有効化
        // これをしないと入力を受け取れないことに注意
        mReturnButton?.Enable();
    }

    // 無効化
    private void OnDisable()
    {
        // 自身が無効化されるタイミングなどで
        // Actionを無効化する必要がある
        mReturnButton?.Disable();
    }


    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = GameManager.mScore.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (mReturnButton.ReadValue<float>()==1.0f)
        {
            GameManager.mScore = 0;
            SceneManager.LoadScene("Main");
        }
    }
}
