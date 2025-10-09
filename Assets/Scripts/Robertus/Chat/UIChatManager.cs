using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengatur seluruh hal mengenai UI dari Chat, seperti menampilkan chat bubble dan animasi dari chat bubble.
/// </summary>
public class UIChatManager : MonoBehaviour
{
    public static UIChatManager Instance { get; private set; }

    public GameObject PB_chatBubble;
    public List<UIChatBubble> List_chatBubble;
    public Transform TF_chatBubbleParent;

    /// <summary>
    /// Ini jumlah maksimum chat bubble yang nilai transparansinya 100% kelihatan
    /// </summary>
    public int I_maxChatBubble = 2;
    /// <summary>
    /// Ini jumlah chat bubble yang bakal menjadi sedikit transparan. Chat bubble bakal jadi sedikit transparan setelah melewati batasan I_maxChatBubble. Kalau nilai ini 0, maka ga bakal ada chat bubble yang setengah transparan.
    /// </summary>
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


    /// <summary>
    /// Dipanggil pas click interact, seharusnya
    /// Ini seharusnya dipanggil sebelum PlayDialogue di ChatManager.
    /// 
    /// Buat setup semua chat bubble. Awalnya semuanya bakal didisable, jadi nanti tinggal dienable aja kalau mau munculin
    /// </summary>

    public void SetupAllChats()
    {
        //Bersihin list chat bubble kalau misalnya ada yang tersisa, mungkin dari dialog-dialog sebelumnya.
        if (List_chatBubble == null) List_chatBubble = new List<UIChatBubble>();
        List_chatBubble.Clear();

        //Hapus semua game objects dari parent chat bubble, mungkin dari dialog-dialog sebelumnya
        foreach (Transform child in TF_chatBubbleParent)
        {
            Destroy(child.gameObject);
        }

        //Ini dialog/renungan saat ini.
        DialogSO SO_dialogSO = ChatManager.Instance.SO_currDialog;

        //Ini bakal membuat semua chat bubble yang bakal ada di dalam renungan, tetapi dia bakal didisable terlebih dahulu supaya ga kelihatan di UI. Nanti mereka bakal dienable kalau sudah waktunya.
        foreach (DialogComponent components in SO_dialogSO.SCR_dialogComponent)
        {
            GameObject GO_chatBubble = Instantiate(PB_chatBubble, TF_chatBubbleParent);

            UIChatBubble SCR_UIChatBubble = GO_chatBubble.GetComponent<UIChatBubble>();

            SCR_UIChatBubble.Setup(components);

            GO_chatBubble.SetActive(false);

            List_chatBubble.Add(SCR_UIChatBubble);
        }

        //Untuk chat bubble pertama, dia bakal dienable.
        SetupChatBubble(0, 1f);
    }

    /// <summary>
    /// Mengatur chat bubble selanjutnya yang akan muncul, DAN JUGA mengatur chat bubble sebelumnya (untuk fading dan semacamnya)
    /// </summary>
    /// <param name="I_currentIndex">Index chat bubble dalam dialog/renungan saat ini</param>
    private void SetupNextChatBubble(int I_currentIndex)
    {
        if(I_currentIndex < List_chatBubble.Count)
        {
            SetupChatBubble(I_currentIndex, 1f);
            SetupPreviousChatBubbles(I_currentIndex);
        }
    }

    /// <summary>
    /// Ini mengatur chat bubble secara satuan.
    /// </summary>
    /// <param name="I_index">Index dari chat bubble yang akan diatur</param>
    /// <param name="F_alpha">Alpha/transparansi yang akan dipasang ke chat bubblenya</param>
    private void SetupChatBubble(int I_index, float F_alpha)
    {
        List_chatBubble[I_index].gameObject.SetActive(true);
        List_chatBubble[I_index].Fade(F_alpha);
    }

    /// <summary>
    /// Ini mengatur chat bubble yang udah pernah muncul sebelumnya.
    /// 
    /// Kalau chat bubblenya sudah sangat lama, dia bakal didisable.
    /// 
    /// Kalau chat bubblenya udah agak lama, dia bakal ngefade (dan fadingnya bakal perlahan-lahan seperti gradasi)
    /// 
    /// Kalau chat bubblenya masih baru, transparansinya masih 100% kelihatan
    /// 
    /// Kalau chat bubblenya memang belum muncul, dia bakal keluar dari loop soalnya sudah ga perlu ngeproses yang belum muncul.
    /// </summary>
    /// <param name="I_currentIndex">Index chat bubble saat ini</param>
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
