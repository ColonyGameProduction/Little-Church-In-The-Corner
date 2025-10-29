using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tipe-tipe dari skin, dipisah berdasarkan ruangan.
/// </summary>
[CreateAssetMenu(fileName = "SkinType_SKIN TYPE ID", menuName = "Skin/SkinTypeSO")]
public class SkinTypeSO : ScriptableObject
{
    /// <summary>
    /// Tipe ruangan
    /// </summary>
    public ENM_Room ENM_skinRoom;
    /// <summary>
    /// Semua skin yang ada di dalam ruangan, dibagi berdasarkan subtipe (jenis object)
    /// </summary>
    public List<SkinSubtypeSO> List_SO_skinSubType;

    /// <summary>
    /// Function untuk mendapatkan semua skin dengan jenis/subtipe tertentu, misal mendapatkan semua skin dengan subtipe Dinding.
    /// </summary>
    /// <param name="ENM_skinType"></param>
    /// <returns></returns>
    public SkinSubtypeSO SO_GetSkinSubTypeSO(ENM_SkinType ENM_skinType)
    {
        SkinSubtypeSO SO_skinSubtype = null;

        foreach (SkinSubtypeSO SO_skinSubtypeInList in List_SO_skinSubType)
        {
            if (SO_skinSubtypeInList.ENM_skinType == ENM_skinType)
            {
                SO_skinSubtype = SO_skinSubtypeInList;
            }
        }

        return SO_skinSubtype;
    }
}
