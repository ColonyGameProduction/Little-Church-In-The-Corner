using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Berisi list semua dialog yang ada
/// Ada juga function-function buat ambil dialog berdasarkan title dan dialog random dari list
/// </summary>
[CreateAssetMenu(fileName = "AllDialogSO", menuName = "Chat/AllDialogSO")]
public class ListOfDialogSO : ScriptableObject
{
    public List<DialogTypeSO> List_SO_dialogTypeSO;


    /// <summary>
    /// Ambil dialog di ruangan tertentu. Kalau dialog title none, maka dia bakal ambil random dialog.
    /// </summary>
    /// <param name="ENM_room">Renungan dari ruangan mana</param>
    /// <param name="ENM_dialogTitle">Judul renungan kalau ada</param>
    /// <returns>DialogSO</returns>
    public DialogSO SO_GetDialogSO(ENM_Room ENM_room, ENM_DialogTitle ENM_dialogTitle = ENM_DialogTitle.None)
    {
        DialogSO SO_dialogSO = null;

        foreach (DialogTypeSO dialogTypeSO in List_SO_dialogTypeSO)
        {
            if(dialogTypeSO.ENM_dialogType == ENM_room)
            {
                if (ENM_dialogTitle == ENM_DialogTitle.None)
                    SO_dialogSO = dialogTypeSO.SO_GetRandomDialogSO();
                else
                    SO_dialogSO = dialogTypeSO.SO_GetDialogSO(ENM_dialogTitle);
                break;
            }
        }
        
        return SO_dialogSO;
    }
}
