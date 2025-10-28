using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NOTE: INI HANYA UNTUK TESTING DOANG, BUKAN FINAL.
/// Ini adalah class untuk menampung data tombol testing gonta-ganti skin, makanya agak acak-acakan dan ga sesuai guidelines.
/// Nanti kalau udah final, baru dirapiin dan dipindah ke class lain.
/// </summary>
public class TestChangeSkinButton : MonoBehaviour
{
    public Image background;
    /// <summary>
    /// Border bakal aktif kalau misalnya tombolnya merupakan tombol skin yang saat ini dipakai pemain.
    /// </summary>
    public GameObject border;
    public Button button;
    public TextMeshProUGUI buttonText;
    public Skin skin;
    public ENM_SkinType skinType;

    /// <summary>
    /// Ada ini untuk memanggil function ganti skin.
    /// </summary>
    private TestChangeSkin SCR_testChangeSkinReference;

    private void OnEnable()
    {
        if (skin != null) button.onClick.AddListener(ChangeSkin);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Function ini untuk setup data-data yang ada di tombol, seperti background, tulisan, border, dan data lainnya yang mungkin ga kelihatan.
    /// </summary>
    /// <param name="backgroundSprite">Sprite dari background, diambil dari sprite atau texture dalam skin</param>
    /// <param name="text">Teks yang bakal ditampilkan di tombol, biasanya berupa enum dari skin itu</param>
    /// <param name="skin">Data skin untuk disimpan, biar bisa digunakan pas mau ganti skin</param>
    /// <param name="skinType">Tipe skin, untuk digunakan pas mau ganti skin</param>
    /// <param name="testChangeSkin">Ini hanya referensi ke class yang memiliki function untuk ganti skin</param>
    /// <param name="activeStatus">Status keaktifan dari tombolnya</param>
    public void Setup(Sprite backgroundSprite, string text, Skin skin, ENM_SkinType skinType, TestChangeSkin testChangeSkin, bool activeStatus)
    {
        background.sprite = backgroundSprite;
        buttonText.text = text;
        this.skin = skin;
        this.skinType = skinType;

        SCR_testChangeSkinReference = testChangeSkin;

        SetAsActiveButton(activeStatus);

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ChangeSkin);
    }

    /// <summary>
    /// Function untuk ganti skin. Ini juga bakal membuat tombolnya menjadi aktif.
    /// </summary>
    private void ChangeSkin()
    {
        SCR_testChangeSkinReference.ReplaceActiveSkinStatus(skinType, this);
        SCR_testChangeSkinReference.ChangeSkin(skin, skinType);
    }

    /// <summary>
    /// Function untuk ganti keaktifan dari tombolnya
    /// </summary>
    /// <param name="status"></param>
    public void SetAsActiveButton(bool status)
    {
        border.SetActive(status);
    }
}
