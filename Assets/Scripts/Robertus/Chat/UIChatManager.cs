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
    /// <summary>
    /// Game object yang berisi tombol-tombol untuk download renungan saat ini. Ini muncul pas renungan sudah selesai ditampilkan semuanya.
    /// </summary>
    public GameObject GO_optionToDownloadContainer;
    /// <summary>
    /// Game object yang berisi tombol untuk pergi ke menu list renungan yang sudah didownload. Ini muncul setelah pemain pilih salah satu opsi untuk download atau tidak renungan saat ini.
    /// </summary>
    public GameObject GO_downloadedSermonButtonContainer;
    /// <summary>
    /// Menu renungan yang sudah didownload
    /// </summary>
    public GameObject GO_dictionaryContainer;

    /// <summary>
    /// Tombol untuk download renungan saat ini
    /// </summary>
    public Button BTN_yesToDownloadButton;
    /// <summary>
    /// Tombol untuk tidak download renungan saat ini.
    /// </summary>
    public Button BTN_noToDownloadButton;
    /// <summary>
    /// Tombol untuk menampilkan menu list renungan yang sudah didownload
    /// </summary>
    public Button BTN_downloadedSermonsButton;
    /// <summary>
    /// Tombol untuk keluar dari menu list renungan yang sudah didownload
    /// </summary>
    public Button BTN_closeDownloadedSermonsButton;

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
    public float F_textAnimationSpeed;

    /// <summary>
    /// Action untuk menunjukkan kalau lagi ga ada renungan yang sedang ditampilkan.
    /// </summary>
    public static event Action ACT_NoCurrentSermonAvailable;

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
        BTN_yesToDownloadButton.onClick.AddListener(DownloadSermon);
        BTN_yesToDownloadButton.onClick.AddListener(HideDownloadOptionsAndDialogue);
        BTN_noToDownloadButton.onClick.AddListener(HideDownloadOptionsAndDialogue);
        ChatManager.ACT_RenunganDone += ShowDownloadOptions;
        BTN_downloadedSermonsButton.onClick.AddListener(ShowDownloadedSermons);
        BTN_closeDownloadedSermonsButton.onClick.AddListener(HideDownloadedSermons);
    }

    private void OnDisable()
    {
        ChatManager.ACT_PlayDialogue -= SetupNextChatBubble;
        BTN_yesToDownloadButton.onClick.RemoveAllListeners();
        BTN_noToDownloadButton.onClick.RemoveAllListeners();
        ChatManager.ACT_RenunganDone -= ShowDownloadOptions;
        BTN_downloadedSermonsButton.onClick.RemoveAllListeners();
        BTN_closeDownloadedSermonsButton.onClick.RemoveAllListeners();
    }

    #region Chat Bubble
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

        //Renungan baru bakal dimulai, jadi tombol "Saved Sermon" dihilangin untuk sementara waktu
        GO_downloadedSermonButtonContainer.SetActive(false);

        //Ini dialog/renungan saat ini.
        DialogSO SO_dialogSO = ChatManager.Instance.SO_listOfDialogueSO.SO_GetDialogSO(TransitionManager.Instance.ENM_room, ChatManager.Instance.ENM_currDialog); ;

        //Ini bakal membuat semua chat bubble yang bakal ada di dalam renungan, tetapi dia bakal didisable terlebih dahulu supaya ga kelihatan di UI. Nanti mereka bakal dienable kalau sudah waktunya.
        foreach (DialogComponent components in SO_dialogSO.SCR_dialogComponent)
        {
            GameObject GO_chatBubble = Instantiate(PB_chatBubble, TF_chatBubbleParent);

            UIChatBubble SCR_UIChatBubble = GO_chatBubble.GetComponent<UIChatBubble>();

            SCR_UIChatBubble.Setup(components);

            GO_chatBubble.SetActive(false);

            List_chatBubble.Add(SCR_UIChatBubble);
        }

        ChatManager.Instance.PlayDialogue();
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
        //Bersihin list chat bubble kalau misalnya ada yang tersisa, mungkin dari dialog-dialog sebelumnya.
        if (List_chatBubble == null) List_chatBubble = new List<UIChatBubble>();
        List_chatBubble.Clear();
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
        for (int I_chatBubbleIndex = 0; I_chatBubbleIndex < List_chatBubble.Count; I_chatBubbleIndex++)
        {
            //Kalau I_chatBubbleIndex melebihi current index, break soalnya dialognya lom dimunculin.
            if (I_chatBubbleIndex > I_currentIndex)
            {
                //Debug.Log($"{I_chatBubbleIndex}. Reached the end of setup");
                break;
            }

            //Kalau chat bubblenya udah lama banget, hilangin. Ga jadi didisable soalnya ngaruh ke animasi fade outnya. Kalau didisable langsung, dia bakal abrupt gitu fade outnya.
            if (I_currentIndex - I_chatBubbleIndex >= I_maxChatBubble + I_amountOfFadedChatBubbles)
            {
                SetupChatBubble(I_chatBubbleIndex, 0f);
                //List_chatBubble[I_chatBubbleIndex].gameObject.SetActive(false);
                //Debug.Log($"{I_chatBubbleIndex} chat has expired, disabling");
                continue;
            }

            //Kalau chat bubblenya baru dan belum melebihi max chat bubble, maka transparansinya 1
            if (I_currentIndex - I_chatBubbleIndex < I_maxChatBubble)
            {
                SetupChatBubble(I_chatBubbleIndex, 1f);
                //Debug.Log($"{I_chatBubbleIndex} chat is new, alpha at 1");
                continue;
            }

            //Kalau chat bubblenya udah agak lama, mulai fade
            float F_alpha = 1 - ((I_currentIndex - I_chatBubbleIndex + 1.0f - I_maxChatBubble) / (I_amountOfFadedChatBubbles + 1.0f));
            SetupChatBubble(I_chatBubbleIndex, F_alpha);
            //Debug.Log($"{I_chatBubbleIndex} chat is fading, alpha at {F_alpha}");
        }
    }
    #endregion

    #region Dictionary

    /// <summary>
    /// Tampilkan pilihan untuk download renungan saat ini.
    /// Dipanggil saat renungan sudah selesai ditampilkan.
    /// 
    /// Ada SetAsLastSibling karena sekarang pilihannya dimasukkin ke dalam list chat bubble di dalam UI, sehingga harus pake itu biar pilihan downloadnya ditaro di paling bawah dalam list
    /// </summary>
    private void ShowDownloadOptions()
    {
        GO_optionToDownloadContainer.transform.SetAsLastSibling();
        GO_optionToDownloadContainer.SetActive(true);
    }

    /// <summary>
    /// Sembunyikan pilihan untuk download renungan saat ini.
    /// Also sembunyikan semua chat bubble renungan saat ini.
    /// Also (2) tampilkan tombol untuk pergi ke menu list renungan yang sudah didownload.
    /// 
    /// Ditampilkan saat pemain pilih salah satu opsi untuk download atau tidak.
    /// </summary>
    private void HideDownloadOptionsAndDialogue()
    {
        GO_optionToDownloadContainer.SetActive(false);
        RemoveAllChatBubbles();
        GO_downloadedSermonButtonContainer.SetActive(true);

        //Sudah ga ada renungan yang berjalan.
        ChatManager.Instance.ENM_currDialog = ENM_DialogTitle.None;
        ACT_NoCurrentSermonAvailable?.Invoke();
    }

    /// <summary>
    /// Dipanggil saat pemain pilih Yes untuk download renungan saat ini.
    /// </summary>
    private void DownloadSermon()
    {
        DictionaryManager.Instance.DownloadToDevice();
    }

    /// <summary>
    /// Dipanggil saat pemain menekan tombol untuk menampilkan menu list renungan yang sudah didownload.
    /// Ini bakal menyiapkan dan menampilkan list renungan yang sudah didownload.
    /// </summary>
    private void ShowDownloadedSermons()
    {
        DictionaryManager.Instance.SCR_UIDictionary.SetupAllListOfSermon();
        GO_dictionaryContainer.SetActive(true);
    }

    /// <summary>
    /// Dipanggil saat pemain menekan tombol exit atau keluar di dalam menu list renungan yang sudah didownload.
    /// </summary>
    private void HideDownloadedSermons()
    {
        GO_dictionaryContainer.SetActive(false);
    }

    #endregion
}
