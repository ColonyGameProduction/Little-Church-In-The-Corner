using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// NOTE: INI HANYA UNTUK TESTING DOANG, BUKAN FINAL.
/// Ini adalah class untuk testing gonta-ganti skin, makanya agak acak-acakan dan ga sesuai guidelines.
/// Nanti kalau udah final, baru dirapiin dan dipindah ke class lain.
/// </summary>
public class TestChangeSkin : MonoBehaviour
{
    public ListOfAllSkinsSO SCR_listOfAllSkinsSO;
    /// <summary>
    /// Ini dictionary untuk menampung tombol untuk ubah skin YANG AKTIF dalam tipe skin tertentu. Misal, pemain memilih skin Floor 3, maka dictionary ini akan menampung tombol Floor 3 dengan key ENM_SkinType "Floor".
    /// </summary>
    private Dictionary<ENM_SkinType, TestChangeSkinButton> currentButtonsActive;

    [Header("Unity References")]
    // Ini parent untuk testing UI. Di dalam UIParent, semua tombol akan dimunculkan.
    public Transform UIParent;
    /// <summary>
    /// Game object dinding yang ada di scene. Dinding bakal cuma ganti texture doang.
    /// </summary>
    public GameObject wallGO;
    /// <summary>
    /// Game object lantai yang ada di scene. Lantai bakal cuma ganti texture doang.
    /// </summary>
    public GameObject floorGO;
    /// <summary>
    /// Ini contoh penggunaan skin kalau misalnya modelnya berbeda-beda. Dia pakai parent, jadi setiap kali mau ganti skin, game object lama yang ada di dalam parent bakal dihapus dan diganti dengan game object skin baru.
    /// </summary>
    public Transform testObjectParent;

    [Header("Prefabs")]
    // Ini cuma untuk UI testing doang, ga terlalu penting. Ini untuk menunjukkan ruangan-ruangan saja.
    public GameObject headingTextPrefab;
    // Ini cuma untuk UI testing doang, ga terlalu penting. Ini untuk menunjukkan tipe skin saja, misal dinding atau lantai.
    public GameObject subheadingTextPrefab;
    /// <summary>
    /// Ini prefab kosong yang mengandung komponen Grid Layout Group, sehingga tombol-tombol ganti skin bakal dimasukin di dalamnya biar tertata rapi.
    /// </summary>
    public GameObject buttonsGridPrefab;
    /// <summary>
    /// Prefab untuk tombol ganti skin.
    /// </summary>
    public GameObject buttonPrefab;

    private void Start()
    {
        //ChangeSkin(SCR_listOfAllSkinsSO.List_SO_skinType[0].List_SO_skinSubType[0].List_SCR_skin[0], wallGO);
        SetupStartingSkin();
        SetupTestUI();
    }

