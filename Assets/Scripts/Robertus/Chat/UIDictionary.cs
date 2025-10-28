using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class untuk mengatur UI dari dictionary atau renungan-renungan yang sudah didownload.
/// </summary>
public class UIDictionary : MonoBehaviour
{
    /// <summary>
    /// Prefab dari item-item renungan yang sudah didownload, berisi judul renungan serta tombol delete dan favorit.
    /// </summary>
    public GameObject PB_dictionaryListButton;
    /// <summary>
    /// Menu yang menampilkan isi dari renungan yang saat ini dipilih.
    /// </summary>
    public GameObject GO_currSavedDialogContainer;
    /// <summary>
    /// Parent dari menu yang menampilkan isi dari renungan yang saat ini dipilih. Digunakan untuk menampilkan chat bubble renungan yang terpilih.
    /// </summary>
    public Transform TF_currSavedDialogContainer;
    /// <summary>
    /// Parent dari list renungan yang sudah didownload.
    /// </summary>
    public Transform TF_dictionaryListParent;
    /// <summary>
    /// Ini digunakan untuk track semua item dalam list renungan yang sudah didownload. Berguna untuk reset UI seluruh item renungan.
    /// </summary>
    private List<UISavedSermonItem> List_SCR_savedSermonItemList;
    /// <summary>
    /// Tombol untuk membuka dan membaca isi dari renungan yang sudah terpilih.
    /// </summary>
    public Button BTN_openSelectedSermon;
    /// <summary>
    /// Tombol untuk menutup isi renungan yang sudah dipilih.
    /// </summary>
    public Button BTN_closeSelectedSermon;

    /// <summary>
    /// Item renungan yang saat ini dipilih. Ini digunakan untuk delete renungan, biar bisa dihapus dari UI juga.
    /// </summary>
    [HideInInspector] public UISavedSermonItem SCR_currSelectedSermon;

    /// <summary>
    /// Panel konfirmasi pas mau delete renungan
    /// </summary>
    public GameObject GO_deleteConfirmationPanel;
    /// <summary>
    /// Teks yang bakal ditampilkan pas mau delete renungan
    /// </summary>
    public TextMeshProUGUI TMPUGUI_deleteConfirmationText;
    /// <summary>
    /// Tombol YES untuk delete renungan
    /// </summary>
    public Button BTN_yesDeleteSermon;
    /// <summary>
    /// Tombol NO untuk delete renungan
    /// </summary>
    public Button BTN_noDeleteSermon;

    private void OnEnable()
    {
        BTN_openSelectedSermon.onClick.AddListener(OpeningSavedSermon);
        BTN_closeSelectedSermon.onClick.AddListener(CloseSavedSermon);
        BTN_yesDeleteSermon.onClick.AddListener(DeleteSelectedDialog);
        BTN_noDeleteSermon.onClick.AddListener(HideDeleteConfirmation);
    }

    private void OnDisable()
    {
        BTN_openSelectedSermon.onClick.RemoveListener(OpeningSavedSermon);
        BTN_closeSelectedSermon.onClick.RemoveListener(CloseSavedSermon);
        BTN_yesDeleteSermon.onClick.RemoveListener(DeleteSelectedDialog);
        BTN_noDeleteSermon.onClick.RemoveListener(HideDeleteConfirmation);
    }

    /// <summary>
    /// Menyiapkan seluruh list renungan yang sudah didownload. Kalau misalnya ga ada, ga bakal nampilin apa-apa.
    /// </summary>
    public void SetupAllListOfSermon()
    {
        //Mengosongkan list biar ga ada duplicate
        if (List_SCR_savedSermonItemList == null) List_SCR_savedSermonItemList = new List<UISavedSermonItem>();
        List_SCR_savedSermonItemList.Clear();

        //Menghapus UI lama biar ga ada duplicate
        foreach (Transform TF_child in TF_dictionaryListParent)
        {
            Destroy(TF_child.gameObject);
        }

        //Ngeloop semua renungan yang sudah didownload
        foreach (DialogSO SO_savedDialog in DictionaryManager.Instance.List_SO_allDownloadedDialog)
        {
            GameObject GO_savedSermon = Instantiate(PB_dictionaryListButton, TF_dictionaryListParent);

            UISavedSermonItem SCR_savedSermonItem = GO_savedSermon.GetComponent<UISavedSermonItem>();

            //Setup judul dan dialogSO
            SCR_savedSermonItem.SCR_dialogSO = SO_savedDialog;
            SCR_savedSermonItem.TMPUGUI_title.text = SO_savedDialog.ENM_dialogTitle.ToString();
            SCR_savedSermonItem.IMG_background.color = Color.white;

            List_SCR_savedSermonItemList.Add(SCR_savedSermonItem);
        }

        SCR_currSelectedSermon = null;
    }

