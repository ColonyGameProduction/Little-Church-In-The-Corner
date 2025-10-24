using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object untuk dialog type. Ini untuk menyimpan renungan-renungan di dalam suatu ruangan tertentu.
/// </summary>
[CreateAssetMenu(fileName = "DialogType_RUANGAN", menuName = "Chat/DialogTypeSO")]
public class DialogTypeSO : ScriptableObject
{
    public List<DialogSO> List_SO_dialogSO;
    public ENM_Room ENM_dialogType;

    /// <summary>
    /// Ambil dialog tertentu berdasarkan title dari dialog tersebut
    /// </summary>
    /// <param name="ENM_dialogTitle">Title dari dialog</param>
    /// <returns>DialogSO</returns>
    public DialogSO SO_GetDialogSO(ENM_DialogTitle ENM_dialogTitle)
    {
        DialogSO SO_dialogSO = null;

        foreach (DialogSO dialogSO in List_SO_dialogSO)
        {
            if (dialogSO.ENM_dialogTitle == ENM_dialogTitle)
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