    /// <summary>
    /// Ini function untuk menyiapkan skin awal yang bakal digunakan pemain.
    /// Untuk sekarang, setiap kali ngeload game, skin awalnya adalah skin pertama dalam list.
    /// Tapi, ke depannya mungkin bisa diganti dengan sistem save load.
    /// </summary>
    private void SetupStartingSkin()
    {
        foreach (SkinTypeSO skinType in SCR_listOfAllSkinsSO.List_SO_skinType)
        {
            foreach (SkinSubtypeSO skinSubtype in skinType.List_SO_skinSubType)
            {
                //Kalau misalnya ada skin di dalam list, maka atur supaya skin yang saat itu dipakai adalah skin pertama.
                if(skinSubtype.List_SCR_skin.Count > 0) skinSubtype.SCR_currEquippedSkin = skinSubtype.List_SCR_skin[0];

                //Lalu, ganti beneran di scenenya biar kelihatan skin yang dipakai saat itu.
                switch (skinSubtype.ENM_skinType)
                {
                    case ENM_SkinType.Floor:
                        ChangeSkin(skinSubtype.SCR_currEquippedSkin, floorGO);
                        break;
                    case ENM_SkinType.Wall:
                        ChangeSkin(skinSubtype.SCR_currEquippedSkin, wallGO);
                        break;
                    case ENM_SkinType.ObjectTest:
                        ChangeSkin(skinSubtype.SCR_currEquippedSkin, testObjectParent);
                        break;
                    default:
                        //Ini kalau misalnya ternyata ada tipe skin baru yang belum diimplement di code.
                        Debug.Log("Not implemented yet");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Ini cuma untuk generate UI testing.
    /// Dia bakal ngeloop semua skin yang ada dan membuat tombol-tombol biar bisa ganti-ganti skin.
    /// </summary>
    private void SetupTestUI()
    {
        currentButtonsActive = new Dictionary<ENM_SkinType, TestChangeSkinButton>();

        foreach (SkinTypeSO skinType in SCR_listOfAllSkinsSO.List_SO_skinType)
        {
            //Setiap skinType (ruangan) bakal ada heading untuk menunjukkan skin itu untuk ruangan yang mana.
            GameObject headingGO = Instantiate(headingTextPrefab, UIParent);
            headingGO.GetComponent<TextMeshProUGUI>().text = skinType.ENM_skinRoom.ToString();

            foreach (SkinSubtypeSO skinSubtype in skinType.List_SO_skinSubType)
            {
                //Setiap skinSubtype (subtipe object) bakal ada heading untuk menunjukkan skin itu untuk object yang mana, seperti dinding, meja, dst.
                GameObject subheadingGO = Instantiate(subheadingTextPrefab, UIParent);
                subheadingGO.GetComponent<TextMeshProUGUI>().text = skinSubtype.ENM_skinType.ToString();

                //Siapin grid untuk tombol-tombol
                GameObject buttonsGridGO = Instantiate(buttonsGridPrefab, UIParent);

                foreach (Skin skin in skinSubtype.List_SCR_skin)
                {
                    //Bikin tombol ganti skin baru dan masukin ke dalam grid
                    GameObject skinGO = Instantiate(buttonPrefab, buttonsGridGO.transform);

                    //Teks yang ada di tombol adalah nama skin itu (tergantung dari enumnya)
                    string buttonText = skin.ENM_skinItem.ToString();
                    Sprite buttonBackgroundSprite = null;

                    //Sementara background dari tombol adalah sprite atau texture (kalau ada). Kalau ga ada, dia bakal jadi putih doang.
                    if (skin.SPR_skinSprite != null)
                    {
                        buttonBackgroundSprite = skin.SPR_skinSprite;
                    }
                    else if(skin.TEX_skinTexture != null)
                    {
                        //Ini untuk membuat sprite baru berdasarkan texture, soalnya Image butuh sprite, sementara texture itu asalnya dari 3D model
                        Rect rec = new Rect(0, 0, skin.TEX_skinTexture.width, skin.TEX_skinTexture.height);
                        Sprite newSprite = Sprite.Create((Texture2D)skin.TEX_skinTexture, rec, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

                        buttonBackgroundSprite = newSprite;
                    }

                    //Beneran setup tombol ganti skinnya.
                    TestChangeSkinButton SCR_button = skinGO.GetComponent<TestChangeSkinButton>();

                    //Kalau misalnya ternyata skinnya adalah skin yang sedang dipakai, maka tambahin "border" selected di sekitar tombol ganti skinnya.
                    bool currentSkin = false;
                    if (skinSubtype.SCR_currEquippedSkin.ENM_skinItem == skin.ENM_skinItem)
                    {
                        currentSkin = true;
                        //Also tambahin ke dictionary
                        currentButtonsActive.TryAdd(skinSubtype.ENM_skinType, SCR_button);
                    }

                    SCR_button.Setup(buttonBackgroundSprite, buttonText, skin, skinSubtype.ENM_skinType, this, currentSkin);
                }
            }
        }
    }

    /// <summary>
    /// Function untuk ganti skin. Ini yang bakal dipanggil di tombol-tombol.
    /// </summary>
    /// <param name="skin">Skin yang bakal menggantikan</param>
    /// <param name="type">Tipe skin</param>
    public void ChangeSkin(Skin skin, ENM_SkinType type)
    {
        switch (type)
        {
            case ENM_SkinType.Floor:
                ChangeSkin(skin, floorGO);
                break;
            case ENM_SkinType.Wall:
                ChangeSkin(skin, wallGO);
                break;
            case ENM_SkinType.ObjectTest:
                ChangeSkin(skin, testObjectParent);
                break;
            default:
                Debug.Log("Not implemented yet");
                break;
        }
    }

    /// <summary>
    /// Ini untuk menukar status aktif untuk tombol-tombol ganti skin. Misal pemain awalnya pakai skin Floor 1, maka tombol Floor 1 adalah tombol ynag aktif. Kalau pemain mau ganti ke Floor 4, maka tombol Floor 1 bakal dinonaktifkan dan tombol Floor 4 akan diaktifkan.
    /// Maksud dari "Aktif" adalah adanya border berwarna di sekitar tombolnya. Yes, ini cuma visual doang.
    /// </summary>
    /// <param name="skinType"></param>
    /// <param name="button"></param>
    public void ReplaceActiveSkinStatus(ENM_SkinType skinType, TestChangeSkinButton button)
    {
        if (currentButtonsActive.TryGetValue(skinType, out TestChangeSkinButton currentButtonActive))
        {
            currentButtonActive.SetAsActiveButton(false);
            currentButtonsActive[skinType] = button;
            button.SetAsActiveButton(true);
        }
    }

    /// <summary>
    /// Ini function untuk ganti skin yang ada di game object tertentu.
    /// </summary>
    /// <param name="skin">Skin yang akan menggantikan</param>
    /// <param name="targetGO">Game object yang bakal memiliki skin itu</param>
    public void ChangeSkin(Skin skin, GameObject targetGO)
    {
        //Kalau game object targetGO sama dengan game object yang ada di skin, dalam arti 3D Model dan Materialnya sama, berarti cuma ganti texture material doang
        if (CompareSkinGO(skin, targetGO))
        {
            //Kalau misalnya skinnya ga punya texture, somehow, maka jangan lakukan apa-apa. A.k.a skinnya ga bakal keganti.
            if (skin.TEX_skinTexture == null)
            {
                Debug.Log("Skin texture is null! Don't do anything.");
                return;
            }

            MeshRenderer meshRenderer = targetGO.GetComponent<MeshRenderer>();
            //Ini bakal mengubah texture dari material yang ada di targetGO menjadi seperti yang di skin.
            meshRenderer.sharedMaterial.SetTexture("_BaseMap", skin.TEX_skinTexture);
        }
        else
        {
            Debug.Log("Replace!");
            //Ternyata targetGO dan game object di dalam skin berbeda, berarti ganti game object juga, ditukar ama yang ada di skin.
            SwitchGameObjects(targetGO, skin.GO_skinObject);
        }
    }

    /// <summary>
    /// Mirip seperti function di atas, tapi untuk parent tertentu.
    /// Ini ada karena terkadang function untuk replace game objectnya ngebug, jadi pakai transform parent
    /// </summary>
    /// <param name="skin">Skin yang akan menggantikan</param>
    /// <param name="targetGOParent">Parent Game object yang bakal menampung game object dari skin itu</param>
    public void ChangeSkin(Skin skin, Transform targetGOParent)
    {
        //Di dalam parent harus ada game object terlebih dahulu, karena dia bakal compare game objectnya dengan yang ada di skin.
        if (targetGOParent.childCount <= 0)
        {
            Debug.Log("WARNING: Parent has no child!");
            return;
        }
        GameObject targetGO = targetGOParent.GetChild(0).gameObject;

        //Sisanya sama seperti function yang di atas.
        if (CompareSkinGO(skin, targetGO))
        {
            if (skin.TEX_skinTexture == null)
            {
                Debug.Log("Skin texture is null! Don't do anything.");
                return;
            }

            MeshRenderer meshRenderer = targetGO.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial.SetTexture("_BaseMap", skin.TEX_skinTexture);
        }
        else
        {
            Debug.Log("Replace!");
            SwitchGameObjects(targetGO, skin.GO_skinObject);
        }
    }

    /// <summary>
    /// Tukar game object dari lama ke baru. Ini digunakan misalnya untuk ganti skin dengan 3D model yang berbeda.
    /// </summary>
    /// <param name="oldGO">Game object lama yang bakal didelete</param>
    /// <param name="newGO">Game object baru yang bakal menggantikan game object lama</param>
    private void SwitchGameObjects(GameObject oldGO, GameObject newGO)
    {
        //Game object baru bakal memiliki transform yang parent yang sama dengan game object lama.
        GameObject replacement = Instantiate(newGO, oldGO.transform.parent);

        replacement.transform.position = oldGO.transform.position;
        replacement.transform.rotation = oldGO.transform.rotation;
        replacement.transform.localScale = oldGO.transform.localScale;

        replacement.transform.SetSiblingIndex(oldGO.transform.GetSiblingIndex());

        Destroy(oldGO);
    }

    /// <summary>
    /// Membandingkan game object yang ada di dalam skin dengan game object target.
    /// Yang dibandingkan adalah mesh dan material pertama yang ada di dalam setiap game object.
    /// Kalau ga ada salah satu dari mereka (mesh/material), maka dia bakal return false.
    /// Kalau misalnya ada keduanya, tapi ternyata mesh/materialnya berbeda, dia bakal return false.
    /// Tapi, kalau semuanya sama, maka dia bakal return true.
    /// </summary>
    /// <param name="skin">Skin</param>
    /// <param name="targetGO">Game object untuk dibandingkan</param>
    /// <returns>Kalau ternyata data skinnya berbeda, return false. Else, return true</returns>
    private bool CompareSkinGO(Skin skin, GameObject targetGO)
    {
        MeshFilter meshFilter = null;
        MeshFilter skinMeshFilter = null;
        MeshRenderer meshRenderer = null;
        MeshRenderer skinMeshRenderer = null;

        if (targetGO.TryGetComponent(out meshFilter) &&
            skin.GO_skinObject.TryGetComponent(out skinMeshFilter))
        {
            //Debug.Log("Got mesh filters");
            if (meshFilter.sharedMesh == skinMeshFilter.sharedMesh)
            {
                //Debug.Log("Mesh are the same");
                if (targetGO.TryGetComponent(out meshRenderer) &&
                    skin.GO_skinObject.TryGetComponent(out skinMeshRenderer))
                {
                    //Debug.Log("Got mesh renderers");
                    if (meshRenderer.sharedMaterial == skinMeshRenderer.sharedMaterial)
                    {
                        //Debug.Log("Materials are the same");
                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else return false;
        }
        else return false;
    }
}
