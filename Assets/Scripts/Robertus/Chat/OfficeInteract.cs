using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class untuk mengatur tombol interact NPC di ruangan Office. Berbeda dengan Interact biasa karena interact biasa perlu otak-atik hal-hal yang berkaitan dengan gereja, dan itu terpisah dengan yang ada di office.
/// </summary>
public class OfficeInteract : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Pas click, bakal setup chat, lalu sembunyiin tombol interactnya.
    /// </summary>
    private void ClickInteractButton()
    {
        //Dua line di bawah ini tetep sama seperti Interact Church
        ChatManager.Instance.SetupRenungan();
        UIChatManager.Instance.SetupAllChats();
        //Kurangin queue QnA kantor. Ini yang berbeda
        TimeManager.Instance.I_queuedQnA--;

        //Pas interact, munculin NPC
        OfficeManager.Instance.SetupNPC();
        //Sembunyiin icon interactnya
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Kalau tombol interact diklik
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        ClickInteractButton();
    }
}
