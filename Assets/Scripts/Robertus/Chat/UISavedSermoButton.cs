using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Container untuk Game Object renungan yang sudah didownload.
/// </summary>
[RequireComponent(typeof(Button))]
public class UISavedSermoButton : MonoBehaviour
{
    /// <summary>
    /// Judul renungan
    /// </summary>
    public TextMeshProUGUI TMPUGUI_title;
    /// <summary>
    /// Ini tombol untuk select renungan yang mau dibaca. Ini bakal ngecover seluruh kotak di dalam UI (kotak itu yang ada judul renungan, tombol favorit, dst.)
    /// </summary>
    private Button BTN_selectSermon;
    /// <summary>
    /// DialogSO yang ada di game object renungan yang sudah didownload/.
    /// </summary>
    [HideInInspector] public DialogSO SCR_dialogSO;
    /// <summary>
    /// Kalau diselect, warna backgroundnya bakal berubah
    /// </summary>
    public Image IMG_background;
    /// <summary>
    /// Warna yang bakal dipakai untuk renungan yang sedang diseleksi. Defaultnya abu-abu.
    /// </summary>
    public Color COL_selectedColor = Color.gray;

    /// <summary>
    /// Tombol delete renungan. Ini bakal menampilkan panel konfirmasi terlebih dahulu. Also, ini juga hanya muncul kalau renungan diseleksi.
    /// </summary>
    public Button BTN_deleteButton;
    /// <summary>
    /// Tombol favorit renungan. Untuk sekarang, ini hanya visual saja. Ga tau ke depannya bakal ada fungsi lain atau tidak.
    /// </summary>
    public Button BTN_favoriteButton;

    private void Awake()
    {
        BTN_selectSermon = GetComponent<Button>();
    }

    private void OnEnable()
    {
        BTN_selectSermon.onClick.AddListener(SelectSermon);
        BTN_deleteButton.onClick.AddListener(DeleteSermon);
        BTN_favoriteButton.onClick.AddListener(FavoriteSermon);
    }

    private void OnDisable()
    {
        BTN_selectSermon.onClick.RemoveListener(SelectSermon);
        BTN_deleteButton.onClick.RemoveListener(DeleteSermon);
        BTN_favoriteButton.onClick.RemoveListener(FavoriteSermon);
    }

    /// <summary>
    /// Kalau salah satu renungan dipencet, maka function ini bakal dipanggil.
    /// </summary>
    private void SelectSermon()
    {
        //Pertama-tama, deselect semuanya supaya semuanya kembali ke default.
        DictionaryManager.Instance.SCR_UIDictionary.DeselectAllSavedSermon();
        //Lalu, atur supaya renungan yang saat ini diseleksi adalah renungan ini.
        DictionaryManager.Instance.SO_currDialogSelected = SCR_dialogSO;
        DictionaryManager.Instance.SCR_UIDictionary.SCR_currSelectedSermon = this;
        //Lalu ubah UInya
        IMG_background.color = COL_selectedColor;
        BTN_deleteButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Ini dipanggil saat tombol delete di sebelah renungan dipencet.
    /// </summary>
    private void DeleteSermon()
    {
        DictionaryManager.Instance.SCR_UIDictionary.ShowDeleteConfirmation();
    }

    /// <summary>
    /// Ini dipanggil saat tombol favorit di sebelah renungan dipencet.
    /// </summary>
    private void FavoriteSermon()
    {
        //Bolak-balik antara warna asli (white) dan warna silhouette (hitam)
        BTN_favoriteButton.image.color = BTN_favoriteButton.image.color == Color.white ? Color.black : Color.white;
    }
}
