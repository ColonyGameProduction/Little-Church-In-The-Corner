using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    private float greyOutImage = .5f;
    private Image itemImage;

    private void OnEnable()
    {
        itemImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        ChangingTheImageOppacity();
        ShopInventoryUI.OnBuyItem += ChangingTheImageOppacity;
    }

    private void OnDisable()
    {
        ShopInventoryUI.OnBuyItem -= ChangingTheImageOppacity;
    }

    public void ChangingTheImageOppacity()
    {
        if(!itemData.isGet)
        {
            Color color = itemImage.color;
            color.a = greyOutImage;
            itemImage.color = color;
        }
        else
        {
            Color color = itemImage.color;
            color.a = 0f;
            itemImage.color = color;
        }

    }

}
