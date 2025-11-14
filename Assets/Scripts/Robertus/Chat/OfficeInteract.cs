using UnityEngine;
using UnityEngine.EventSystems;

public class OfficeInteract : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Pas click, bakal setup chat, lalu sembunyiin tombol interactnya.
    /// </summary>
    private void ClickInteractButton()
    {
        ChatManager.Instance.SetupRenungan();
        UIChatManager.Instance.SetupAllChats();
        //Kurangin queue QnA kantor
        TimeManager.Instance.I_queuedQnA--;

        OfficeManager.Instance.SetupNPC();
        //Sembunyiin icon interactnya
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Kalau tombol Alert diklik
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        ClickInteractButton();
    }
}
