using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void Setup(DialogComponent SCR_dialogComponent)
    {
        Fade(0f);

        IMG_chatBubble.color = SCR_dialogComponent.COL_bubbleColour;
        TMPUGUI_chatBubble.text = SCR_dialogComponent.S_stringText;
        IMG_face.sprite = ChatManager.Instance.SCR_listOfFace.SCR_GetFace(SCR_dialogComponent.ENM_charFace).SPR_characterSprite;
    }

    public void Fade(float F_alpha)
    {
        CG_chatBubble.alpha = F_alpha;
    }
}
