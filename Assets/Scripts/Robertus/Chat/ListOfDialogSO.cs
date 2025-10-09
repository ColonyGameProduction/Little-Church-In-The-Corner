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
    public List<DialogSO> List_SO_dialogSO;

    /// <summary>
    /// Ambil dialog tertentu berdasarkan title dari dialog tersebut
    /// </summary>
    /// <param name="ENM_dialogTitle">Title dari dialog</param>
    /// <returns>DialogSO</returns>
    public DialogSO SO_GetDialogSO(AllEnum.ENM_DialogTitle ENM_dialogTitle)
    {
        DialogSO SO_dialogSO = null;

        foreach (DialogSO dialogSO in List_SO_dialogSO)
        {
            if(dialogSO.ENM_dialogTitle == ENM_dialogTitle)
            {
                SO_dialogSO = dialogSO;
            }
        }
        
        return SO_dialogSO;
    }

    /// <summary>
    /// Ambil dialog dari list semua dialog secara acak.
    /// </summary>
    /// <returns></returns>
    public DialogSO SO_GetRandomDialogSO()
    {
        int I_randomIndex = UnityEngine.Random.Range(0, List_SO_dialogSO.Count);
        //Debug.Log(I_randomIndex);
        //Debug.Log(List_SO_dialogSO.Count);

        return List_SO_dialogSO[I_randomIndex];
    }
}
