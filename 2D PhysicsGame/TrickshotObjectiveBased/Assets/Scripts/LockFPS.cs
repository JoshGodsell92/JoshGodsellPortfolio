using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFPS : MonoBehaviour {

    void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled - KT
        Application.targetFrameRate = 30; //Lock the frame rate to 30 - KT
    }
}
