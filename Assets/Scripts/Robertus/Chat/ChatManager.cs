using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Mengatur logic dari chat secara keseluruhan, seperti mengatur kapan muncul dialog selanjutnya, dan semacamnya.
/// </summary>
public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }

    /// <summary>
    /// List semua dialog yang ada dalam bentuk scriptable object
    /// </summary>
    public ListOfDialogSO SO_listOfDialogueSO;
    /// <summary>
    /// List semua muka karakter yang ada
    /// </summary>
    public ListOfFace SCR_listOfFace;
    /// <summary>
    /// List semua background yang dapat dipakai oleh chat bubble
    /// </summary>
    public ListOfChatBubbleBackground SCR_listOfChatBubbleBackground;
    /// <summary>
    /// Dialog yang aktif saat ini
    /// </summary>
    [HideInInspector] public ENM_DialogTitle ENM_currDialog;
    /// <summary>
    /// Jeda antar teks dialog dalam satuan detik.
    /// </summary>
    public float F_interval;
    /// <summary>
    /// Index teks dialog saat ini
    /// </summary>
    [HideInInspector] public int I_currDialogComponentIndex;

    /// <summary>
    /// Berapa lama waktu setelah semua chat ditampilkan sebelum menampilkan pilihan untuk download
    /// </summary>
    public float F_delayBeforeShowingDownloadOptions = 1.5f;

    /// <summary>
    /// Ini buat tahu apakah renungannya sudah selesai atau belum. Technically kalau dialog terakhir udah muncul, dia udah selesai. Tapi, gara-gara ada animasi teks muncul perlahan-lahan, jadi dia beneran udah selesai pas animasinya udah selesai.
    /// Animasi udah selesai atau belum, ditentuin dari variabel ini.
    /// </summary>
    public int I_amountOfTextAnimationDone;

    /// <summary>
    /// Ini cuma aktif kalau misalnya ada chat. Kalau ga ada chat, swiping bakal tetep nyala.
    /// </summary>
    public DisableRoomSwipe SCR_disableRoomSwipe;

    public static event Action<int> ACT_PlayDialogue;
    public static event Action ACT_RenunganDone;

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

    //private void OnEnable()
    //{
    //    TimeManager.ACT_interactIsReady += SetupRenungan;
    //    UIChatManager.ACT_NoCurrentSermonAvailable += SetupRenungan;
    //}

    //private void OnDisable()
    //{
    //    TimeManager.ACT_interactIsReady -= SetupRenungan;
    //    UIChatManager.ACT_NoCurrentSermonAvailable -= SetupRenungan;
    //}

    /// <summary>
    /// Mengatur jalannya dialog dalam renungan secara otomatis.
    /// 
    /// Dia bakal lanjut ke dialog selanjutnya secara otomatis setelah sekian lama.
    /// </summary>
    /// <param name="f_interval">Berapa lama jeda antar dialog</param>
    /// <returns></returns>
    public IEnumerator DialogueSequence(float f_interval)
    {
        WaitForSeconds WFS_interval = new WaitForSeconds(f_interval);
        WaitForSeconds WFS_delay = new WaitForSeconds(F_delayBeforeShowingDownloadOptions);

        I_amountOfTextAnimationDone = 0;

        SCR_disableRoomSwipe.enabled = true;

        DialogSO SO_currDialog = SO_listOfDialogueSO.SO_GetDialogSO(TransitionManager.Instance.ENM_room, ENM_currDialog);

        while (I_currDialogComponentIndex < SO_currDialog.SCR_dialogComponent.Count)
        {
            ACT_PlayDialogue?.Invoke(I_currDialogComponentIndex);
            I_currDialogComponentIndex++;
            // Ini buat nungguin animasi teksnya kelar dulu, baru lanjut ke teks selanjutnya
            yield return new WaitUntil(() => I_amountOfTextAnimationDone >= I_currDialogComponentIndex);
            //Keluar duluan biar ga usah nunggu selama f_interval, tapi nunggunya tergantung text animation.
            if (I_currDialogComponentIndex >= SO_currDialog.SCR_dialogComponent.Count) break;
            yield return WFS_interval;
        }

        //Kalau semua teks udah selesai animasinya, baru lanjut. Kalau belum, stay di sini.
        yield return new WaitUntil(() => I_amountOfTextAnimationDone >= I_currDialogComponentIndex);

        Debug.Log("All dialogues done!");

        //Mungkin tambahin delay sedikit
        yield return WFS_delay;

        //Nunggu semuanya selesai dulu, baru munculin hal lain seperti opsi untuk download
        ACT_RenunganDone?.Invoke();
    }

    /// <summary>
    /// Menjalankan dialog/renungan
    /// </summary>
    public void PlayDialogue()
    {
        Debug.Log("Play dialogue");
        StartCoroutine(DialogueSequence(F_interval));
    }

    /// <summary>
    /// Menghentikan dialog/renungan yang saat ini ada.
    /// </summary>
    public void StopDialogue()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Mengatur renungan baru. Ini seharusnya dipanggil dari RenunganIsReady yang ada di TimeManager.
    /// Also dipanggil kalau misalnya ada renungan di queue dan seluruh renungan sudah selesai
    /// Ini bakal ngambil renungan random dan mengatur index dialog menjadi 0 kembali.
    /// </summary>
    public void SetupRenungan()
    {
        //Kalau lagi ada renungan yang berjalan, jangan setup renungan.
        if (ENM_currDialog != ENM_DialogTitle.None)
            return;

        ////Kalau ga ada renungan dalam queue, jangan setup renungan.
        ///Tergantung ruangan. Kalau misalnya ruangannya office, maka cek queue QnA.
        ///Kalau di gereja, cek queue renungan.
        ///Ruangannya tergantung ruangan saat ini.
        if (TransitionManager.Instance.ENM_room == ENM_Room.Office)
        {
            if (TimeManager.Instance.I_queuedQnA <= 0)
                return;
        }
        else if (TransitionManager.Instance.ENM_room == ENM_Room.Church)
        {
            if (TimeManager.Instance.I_queuedSermon <= 0)
                return;
        }

        ENM_currDialog = SO_listOfDialogueSO.SO_GetDialogSO(TransitionManager.Instance.ENM_room).ENM_dialogTitle;
        I_currDialogComponentIndex = 0;
    }
}
