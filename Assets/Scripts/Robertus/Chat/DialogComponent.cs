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
    public string S_stringText;
    public AllEnum.ENM_CharFace ENM_charFace;
}
