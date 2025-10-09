using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Container untuk UI chat bubble. Isinya referensi-referensi komponen Unity yang diperlukan untuk UI chat bubble.
/// Require component Canvas Group karena ini letaknya di prefab Chat Bubble. Canvas Group juga letaknya di prefab Chat Bubble supaya kalau alpha dari Canvas Group diatur, dia bakal ngefek ke semua child game object yang ada di dalamnya.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIChatBubble : MonoBehaviour
{
    public Image IMG_chatBubble;
    public Image IMG_face;
    public TextMeshProUGUI TMPUGUI_chatBubble;

    private CanvasGroup CG_chatBubble;

    private void Awake()
    {
        CG_chatBubble = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Mengatur UI chat bubble berdasarkan data dari DialogComponent.
    /// </summary>
    /// <param name="SCR_dialogComponent">Data Dialog</param>
    public void Setup(DialogComponent SCR_dialogComponent)
    {
        //Pertama-tama dihide dulu soalnya ga semua dialog bakal dimunculin di awal. Dialog-dialog bakal dimunculin perlahan-lahan. Jadi, muncul atau engga bakal diatur di UIChatManager nantinya.
        Fade(0f);

        IMG_chatBubble.color = SCR_dialogComponent.COL_bubbleColour;
        TMPUGUI_chatBubble.text = SCR_dialogComponent.S_stringText;
        IMG_face.sprite = ChatManager.Instance.SCR_listOfFace.SCR_GetFace(SCR_dialogComponent.ENM_charFace).SPR_characterSprite;
    }

    /// <summary>
    /// Buat ngefade seluruh chat bubble, seperti teks, chat bubble, dan muka orang.
    /// </summary>
    /// <param name="F_alpha">Nilai dari 0-1. 0 itu transparan/hilang, 1 itu kelihatan 100%</param>
    public void Fade(float F_alpha)
    {
        CG_chatBubble.alpha = F_alpha;
    }
}
