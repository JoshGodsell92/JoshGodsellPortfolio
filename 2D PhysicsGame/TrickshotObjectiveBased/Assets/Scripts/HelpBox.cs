using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpBox : MonoBehaviour
{
    [SerializeField]
    private string sHelpMessage;

    public string getMessage()
    {
        return sHelpMessage;
    }

    public void setMessage(string a_message)
    {
        sHelpMessage = a_message;
    }

}
