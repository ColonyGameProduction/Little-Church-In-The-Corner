using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    private void UpdateText(float value)
    {
        text.text = value.ToString("F0");
    }
}
