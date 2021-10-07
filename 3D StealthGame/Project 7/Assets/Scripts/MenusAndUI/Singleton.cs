using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<singleton> : MonoBehaviour where singleton : MonoBehaviour
{
    private static singleton instance;

    public static singleton Instance
    {
        get
        {
            if(instance == null)
            {
                instance = (singleton)FindObjectOfType<singleton> ();

                if(instance == null)
                {
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<singleton>();
                    singletonObject.name = typeof(singleton).ToString() + "Singleton";

                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
}