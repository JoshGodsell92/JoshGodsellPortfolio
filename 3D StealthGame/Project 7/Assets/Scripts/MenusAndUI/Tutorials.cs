using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    [SerializeField] private GameObject m_goTutUI;
    [SerializeField] private GameObject m_goTut;

    private PlayerController m_PC;

    bool b_SeenMessage = false;

    private void Awake()
    {
        m_PC = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        if(gameObject.CompareTag("GameManager"))
        {
            m_goTutUI.SetActive(true);
            m_goTut.SetActive(true);

            m_PC.SetTut(true);

            Time.timeScale = 0.0f;

            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    public void OpenTut()
    {
        if(!b_SeenMessage)
        {
            b_SeenMessage = true;
            m_goTutUI.SetActive(true);
            m_goTut.SetActive(true);

            m_PC.SetTut(true);

            Time.timeScale = 0.0f;

            Cursor.lockState = CursorLockMode.None;

            Cursor.visible = true;
        }
    }

    public void Dismiss()
    {
        m_goTut.SetActive(false);
        m_goTutUI.SetActive(false);

        m_PC.SetTut(false);

        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
