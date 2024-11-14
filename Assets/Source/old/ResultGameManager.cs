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

    // �L����
    private void OnEnable()
    {
        // InputAction��L����
        // ��������Ȃ��Ɠ��͂��󂯎��Ȃ����Ƃɒ���
        mReturnButton?.Enable();
    }

    // ������
    private void OnDisable()
    {
        // ���g�������������^�C�~���O�Ȃǂ�
        // Action�𖳌�������K�v������
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
