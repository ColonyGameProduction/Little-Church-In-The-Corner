using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;



public class ShopInventoryUI : MonoBehaviour
{
    public ItemManager IM;

    public GameObject shopWindow;
    public GameObject inventoryWindow;
    public GameObject setContainerPrefab;
    public GameObject itemButtonPrefab;
    public GameObject contentGameObject;

    public AllItemSO allInventoryItem;
    public List<Button> itemToBuyDirectly;
    public List<GameObject> allItemGameObject;

    public bool isShoping = false;

    public static event System.Action OnBuyItem;

    private void Start()
    {
        IM = ItemManager.instance;

        StartCoroutine(DelayedStart());

        //SetupShopUI();
    }

    // Ada ini karena Setup Inventory perlu item sets dari Item Manager, di mana dia belum diinitialize.
    private IEnumerator DelayedStart()
    {
        yield return null;
        shopWindow.SetActive(true);
        inventoryWindow.SetActive(true);
        SetupItemToBuyDirectlyToButton();
        SetupChests();
        ResetInventoryItem();
        SetupTheItemInventory();
    }

    private void SetupChests()
    {
        ChestUI[] chestUIs = (ChestUI[])FindObjectsByType(typeof(ChestUI), FindObjectsSortMode.None);
        foreach (ChestUI chestUI in chestUIs)
        {
            chestUI.EnableChest(true);
        }
    }

    public void SetupShopUI()
    {
        WhatUIToShow(shopWindow, inventoryWindow);
        if(!isShoping)
        {
            isShoping = true;
            SetupItemToBuyDirectlyToButton();
        }
    }

    public void SetupInventoryUI()
    {
        WhatUIToShow(inventoryWindow, shopWindow);
        if(isShoping)
        {
            ResetInventoryItem();
            SetupTheItemInventory();
        }
    }

    public void ResetInventoryItem()
    {
        if(allItemGameObject.Count > 0)
        {
            foreach(GameObject item in allItemGameObject)
            {
                Destroy(item);
            }
        }
    }

    public void SetupTheItemInventory()
    {
        
        var collected = IM.GM.collectedItems;
        

        foreach (var pair in collected)
        {
            string itemID = pair.Key;
            bool isCollected = pair.Value;

            ItemData currItem;

            foreach(ItemData item in IM.allItems)
            {
                if(itemID == item.itemSO.itemID)
                {
                    item.isGet = isCollected;
                    currItem = item;
                }
            }
        }

        foreach(ItemSetData itemSet in IM.allSetsItems)
        {
            GameObject currSetContainer = Instantiate(setContainerPrefab, contentGameObject.transform);
            TextMeshProUGUI setName = currSetContainer.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            setName.text = itemSet.setItem.setName;
            TextBoxFitter setTextBox = currSetContainer.transform.GetChild(0).gameObject.GetComponent<TextBoxFitter>();
            setTextBox.ResizeTextBox();

            foreach (ItemData item in itemSet.items)
            {
                GameObject currItem = Instantiate(itemButtonPrefab, currSetContainer.transform.GetChild(1));
                Item itemComponent = currItem.GetComponent<Item>();
                Image itemImage = currItem.transform.GetChild(0).GetComponent<Image>();
                itemComponent.itemData = item;
                itemImage.sprite = item.itemSO.itemSprite;

                

            }
            allItemGameObject.Add(currSetContainer);
        }
    }


    public void BuyItemsDirectly(ItemSO data)
    {
        if(IM.GM.playerMoney >= data.itemPrice)
        {
            IM.PlayerCollectTheItem(data);
        }
    }



    public void SetupItemToBuyDirectlyToButton()
    {
        //randomkan item disini

        System.Random rng = new System.Random(Mathf.Abs(IM.GM.GettingTheSeed()));


        for (int i = 0; i < itemToBuyDirectly.Count; i++)
        {
            int index = rng.Next(0, allInventoryItem.items.Count);
            ItemSO item = allInventoryItem.items[index];
            ItemSO itemCopy = item;
            itemToBuyDirectly[i].onClick.RemoveAllListeners();
            itemToBuyDirectly[i].onClick.AddListener(() => BuyItemsDirectly(itemCopy));

        }
    }
    public void BuyChest(ChestUI chestUI)
    {
        if(IM.GM.playerMoney >= IM.chestPrice)
        {
            IM.PlayerCollectTheItem(GetItemFromChest());
            chestUI.EnableChest(false);
            //ResetInventoryItem();
            //SetupTheItemInventory();
            OnBuyItem?.Invoke();
        }
    }

    public ItemSO GetItemFromChest()
    {
        List<ItemData> tempItemToRandom = new List<ItemData>();

        float standartPercentage = IM.standardItemGetChance * 0.01f;
        float mediumPercentage = IM.mediumItemGetChance * 0.01f;
        float rarePercentage = IM.rareItemGetChance * 0.01f;

        float totalPercentage = standartPercentage + mediumPercentage + rarePercentage;
        if (totalPercentage > 1)
        {
            Debug.Log("Total persen lebih besar dari 100%");
            return null;
        }

        float realStandartPercentage = standartPercentage;
        float realMediumPercentage = standartPercentage + mediumPercentage;
        float realRarePercentage = standartPercentage + mediumPercentage + rarePercentage;

        float random = Random.Range(0f, 1f);

        if (random <= realStandartPercentage)
        {
            ItemData itemToGet = IM.standardRarityItems[Random.Range(0, tempItemToRandom.Count)];
            return itemToGet.itemSO;
        }
        else if (random <= realMediumPercentage)
        {
            ItemData itemToGet = IM.mediumRarityItems[Random.Range(0, tempItemToRandom.Count)];
            return itemToGet.itemSO;
        }
        else if (random <= realRarePercentage)
        {
            ItemData itemToGet = IM.rareRarityItems[Random.Range(0, tempItemToRandom.Count)];
            return itemToGet.itemSO;
        }
        Debug.Log("Didn't Get Any Items");
        return null;
    }

    public void WhatUIToShow(GameObject clicked, GameObject hide)
    {
        clicked.SetActive(true);
        hide.SetActive(false);
    }
}
