//////////////////////////////////////////////////////////////////////////
///File name: PatrolPoint.cs
///Date Created: 12/10/2020
///Created by: JG
///Brief: Class for patrolPoint data.
///Last Edited by: JG
///Last Edited on: 12/10/2020
//////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    private Vector3 m_v3Position;

    [SerializeField]
    private GameObject[] m_aNeighbourPoints;

    // Start is called before the first frame update
    void Start()
    {

        m_v3Position = this.transform.position;

    }


    #region Get&Set

    public Vector3 GetPosition()
    {
        return m_v3Position;
    }
    public void SetPosition(Vector3 a_v3Position)
    {
        this.transform.position = a_v3Position;
        m_v3Position = a_v3Position;
    }

    public GameObject[] GetNeighbours()
    {
        return m_aNeighbourPoints;
    }
    public void SetNeighbours(GameObject[] a_aNeighbours)
    {
        m_aNeighbourPoints = a_aNeighbours;
    }

    #endregion   //!Get&Set

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_aNeighbourPoints.Length > 0)
        {
            for (int i = 0; i < m_aNeighbourPoints.Length; i++)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawLine(this.transform.position, m_aNeighbourPoints[i].transform.position);
            }
        }
    }

}
