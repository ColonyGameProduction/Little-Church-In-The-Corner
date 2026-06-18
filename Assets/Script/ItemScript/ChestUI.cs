using TMPro;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject priceIcon;

    public void EnableChest(bool isEnabled)
    {
        //Debug.Log($"{(isEnabled ? "Enabled" : "Disabled")} CHEST for {gameObject}", gameObject);
        if (isEnabled)
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            priceText.text = ItemManager.instance.chestPrice.ToString();
            priceIcon.SetActive(true);
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0.5f;
            priceText.text = "SOLD";
            priceIcon.SetActive(false);
        }
    }
}
