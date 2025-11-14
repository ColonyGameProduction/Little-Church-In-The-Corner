using System;
using UnityEngine;

/// <summary>
/// Data untuk dialognya, ada warna chat bubble, teksnya sendiri, serta muka orang yang bakal ditampilin di samping chat bubble.
/// </summary>
[Serializable]
public class DialogComponent
{
    //Warna defaultnya putih.
    //public Color COL_bubbleColour = Color.white;
    public Sprite SPR_background;
    [TextArea]
    public string S_stringText;
    public ENM_CharFace ENM_charFace;
    public bool B_isFlipped;

    public DialogComponent(Sprite SPR_background, string stringText, ENM_CharFace eNM_charFace, bool B_isFlipped)
    {
        this.SPR_background = SPR_background;
        S_stringText = stringText;
        ENM_charFace = eNM_charFace;
        this.B_isFlipped = B_isFlipped;
    }
}
