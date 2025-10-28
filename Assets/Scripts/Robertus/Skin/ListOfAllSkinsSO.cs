using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object untuk menampung semua skin yang ada di dalam game. Skin-skin dibagi berdasarkan ruangan.
/// </summary>
[CreateAssetMenu(fileName = "AllSkinsSO", menuName = "Skin/AllSkinsSO")]
public class ListOfAllSkinsSO : ScriptableObject
{
    public List<SkinTypeSO> List_SO_skinType;

    /// <summary>
    /// Function untuk mendapatkan semua skin yang ada di ruangan tertentu.
    /// </summary>
    /// <param name="ENM_room">Ruangan yang dicari</param>
    /// <returns>Semua skin dari ruangan itu</returns>
    public SkinTypeSO SO_GetSkinTypeSO(ENM_Room ENM_room)
    {
        SkinTypeSO SO_skinType = null;

        foreach (SkinTypeSO SO_skinTypeInList in List_SO_skinType)
        {
            if (SO_skinTypeInList.ENM_skinRoom == ENM_room)
            {
                SO_skinType = SO_skinTypeInList;
            }
        }

        return SO_skinType;
    }
}
