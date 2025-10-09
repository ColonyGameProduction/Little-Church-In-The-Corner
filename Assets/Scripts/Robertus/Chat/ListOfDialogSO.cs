using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllDialogSO", menuName = "Chat/AllDialogSO")]
public class ListOfDialogSO : ScriptableObject
{
    public List<DialogSO> List_SO_dialogSO;

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

    public DialogSO SO_GetRandomDialogSO()
    {
        int i_randomIndex = UnityEngine.Random.Range(0, List_SO_dialogSO.Count);
        Debug.Log(i_randomIndex);
        Debug.Log(List_SO_dialogSO.Count);

        return List_SO_dialogSO[i_randomIndex];
    }
}
