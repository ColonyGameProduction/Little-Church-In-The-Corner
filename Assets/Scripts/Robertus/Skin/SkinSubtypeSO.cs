using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subtipe dari skin. Kalau tipe biasa adalah per ruangan, ini adalah per object (misal, dinding, lantai, lemari, meja, dst.)
/// </summary>
[CreateAssetMenu(fileName = "SkinSubtype_SKIN SUBTYPE ID", menuName = "Skin/SkinSubtypeSO")]
public class SkinSubtypeSO : ScriptableObject
{
    /// <summary>
    /// Enum dari tipe skin, seperti Dinding, Lantai, dst.
    /// </summary>
    public ENM_SkinType ENM_skinType;
    /// <summary>
    /// Semua skin yang ada di kategori itu, misal semua skin dinding, atau semua skin meja.
    /// </summary>
    public List<Skin> List_SCR_skin;
    /// <summary>
    /// Skin yang saat ini digunakan oleh pemain.
    /// </summary>
    public Skin SCR_currEquippedSkin;

    /// <summary>
    /// Function untuk mendapatkan skin tertentu berdasarkan enum skinnya.
    /// </summary>
    /// <param name="ENM_skinItem">Enum dari skin spesifik itu</param>
    /// <returns>Skin yang sesuai dengan enum. Kalau ga ada, bakal return null.</returns>
    public Skin SCR_GetSkin(ENM_SkinItem ENM_skinItem)
    {
        Skin SCR_skin = null;

        foreach (Skin SCR_skinInList in List_SCR_skin)
        {
            if (SCR_skinInList.ENM_skinItem == ENM_skinItem)
            {
                SCR_skin = SCR_skinInList;
            }
        }

        return SCR_skin;
    }
}
