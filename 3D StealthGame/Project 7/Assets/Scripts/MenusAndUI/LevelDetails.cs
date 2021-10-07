using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelDetails : MonoBehaviour
{
    [SerializeField] private GameObject m_goPlayer;
    [SerializeField] private GameObject[] m_goAIs;
    private PlayerController m_PC;
    [SerializeField] private GameObject m_goObjectiveBox;
    [SerializeField] private GameObject m_goObjectiveText;
    private Text m_txBoxText;
    private bool m_bObjectiveBoxOpen = false;
    private int m_iKeyTotal = 0;
    private int m_iBigKeyTotal = 0;
    private bool m_bDoorOpen = false;
    private bool m_bHasFoundDoor = false;

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            m_txBoxText = m_goObjectiveText.GetComponent<Text>();
            m_txBoxText.text = "Locate a way to enter balcony room";
            m_PC = m_goPlayer.GetComponent<PlayerController>();
            m_goAIs = GameObject.FindGameObjectsWithTag("AIAgent");
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            m_txBoxText = m_goObjectiveText.GetComponent<Text>();
            m_txBoxText.text = "Find A Way Up Stairs To CEO Room";
            m_PC = m_goPlayer.GetComponent<PlayerController>();
        }
    }

    public void OpenObjectiveBox()
    {
            m_goObjectiveBox.GetComponent<Animator>().SetBool("IsOpen", !m_bObjectiveBoxOpen);
            m_bObjectiveBoxOpen = !m_bObjectiveBoxOpen;
    }

    private void Update()
    {

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (m_PC.GetInventory().GetHasObjective())
            {
                m_txBoxText.text = "Go Down Ladder and leave through the back";
            }
            else if (m_bDoorOpen)
            {
                m_txBoxText.text = "Retrieve Robot Part (Cube)";
            }
            else if (m_bHasFoundDoor && m_iKeyTotal > 0)
            {
                m_txBoxText.text = "Unlock The Door";
            }
            else if (m_bHasFoundDoor && m_iKeyTotal == 0)
            {
                m_txBoxText.text = "Locate A Key To Unlock The Door";
            }
            else if (!m_bHasFoundDoor && m_iKeyTotal > 0)
            {
                m_txBoxText.text = "Locate A Door To Unlock";
            }
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (m_PC.GetInventory().GetHasObjective())
            {
                m_txBoxText.text = "Return To The Basement";
            }
        }
    }

    public int GetKey()
    {
        return m_iKeyTotal;
    }

    public void TakeKey()
    {
        m_iKeyTotal++;
    }

    public void UseKey()
    {
        m_iKeyTotal--;
    }

    public int GetBigKey()
    {
        return m_iBigKeyTotal;
    }

    public void TakeBigKey()
    {
        m_iBigKeyTotal++;
    }

    public void UseBigKey()
    {
        m_iBigKeyTotal--;
    }

    public void SetSeenDoor(bool a_Seen)
    {
        m_bHasFoundDoor = a_Seen;
    }

    public void OpenDoor()
    {
        m_bDoorOpen = true;
    }

    public GameObject GetPlayer()
    {
        return m_goPlayer;
    }

    public GameObject[] GetAIs()
    {
        return m_goAIs;
    }

    //Note: need to store which keys have been taken
    public int GetKeys()
    {
        return m_iKeyTotal;
    }

    public void SetKeys(int keys)
    {
        m_iKeyTotal = keys;
    }

    public bool GetDoorOpen()
    {
        return m_bDoorOpen;
    }

    public void SetDoorOpen(bool open)
    {
        m_bDoorOpen = open;
    }

    public bool GetHasFoundDoor()
    {
        return m_bHasFoundDoor;
    }

    public void SetHasFoundDoor(bool found)
    {
        m_bHasFoundDoor = found;
    }
}
