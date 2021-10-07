//////////////////////////////////////////////////////////////////////////
///File name: DroneTask.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: class for Attaching tasks for drones to complete.
///Last Edited by: JG
///Last Edited on: 09/11/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneTask : MonoBehaviour
{
    [SerializeField]
    private string m_sName;

    private Vector3 m_v3Position;

    [SerializeField]
    private float m_fTimeForTask;

    [SerializeField]
    private bool m_bOccupied;

    [SerializeField]
    private bool m_bOnCooldown;

    [SerializeField]
    private float m_fTimeToRefresh;

    private void Start()
    {
        m_v3Position = this.gameObject.transform.position;
    }

    public void StartCooldown()
    {

        m_bOnCooldown = true;

        StartCoroutine(Cooldown());


    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(m_fTimeToRefresh);


        m_bOccupied = false;
        m_bOnCooldown = false;

        yield return null;
    }

    #region Get&Set

    public string GetName()
    {
        return m_sName;
    }
    public void SetName(string a_sName)
    {
        m_sName = a_sName;
    }

    public Vector3 GetPosition()
    {
        return m_v3Position;
    }
    public void SetPosition(Vector3 a_v3Position)
    {
        m_v3Position = a_v3Position;
    }

    public float GetTaskTime()
    {
        return m_fTimeForTask;
    }
    public void SetTaskTime(float a_fTimeForTask)
    {
        m_fTimeForTask = a_fTimeForTask;
    }

    public bool GetIsOccupied()
    {
        return m_bOccupied;
    }
    public void SetIsOccupied(bool a_bool)
    {
        m_bOccupied = a_bool;
    }

    public bool GetIsOnCooldown()
    {
        return m_bOnCooldown;
    }
    public void SetIsOnCooldown(bool a_bool)
    {
        m_bOnCooldown = a_bool;
    }

    public float GetCooldown()
    {
        return m_fTimeToRefresh;
    }
    public void SetCooldown(float a_fCooldownTime)
    {
        m_fTimeToRefresh = a_fCooldownTime;
    }


    #endregion //!Get&Set

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position + (Vector3.up * 3.0f), 0.5f);
    }
}
