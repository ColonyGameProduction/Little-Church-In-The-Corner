using System;
using UnityEngine;

/// <summary>
/// Data untuk dialognya, ada sprite background chat bubble, teksnya sendiri, serta muka orang yang bakal ditampilin di samping chat bubble. Also ada boolean untuk posisi muka orang di kotak dialog.
/// </summary>
[Serializable]
public class DialogComponent
{
    public ENM_ChatBubbleBackground ENM_background;
    [TextArea]
    public string S_stringText;
    public ENM_CharFace ENM_charFace;
    /// <summary>
    /// Menunjukkan posisi muka. Kalau false, maka posisi muka di kiri kotak dialog. Kalau true, posisi muka di sebelah kanan dialog.
    /// </summary>
    public bool B_isFlipped;

    public DialogComponent(ENM_ChatBubbleBackground ENM_background, string stringText, ENM_CharFace eNM_charFace, bool B_isFlipped)
    {
        this.ENM_background = ENM_background;
        S_stringText = stringText;
        ENM_charFace = eNM_charFace;
        this.B_isFlipped = B_isFlipped;
    }
}
