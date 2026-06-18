using UnityEngine;
using UnityEngine.UI;

public class ShowHideCustomizationButtons : MonoBehaviour
{
    // Ini class sementara untuk showcase kalau bakal ada fitur customization nantinya.
    // Fitur customization hanya ada untuk gereja
    // Tombol=tombol ini hanya bakal muncul pas lagi di ruangan gereja.

    [SerializeField] private GameObject furnitureButton;
    [SerializeField] private GameObject characterOutfitButton;

    private void OnEnable()
    {
        TransitionManager.OnRoomChange += ShowHideButtons;
    }

    private void OnDisable()
    {
        TransitionManager.OnRoomChange -= ShowHideButtons;
    }

    private void ShowHideButtons(ENM_Room room)
    {
        // Bakal muncul kalau ruangan yang barusan diganti itu gereja (sama aja kayak SetActive(true))
        furnitureButton.SetActive(room == ENM_Room.Church);
        characterOutfitButton.SetActive(room == ENM_Room.Church);
    }
}
