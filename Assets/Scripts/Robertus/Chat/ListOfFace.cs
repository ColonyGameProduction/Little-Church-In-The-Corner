using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List semua muka karakter dalam game.
/// </summary>
[Serializable]
public class ListOfFace
{
    public List<Face> List_face;

    /// <summary>
    /// Ambil muka karakter berdasarkan ID muka karakternya.
    /// </summary>
    /// <param name="ENM_charFace">Enum/ID muka karakter yang ingin diambil</param>
    /// <returns>Data dari muka karakter</returns>
    public Face SCR_GetFace(AllEnum.ENM_CharFace ENM_charFace)
    {
        Face faceResult = null;

        foreach (Face face in List_face)
        {
            if (face.ENM_characterFace == ENM_charFace)
            {
                faceResult = face;
            }
        }

        return faceResult;
    }
}
