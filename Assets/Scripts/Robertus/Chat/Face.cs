using System;
using UnityEngine;

/// <summary>
/// Data untuk muka karakter. Isinya ada enum sebagai ID dari karakter dan juga sprite dari muka karakter.
/// </summary>
[Serializable]
public class Face
{
    public ENM_CharFace ENM_characterFace;
    public Sprite SPR_characterSprite;
}
