using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskingSound : MonoBehaviour
{
    [SerializeField] private float m_fVolume;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "AIAgent")
        {
            float fPosVol = Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position);
            fPosVol *= m_fVolume;

            if (fPosVol > other.gameObject.GetComponent<AudioRange>().GetHighestVol())
            {
                other.gameObject.GetComponent<AudioRange>().SetHighestVol(fPosVol);
                other.gameObject.GetComponent<AudioRange>().SetLoudestObject(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "AIAgent")
        {
            if (this.gameObject == other.GetComponent<AudioRange>().GetLoudestObject())
            {
                other.GetComponent<AudioRange>().SearchForSounds();
            }
        }
    }

    public float GetVol()
    {
        return m_fVolume;
    }

    public float GetRange()
    {
        return this.GetComponent<SphereCollider>().radius;
    }
}
