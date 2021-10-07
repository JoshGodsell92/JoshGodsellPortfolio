using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplySpawner : MonoBehaviour
{
    [SerializeField] SupplyPoints[] SupplySpawns;
    [SerializeField] int iNumOfSupplies;
    [SerializeField] GameObject goSupplyBoxPrefab;

    // Start is called before the first frame update
    void Start()
    {

        foreach(SupplyPoints point in SupplySpawns)
        {
            point.SelfInit();
        }

        for (int i = 0; i < iNumOfSupplies; i++)
        {
            int random = Random.Range(0, SupplySpawns.Length);
            if (!SupplySpawns[random].GetUsed())
            {
                GameObject supply = Instantiate(goSupplyBoxPrefab, SupplySpawns[random].GetSpawnPos(), SupplySpawns[random].transform.rotation); //make sure to avoid double spawning
                SupplySpawns[random].SetUsed(true);
                if (supply.GetComponent<Supply>() != null)
                {
                    supply.GetComponent<Supply>().SetupSupply();
                }
                else
                {
                    Debug.LogError("Supply prefab does not have a script.");
                }
            }
            else
            {
                i--; //decrement i to go back
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
