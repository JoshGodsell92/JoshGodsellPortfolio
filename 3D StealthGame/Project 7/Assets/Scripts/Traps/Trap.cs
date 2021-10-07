using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private bool bActive; //is the trap currently active?

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetActive()
    {
        return bActive;
    }

    public void SetActive(bool active)
    {
        bActive = active;
    }
}
