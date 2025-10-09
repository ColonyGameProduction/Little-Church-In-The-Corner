using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ListOfFace
{
    public List<Face> List_face;

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
