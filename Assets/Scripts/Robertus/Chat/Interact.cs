using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class untuk interact dengan NPC. Sekarang pakai 3D model dan bukan tombol, makanya ada IPointerClickHandler.
/// Also, jangan lupa masukin komponen Physics Raycaster ke kamera.
/// </summary>
public class Interact : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Kalau misalnya udah diklik dan diinteract, maka ga boleh didelete sebelum renungannya selesai.
    /// </summary>
    public bool B_hasBeenInteracted;

    /// <summary>
    /// Pas click, bakal setup chat, lalu sembunyiin tombol interactnya.
    /// </summary>
    private void ClickInteractButton()
    {
        //Kalau masih ada renungan yang jalan, jangan interact lagi
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;

        //Kalau ga ada renungan dalam queue, jangan tampilin interact button.
        if (TimeManager.Instance.I_queuedSermon <= 0)
            return;

        ChatManager.Instance.SetupRenungan();
        UIChatManager.Instance.SetupAllChats();
        //Kurangin queue renungan
        TimeManager.Instance.I_queuedSermon--;

        B_hasBeenInteracted = true;
    }

    /// <summary>
    /// Kalau NPC diklik
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"CLICKED NPC {gameObject.name}", gameObject);
        ClickInteractButton();
    }
}
