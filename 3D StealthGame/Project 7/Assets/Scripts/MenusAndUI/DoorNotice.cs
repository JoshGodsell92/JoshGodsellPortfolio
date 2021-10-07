using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNotice : MonoBehaviour
{
    [SerializeField] private GameObject m_goGameController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_goGameController.GetComponent<LevelDetails>().SetSeenDoor(true);
        }
    }
}
