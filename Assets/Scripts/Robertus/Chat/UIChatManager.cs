using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatManager : MonoBehaviour
{
    public static UIChatManager Instance { get; private set; }

    public GameObject PB_chatBubble;
    public List<UIChatBubble> List_chatBubble;
    public Transform TF_chatBubbleParent;

    public int I_maxChatBubble = 2;
    public int I_amountOfFadedChatBubbles = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        ChatManager.ACT_PlayDialogue += SetupNextChatBubble;
    }

    private void OnDisable()
    {
        ChatManager.ACT_PlayDialogue -= SetupNextChatBubble;
    }

    //Dipanggil pas click interact, seharusnya
    //Buat setup semua chat bubble, jadi nanti tinggal dienable aja kalau mau munculin
    public void SetupAllChats()
    {
        if (List_chatBubble == null) List_chatBubble = new List<UIChatBubble>();
        List_chatBubble.Clear();

        //Clears all game objects from chat bubble parent
        foreach (Transform child in TF_chatBubbleParent)
        {
            Destroy(child.gameObject);
        }

        DialogSO SO_dialogSO = ChatManager.Instance.SO_currDialog;

        foreach (DialogComponent components in SO_dialogSO.SCR_dialogComponent)
        {
            GameObject GO_chatBubble = Instantiate(PB_chatBubble, TF_chatBubbleParent);

            UIChatBubble SCR_UIChatBubble = GO_chatBubble.GetComponent<UIChatBubble>();

            SCR_UIChatBubble.Setup(components);

            GO_chatBubble.SetActive(false);

            List_chatBubble.Add(SCR_UIChatBubble);
        }

        SetupChatBubble(0, 1f);
    }

    private void SetupNextChatBubble(int I_currentIndex)
    {
        if(I_currentIndex < List_chatBubble.Count)
        {
            SetupChatBubble(I_currentIndex, 1f);
            SetupPreviousChatBubbles(I_currentIndex);
        }
    }

    private void SetupChatBubble(int I_index, float F_alpha)
    {
        List_chatBubble[I_index].gameObject.SetActive(true);
        List_chatBubble[I_index].Fade(F_alpha);
    }

    private void SetupPreviousChatBubbles(int I_currentIndex)
    {
        for (int i = 0; i < List_chatBubble.Count; i++)
        {
            //Kalau i melebihi current index, break soalnya dialognya lom dimunculin.
            if (i > I_currentIndex)
            {
                Debug.Log($"{i}. Reached the end of setup");
                break;
            }

            //Kalau chat bubblenya udah lama banget, disable
            if (I_currentIndex - i >= I_maxChatBubble + I_amountOfFadedChatBubbles)
            {
                SetupChatBubble(i, 0f);
                List_chatBubble[i].gameObject.SetActive(false);
                Debug.Log($"{i} chat has expired, disabling");
                continue;
            }

            //Kalau chat bubblenya baru dan belum melebihi max chat bubble, maka transparansinya 1
            if (I_currentIndex - i < I_maxChatBubble)
            {
                SetupChatBubble(i, 1f);
                Debug.Log($"{i} chat is new, alpha at 1");
                continue;
            }

            //Kalau chat bubblenya udah agak lama, mulai fade
            float F_alpha = 1 - ((I_currentIndex - i + 1.0f - I_maxChatBubble) / (I_amountOfFadedChatBubbles + 1.0f));
            SetupChatBubble(i, F_alpha);
            Debug.Log($"{i} chat is fading, alpha at {F_alpha}");
        }
    }

    public void AnimateChatBubble()
    {
        //mungkin ini buat animasi teks? typewriter atau semacamnya
    }
}
