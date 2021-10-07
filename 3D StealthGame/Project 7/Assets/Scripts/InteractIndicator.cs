using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractIndicator : MonoBehaviour
{
    private AI_Blackboard m_Blackboard;

    private Vector3 m_v3PlayerPos;

    private Vector3 PivotPos;
    private float MaxMove = 0.0f;

    [SerializeField] Vector3 v3Offset;

    // Start is called before the first frame update
    void Start()
    {

        PivotPos = this.transform.position;

        if(m_Blackboard == null)
        {
            m_Blackboard = FindObjectOfType<AI_Blackboard>();
        }


    }

    // Update is called once per frame
    void Update()
    {

        m_v3PlayerPos = m_Blackboard.GetPlayerObject().transform.position + v3Offset;

        this.transform.LookAt(transform.position * 2 - Camera.main.transform.position);

        Vector3 moveVec = m_v3PlayerPos - PivotPos;

        if(moveVec.magnitude > 1.6f)
        {

            GetComponent<Canvas>().enabled = false;

        }
        else
        {
            GetComponent<Canvas>().enabled = true;


            //moveVec = moveVec * MaxMove;

            //this.transform.position = PivotPos + moveVec;
        }

    }
}
