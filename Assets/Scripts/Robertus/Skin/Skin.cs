using System;
using UnityEngine;

/// <summary>
/// Class untuk menampung semua data dari skin.
/// </summary>
[Serializable]
public class Skin
{
    /// <summary>
    /// Game object dari skin.
    /// ??????????????????????????????????????????????????????
    /// Kayaknya ini lebih ke prefab? Kalau aku sih implementnya, GO_skinObject bakal gantiin object yang ada di scene saat ini
    /// - Robert
    /// </summary>
    public GameObject GO_skinObject;
    /// <summary>
    /// Ini aku ga tau buat apa, soalnya kurang lebih semua hal yang ada skinnya itu 3D, bukan 2D
    /// - Robert
    /// </summary>
    public Sprite SPR_skinSprite;
    /// <summary>
    /// Texture skinnya. Kalau misalnya ternyata object 3Dnya cuma butuh ganti texture, maka ini yang dipakai (misal, ganti wallpaper dari dinding. Ga perlu ganti seluruh dindingnya, cuma texturenya doang)
    /// </summary>
    public Texture TEX_skinTexture;
    /// <summary>
    /// TODO: implement ini kalau udah ada bahan animasi yang bisa diotak-atik.
    /// </summary>
    public Animation ANM_activityAnimation;
    /// <summary>
    /// TODO: implement ini barengan dengan Market Manager
    /// </summary>
    public int I_price;
    /// <summary>
    /// TODO: implement ini barengan dengan Market Manager
    /// </summary>
    public bool B_inInventory;
    /// <summary>
    /// Enum dari skin. Selalu unik. Jangan lupa tambahin di AllEnum kalau ada skin baru.
    /// </summary>
    public ENM_SkinItem ENM_skinItem;
}
