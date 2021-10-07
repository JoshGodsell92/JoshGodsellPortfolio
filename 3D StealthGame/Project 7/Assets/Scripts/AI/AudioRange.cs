using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRange : MonoBehaviour
{

    //The range that the drone can hear
    [SerializeField] private float fRange;

    //An ID for the drone to make them easy to identify in debug logs
    [SerializeField] private int iID;

    [SerializeField] private float m_fHighestVolume;

    [SerializeField] private GameObject m_goLoudestObject;

    [SerializeField] private float fPosVol;

    //Draw the range of the audio range for testing
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fRange);
    }

    private void Update()
    {
        SearchForSounds();
    }

    public void SearchForSounds()
    {
        m_fHighestVolume = 0.0f;
        m_goLoudestObject = null;
        fPosVol = 0.0f;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("MaskingSound"))
        {
            MaskingSound ms;
            try
            {
                ms = go.GetComponent<MaskingSound>();
                if (Vector3.Distance(this.gameObject.transform.position, go.transform.position) <= ms.GetRange())
                {
                    fPosVol = (Vector3.Distance(this.gameObject.transform.position, go.transform.position) / ms.GetRange()) * 100;
                    fPosVol *= ms.GetVol();
                    if (fPosVol > m_fHighestVolume)
                    {
                        m_fHighestVolume = fPosVol;
                        m_goLoudestObject = go;
                    }
                }
            }
            catch
            {
                Debug.Log("Please add the correct script");
            }
        }
    }

    //Getters for the range and ID
    public float GetAudioRange()
    {
        return fRange;
    }

    public int GetID()
    {
        return iID;
    }

    public float GetHighestVol()
    {
        return m_fHighestVolume;
    }

    public void SetHighestVol(float a_fVol)
    {
        m_fHighestVolume = a_fVol;
    }

    public GameObject GetLoudestObject()
    {
        return m_goLoudestObject;
    }

    public void SetLoudestObject(GameObject a_goObject)
    {
        m_goLoudestObject = a_goObject;
    }

}
