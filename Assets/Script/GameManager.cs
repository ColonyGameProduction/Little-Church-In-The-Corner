using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<ItemSaveData> items = new List<ItemSaveData>();
}

[Serializable]
public class ItemSaveData
{
    public string itemID;
    public bool isGet;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Dictionary<string, bool> collectedItems { get; private set; } = new Dictionary<string, bool>();

    public string stringPlayerID;
    public int integerPlayerID;
    public int playerMoney;

    public E_GameMode gameMode = E_GameMode.None;

    public static event System.Action<int> OnPlayerMoneyUpdated;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        InitPlayerID();
    }

    public int GettingTheSeed()
    {
        return (integerPlayerID * 73856093) ^ (GetWeeklyCycle() * 19349663);
    }

    public int GetWeeklyCycle()
    {
        DateTime now = DateTime.UtcNow;
        return now.Year * 100 + (now.DayOfYear / 7);
    }
    public void InitPlayerID()
    {
        if (!PlayerPrefs.HasKey("playerID"))
        {
            stringPlayerID = Guid.NewGuid().ToString();
            PlayerPrefs.SetString("playerID", stringPlayerID);
        }
        else
        {
            stringPlayerID = PlayerPrefs.GetString("playerID");
        }

        integerPlayerID = stringPlayerID.GetHashCode();

        OnPlayerMoneyUpdated?.Invoke(playerMoney);
    }

    

    public void SaveGameItem()
    {
        SaveData data = new SaveData();

        foreach (var pair in collectedItems)
        {
            ItemSaveData item = new ItemSaveData();
            item.itemID = pair.Key;
            item.isGet = pair.Value;

            data.items.Add(item);
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("collectible_save", json);
    }

    public void LoadGameItem()
    {
        string json = PlayerPrefs.GetString("collectible_save", "");

        if (string.IsNullOrEmpty(json))
            return;

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var item in data.items)
        {
            if (collectedItems.ContainsKey(item.itemID))
            {
                collectedItems[item.itemID] = item.isGet;
            }
        }
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        OnPlayerMoneyUpdated?.Invoke(playerMoney);
    }
}
