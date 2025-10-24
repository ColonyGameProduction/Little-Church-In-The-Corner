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
        UIChatManager.ACT_NoCurrentSermonAvailable += ShowInteractButton;
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactIsReady -= ShowInteractButton;
        BTN_interact.onClick.RemoveListener(ClickInteractButton);
        UIChatManager.ACT_NoCurrentSermonAvailable -= ShowInteractButton;
    }

    /// <summary>
    /// Cuma tampilin aja.
    /// </summary>
    private void ShowInteractButton()
    {
        //Kalau lagi ada renungan yang berjalan, jangan tampilin interact button.
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;
        //Kalau ga ada renungan dalam queue, jangan tampilin interact button.
        if (TimeManager.Instance.I_queuedSermon <= 0)
            return;

        BTN_interact.gameObject.SetActive(true);
    }

    /// <summary>
    /// Pas click, bakal setup chat, lalu sembunyiin tombol interactnya.
    /// </summary>
    private void ClickInteractButton()
    {
        UIChatManager.Instance.SetupAllChats();
        BTN_interact.gameObject.SetActive(false);
        //Kurangin queue renungan
        TimeManager.Instance.I_queuedSermon--;
    }
}
