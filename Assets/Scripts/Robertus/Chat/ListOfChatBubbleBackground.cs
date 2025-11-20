using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List semua background yang dapat dipakai oleh sebuah chat bubble.
/// </summary>
[Serializable]
public class ListOfChatBubbleBackground
{
    public List<ChatBubbleBackground> List_backgrounds;

    /// <summary>
    /// Ambil background chat bubble berdasarkan ID background chat bubblenya.
    /// </summary>
    /// <param name="ENM_chatBubbleBackground">Enum/ID background chat bubble yang ingin diambil</param>
    /// <returns>Data dari background chat bubble</returns>
    public ChatBubbleBackground SCR_GetChatBubbleBackground(ENM_ChatBubbleBackground ENM_chatBubbleBackground)
    {
        ChatBubbleBackground backgroundResult = null;

        foreach (ChatBubbleBackground background in List_backgrounds)
        {
            if (background.ENM_chatBubbleBackground == ENM_chatBubbleBackground)
            {
                backgroundResult = background;
            }
        }

        return backgroundResult;
    }
}