    /// <summary>
    /// Membuka isi renungan yang sudah terpilih
    /// </summary>
    public void OpeningSavedSermon()
    {
        //Kalau ga ada renungan yang sedang dipilih, maka ga terjadi apa-apa.
        if (!DictionaryManager.Instance.SO_currDialogSelected) return;

        //Hapus UI chat bubble lama supaya ga tabrakan
        foreach (Transform TF_child in TF_currSavedDialogContainer)
        {
            Destroy(TF_child.gameObject);
        }

        //Aktifkan game object di sini supaya LeanTween ga ngaco dan memberikan error.
        GO_currSavedDialogContainer.SetActive(true);

        //Ini copas dari UIChatManager lol, kurang lebih
        foreach (DialogComponent SCR_components in DictionaryManager.Instance.SO_currDialogSelected.SCR_dialogComponent)
        {
            GameObject GO_chatBubble = Instantiate(UIChatManager.Instance.PB_chatBubble, TF_currSavedDialogContainer);

            UIChatBubble SCR_UIChatBubble = GO_chatBubble.GetComponent<UIChatBubble>();
            
            GO_chatBubble.SetActive(true);

            SCR_UIChatBubble.Setup(SCR_components);

            //Tapi di sini, langsung munculin semuanya. Ga seperti UIChatManager yang satu per satu munculnya.
            SCR_UIChatBubble.Fade(1f);
        }

    }

    /// <summary>
    /// Tutup menu isi renungan
    /// </summary>
    private void CloseSavedSermon()
    {
        GO_currSavedDialogContainer.SetActive(false);
    }

    /// <summary>
    /// Hapus renungan yang sudah dipilih.
    /// </summary>
    public void DeleteSelectedDialog()
    {
        //Pertama, hapus dari list renungan yang sudah didownload
        DictionaryManager.Instance.List_SO_allDownloadedDialog.Remove(DictionaryManager.Instance.SO_currDialogSelected);
        //Lalu, pastikan renungannya tidak diselect lagi
        DictionaryManager.Instance.SO_currDialogSelected = null;
        //Update save file
        DictionaryManager.Instance.SaveToDevice();

        //Hapus renungan yang sudah dipilih dari UI
        Destroy(SCR_currSelectedSermon.gameObject);

        SCR_currSelectedSermon = null;

        //Sembunyikan panel konfirmasi delete karena sudah ga dibutuhin lagi.
        HideDeleteConfirmation();
    }

    /// <summary>
    /// Menunjukkan panel konfirmasi penghapusan renungan. Ini dipanggil pas pencet tombol delete di sebelah renungan yang saat ini sedang dipilih.
    /// </summary>
    public void ShowDeleteConfirmation()
    {
        TMPUGUI_deleteConfirmationText.text = $"Delete {DictionaryManager.Instance.SO_currDialogSelected.ENM_dialogTitle} from list?";
        GO_deleteConfirmationPanel.SetActive(true);
    }

    /// <summary>
    /// Menyembunyikan panel konfirmasi penghapusan renungan. Ini dipanggil pas pencet tombol NO dan juga YES. Kalau NO, dia cuma panggil ini doang. Kalau YES, dia bakal panggil delete, lalu panggil ini.
    /// </summary>
    public void HideDeleteConfirmation()
    {
        GO_deleteConfirmationPanel.SetActive(false);
    }
}
