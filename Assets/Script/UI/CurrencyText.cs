using TMPro;
using UnityEngine;

public class CurrencyText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        GameManager.OnPlayerMoneyUpdated += UpdateText;
        if(GameManager.instance) UpdateText(GameManager.instance.playerMoney);
    }

    private void OnDisable()
    {
        GameManager.OnPlayerMoneyUpdated -= UpdateText;
    }

    private void UpdateText(int value)
    {
        text.text = value.ToString();
    }
}
