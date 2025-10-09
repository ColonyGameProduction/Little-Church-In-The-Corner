using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog_JUDUL_ENUM", menuName = "Chat/DialogSO")]
public class DialogSO : ScriptableObject
{
    public AllEnum.ENM_DialogTitle ENM_dialogTitle;
    public List<DialogComponent> SCR_dialogComponent;

    //Aku ga tau ini buat apa
    public DialogComponent SCR_GetDialogueComponent()
    {
        throw new NotImplementedException("Get Dialogue Component NOT IMPLEMENTED");
    }
}
