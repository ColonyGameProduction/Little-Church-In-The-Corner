using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ini buat ngatur interact dengan chat
/// </summary>
public class Interact : MonoBehaviour
{
    public Button BTN_interact;

    private void Awake()
    {
        BTN_interact.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        TimeManager.ACT_interactIsReady += ShowInteractButton;
        BTN_interact.onClick.AddListener(ClickInteractButton);
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactIsReady -= ShowInteractButton;
        BTN_interact.onClick.RemoveListener(ClickInteractButton);
    }

    /// <summary>
    /// Cuma tampilin aja.
    /// </summary>
    private void ShowInteractButton()
    {
        BTN_interact.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pas click, bakal setup chat, lalu sembunyiin tombol interactnya.
    /// </summary>
    private void ClickInteractButton()
    {
        UIChatManager.Instance.SetupAllChats();
        BTN_interact.gameObject.SetActive(false);
    }
}
