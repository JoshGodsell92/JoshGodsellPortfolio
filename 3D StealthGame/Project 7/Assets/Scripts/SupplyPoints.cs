using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyPoints : MonoBehaviour
{
    [SerializeField] Vector3 v3SpawnPos;
    [SerializeField] bool bIsUsed = false;

    void Start()
    {
        v3SpawnPos = gameObject.transform.position;
        bIsUsed = false;
    }

    public void SelfInit()
    {
        v3SpawnPos = gameObject.transform.position;
        bIsUsed = false;
    }

    public void SetSpawnPos(Vector3 pos)
    {
        v3SpawnPos = pos;
    }

    public Vector3 GetSpawnPos()
    {
        return v3SpawnPos;
    }

    public void SetUsed(bool used)
    {
        bIsUsed = used;
    }

    public bool GetUsed()
    {
        return bIsUsed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(this.transform.position,new Vector3(0.5f,0.5f,0.5f));
    }
}
