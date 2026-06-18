using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Scriptable Objects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public string itemID;

    public E_ItemRarity itemRarity;
    public string itemType;

    public string itemName;
    public int itemPrice;
    public Sprite itemSprite;
}
