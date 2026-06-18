using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemAmountText;
    [SerializeField] private ShopInventoryUI shopInventoryUI;

    public void SetRandomItem()
    {
        ItemSO randomItem = shopInventoryUI.GetItemFromChest();
        Setup(randomItem);
        ItemManager.instance.PlayerCollectTheItem(randomItem);
    }

    public void Setup(ItemSO itemSO)
    {
        itemIcon.sprite = itemSO.itemSprite;
        // Untuk sekarang masih x1
        itemAmountText.text = "x1";
    }
}
