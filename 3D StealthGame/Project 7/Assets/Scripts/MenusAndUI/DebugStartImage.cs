using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStartImage : MonoBehaviour
{
    [SerializeField] private GameObject PixelShader;

    // Start is called before the first frame update
    void Start()
    {
        PixelShader.SetActive(true);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
