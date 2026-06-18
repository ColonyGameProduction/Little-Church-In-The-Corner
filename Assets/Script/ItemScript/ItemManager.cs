using System;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;


[Serializable]
public class ItemData
{
    public ItemSO itemSO;
    public bool isGet;
}

[Serializable]
public class ItemSetData
{
    public setItemSO setItem;
    public List<ItemData> items = new List<ItemData>();
}


public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public GameManager GM;


    public int chestPrice;

    public AllItemSO allItemSO;
    public AllSetsItemSO allSetsItemSO;

    public List<ItemData> allItems;
    public List<ItemSetData> allSetsItems;

    public List<ItemData> standardRarityItems;
    public List<ItemData> mediumRarityItems;
    public List<ItemData> rareRarityItems;

    public float standardItemGetChance;
    public float mediumItemGetChance;
    public float rareItemGetChance;



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GM = GameManager.instance;
        InitializingAllItem();
    }



    public void PlayerCollectTheItem(ItemSO item)
    {
        if (!GM.collectedItems.ContainsKey(item.itemID))
            return;

        GM.collectedItems[item.itemID] = true;

        GM.SaveGameItem();
    }

    public void InitializingAllItem()
    {
        int allItemCount = allItemSO.items.Count;
        int allSetItemsCount = allSetsItemSO.items.Count;

        foreach (ItemSO item in allItemSO.items)
        {
            GM.collectedItems[item.itemID] = false;
        }
        GM.LoadGameItem();
        GM.SaveGameItem();

        for (int i = 0; i < allItemCount; i++)
        {
            ItemData item = new ItemData();
            item.itemSO = allItemSO.items[i];
            item.isGet = GM.collectedItems[item.itemSO.itemID];
            allItems.Add(item);
        }

        for (int i = 0; i < allItemCount; i++)
        {
            ItemData item = allItems[i];
            if (item.itemSO.itemRarity == E_ItemRarity.standard)
            {
                standardRarityItems.Add(item);
            }
            else if (item.itemSO.itemRarity == E_ItemRarity.rare)
            {
                mediumRarityItems.Add(item);
            }
            else if (item.itemSO.itemRarity == E_ItemRarity.ultra_rare)
            {
                rareRarityItems.Add(item);
            }
        }

        foreach (var itemSets in allSetsItemSO.items)
        {
            ItemSetData currItemSets = new ItemSetData();
            //Dapet Set nya satu satu
            //Debug.Log(itemSets);
            currItemSets.setItem = itemSets;
            foreach (var item in itemSets.items)
            {
                //Dapet List Item nya
                //Debug.Log(item);
                foreach (var itemData in allItems)
                {
                    if (itemData.itemSO.itemID == item.itemID)
                    {
                        //Debug.Log(itemData);
                        currItemSets.items.Add(itemData);
                    }
                }
            }
            allSetsItems.Add(currItemSets);
        }
    }

}
