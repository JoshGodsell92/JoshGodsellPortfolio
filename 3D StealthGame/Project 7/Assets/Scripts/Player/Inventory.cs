using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////
// File: Inventory.cs
// Author: Cameron Lillie
// Brief: Script for handling the inventory, keeping track on items, etc.
////////////////////////////////////////////////////////////

[System.Serializable]
public class InventoryData
{
    public int iMedkitStock;
    public int iSmokeStock;
    public int iStunStock;
    public int iRopeStock;
    public int iDecoyStock;
    public bool bHasObjective;
    public int iCurrentItem;

    public InventoryData(int medStock, int smokeStock, int stunStock, int ropeStock, int decoyStock, bool objective, int currentItem)
    {
        iMedkitStock = medStock;
        iSmokeStock = smokeStock;
        iStunStock = stunStock;
        iRopeStock = ropeStock;
        iDecoyStock = decoyStock;
        bHasObjective = objective;
        iCurrentItem = currentItem;
    }
}

public class Inventory : MonoBehaviour
{
    //[SerializeField] int iMedkitStock;
    [SerializeField] int iMedkitMax;
    [SerializeField] int iMedkitHealAmount;

    //[SerializeField] int iSmokeGrenadeStock;
    [SerializeField] int iSmokeGrenadeMax;

    //[SerializeField] int iStunGrenadeStock;
    [SerializeField] int iStunGrenadeMax;

    //[SerializeField] int m_iRopeStock;
    [SerializeField] int m_iRopeMax;

    //[SerializeField] 
    //private int iDecoyStock;

    [SerializeField]
    private int iDecoyMax;


    //[SerializeField]
    //private bool bHasObjective;

    public enum CURRENT_ITEM
    {
        MEDKIT = 0,
        SMOKEGRENADE = 1,
        STUNGRENADE = 2,
        DECOY = 3
    }

    [SerializeField] private CURRENT_ITEM eCurrentItem;

    [SerializeField] InventoryData inventoryData;

    // Use this for initialization
    void Start()
    {
        //initialise
        inventoryData.iMedkitStock = iMedkitMax;
        inventoryData.iSmokeStock = iSmokeGrenadeMax;
        inventoryData.iStunStock = iStunGrenadeMax;
        inventoryData.iRopeStock = m_iRopeMax;
        inventoryData.iDecoyStock = iDecoyMax;
        inventoryData.bHasObjective = false;
        inventoryData.iCurrentItem = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int GetMedkitStock()
    {
        return inventoryData.iMedkitStock;
    }

    public void SetMedkitStock(int stock)
    {
        inventoryData.iMedkitStock = stock;
    }

    public int GetMedkitMax()
    {
        return iMedkitMax;
    }

    public int GetHealAmount()
    {
        return iMedkitHealAmount;
    }

    public CURRENT_ITEM GetCurrentItem()
    {
        return eCurrentItem;
    }

    public void SetCurrentItem(CURRENT_ITEM item)
    {
        eCurrentItem = item;
    }

    public void SetCurrentItem(int item)
    {
        eCurrentItem = (CURRENT_ITEM)item;
    }

    public int GetSmokeStock()
    {
        return inventoryData.iSmokeStock;
    }

    public void SetSmokeStock(int stock)
    {
        inventoryData.iSmokeStock = stock;
    }

    public int GetSmokeMax()
    {
        return iSmokeGrenadeMax;
    }

    public int GetStunStock()
    {
        return inventoryData.iStunStock;
    }

    public void SetStunStock(int stock)
    {
        inventoryData.iStunStock = stock;
    }

    public int GetStunMax()
    {
        return iStunGrenadeMax;
    }

    public int GetDecoyStock()
    {
        return inventoryData.iDecoyStock;
    }

    public int GetDecoyMax()
    {
        return iDecoyMax;
    }

    public void SetDecoyStock(int stock)
    {
        inventoryData.iDecoyStock = stock;
    }

    public int GetRopeStock()
    {
        return inventoryData.iRopeStock;
    }

    public int GetRopeMax()
    {
        return m_iRopeMax;
    }

    public void SetRopeStock(int a_stock)
    {
        inventoryData.iRopeStock = a_stock;
    }

    public int GetCurrentStock()
    {
        int t_iStockVal = 0;

        switch(eCurrentItem)
        {

            case CURRENT_ITEM.MEDKIT:

                t_iStockVal = GetMedkitStock();

                break;

            case CURRENT_ITEM.SMOKEGRENADE:

                t_iStockVal = GetSmokeStock();

                break;

            case CURRENT_ITEM.STUNGRENADE:

                t_iStockVal = GetStunStock();

                break;

            case CURRENT_ITEM.DECOY:

                t_iStockVal = GetDecoyStock();

                break;

            default:
                break;
        }

        return t_iStockVal;
    }

    public void SetHasObjective(bool a_bHasObjective)
    {
        inventoryData.bHasObjective = a_bHasObjective;
    }

    public bool GetHasObjective()
    {
        return inventoryData.bHasObjective;
    }

    public InventoryData ExportInventory()
    {
        //convert current item to an int to make it export properly
        inventoryData.iCurrentItem = (int)eCurrentItem;
        return inventoryData;
    }

    public void ImportInventory(InventoryData data)
    {
        inventoryData = data;
        //set current item based on imported data
        SetCurrentItem(inventoryData.iCurrentItem);
    }
}
