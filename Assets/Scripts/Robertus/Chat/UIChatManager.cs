using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengatur seluruh hal mengenai UI dari Chat, seperti menampilkan chat bubble dan animasi dari chat bubble.
/// </summary>
public class UIChatManager : MonoBehaviour
{
    public static UIChatManager Instance { get; private set; }

    [Header("References")]
    public GameObject PB_chatBubble;
    [HideInInspector] public List<UIChatBubble> List_chatBubble;
    public Transform TF_chatBubbleParent;

    [Header("Data")]
    /// <summary>
    /// Ini jumlah maksimum chat bubble yang nilai transparansinya 100% kelihatan
    /// </summary>
    public int I_maxChatBubble = 2;
    /// <summary>
    /// Ini jumlah chat bubble yang bakal menjadi sedikit transparan. Chat bubble bakal jadi sedikit transparan setelah melewati batasan I_maxChatBubble. Kalau nilai ini 0, maka ga bakal ada chat bubble yang setengah transparan.
    /// </summary>
    public int I_amountOfFadedChatBubbles = 2;
    /// <summary>
    /// Ini durasi animasi, seperti animasi fade out, animasi gerak, dst.
    /// </summary>
    public float F_animationDuration = 1f;
    /// <summary>
    /// Ini seberapa jauh chat bubblenya bakal naik ke atas pas fade out (fade out di sini itu pas mau mulai dialog baru. Dialog lama bakal fade out semuanya)
    /// </summary>
    public float F_fadeOutDistance = 100f;
    /// <summary>
    /// Ini seberapa cepat animasi teks. Semakin besar angkanya, semakin cepat.
    /// </summary>
    public float F_textAnimationSpeed = 1f;

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
        //Fade out semua chat bubble yang masih tersisa
        RemoveAllChatBubbles();

        //Bersihin list chat bubble kalau misalnya ada yang tersisa, mungkin dari dialog-dialog sebelumnya.
        if (List_chatBubble == null) List_chatBubble = new List<UIChatBubble>();
        List_chatBubble.Clear();


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
    }

    /// <summary>
    /// Fade out semua chat bubble yang ada di list. Setelah difadeout, mereka bakal didestroy.
    /// 
    /// Ini bakal terjadi kalau ada renungan baru. Dialog renungan lama bakal dihapus melalui function ini.
    /// </summary>
    private void RemoveAllChatBubbles()
    {
        foreach (UIChatBubble SCR_chatBubble in List_chatBubble)
        {
            SCR_chatBubble.FadeOutAndDestroyAnimation();
        }
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
            List_chatBubble[I_currentIndex].MoveUpAnimation();
            List_chatBubble[I_currentIndex].StartTextAnimation(F_textAnimationSpeed);
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
                //Debug.Log($"{i}. Reached the end of setup");
                break;
            }

            //Kalau chat bubblenya udah lama banget, hilangin. Ga jadi didisable soalnya ngaruh ke animasi fade outnya. Kalau didisable langsung, dia bakal abrupt gitu fade outnya.
            if (I_currentIndex - i >= I_maxChatBubble + I_amountOfFadedChatBubbles)
            {
                SetupChatBubble(i, 0f);
                //List_chatBubble[i].gameObject.SetActive(false);
                //Debug.Log($"{i} chat has expired, disabling");
                continue;
            }

            //Kalau chat bubblenya baru dan belum melebihi max chat bubble, maka transparansinya 1
            if (I_currentIndex - i < I_maxChatBubble)
            {
                SetupChatBubble(i, 1f);
                //Debug.Log($"{i} chat is new, alpha at 1");
                continue;
            }

            //Kalau chat bubblenya udah agak lama, mulai fade
            float F_alpha = 1 - ((I_currentIndex - i + 1.0f - I_maxChatBubble) / (I_amountOfFadedChatBubbles + 1.0f));
            SetupChatBubble(i, F_alpha);
            //Debug.Log($"{i} chat is fading, alpha at {F_alpha}");
        }
    }
}
