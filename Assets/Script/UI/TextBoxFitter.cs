using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxFitter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float padding;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ResizeTextBox()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(text.preferredWidth + padding, text.preferredHeight + padding);
    }
}
