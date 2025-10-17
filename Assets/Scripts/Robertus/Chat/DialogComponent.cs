using System;
using UnityEngine;

/// <summary>
/// Data untuk dialognya, ada warna chat bubble, teksnya sendiri, serta muka orang yang bakal ditampilin di samping chat bubble.
/// </summary>
[Serializable]
public class DialogComponent
{
    //Warna defaultnya putih.
    public Color COL_bubbleColour = Color.white;
    [TextArea]
    public string S_stringText;
    public ENM_CharFace ENM_charFace;

    public DialogComponent(Color cOL_bubbleColour, string stringText, ENM_CharFace eNM_charFace)
    {
        COL_bubbleColour = cOL_bubbleColour;
        S_stringText = stringText;
        ENM_charFace = eNM_charFace;
    }
}
