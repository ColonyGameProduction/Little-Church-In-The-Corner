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
    /// Ini buat tahu apakah renungannya sudah selesai atau belum. Technically kalau dialog terakhir udah muncul, dia udah selesai. Tapi, gara-gara ada animasi teks muncul perlahan-lahan, jadi dia beneran udah selesai pas animasinya udah selesai.
    /// Animasi udah selesai atau belum, ditentuin dari variabel ini.
    /// </summary>
    public int I_amountOfTextAnimationDone;

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

    private void OnEnable()
    {
        TimeManager.ACT_interactIsReady += SetupRenungan;
        UIChatManager.ACT_NoCurrentSermonAvailable += SetupRenungan;
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactIsReady -= SetupRenungan;
        UIChatManager.ACT_NoCurrentSermonAvailable -= SetupRenungan;
    }

    /// <summary>
    /// Mengatur jalannya dialog dalam renungan secara otomatis.
    /// 
    /// Dia bakal lanjut ke dialog selanjutnya secara otomatis setelah sekian lama.
    /// </summary>
    /// <param name="f_interval">Berapa lama jeda antar dialog</param>
    /// <returns></returns>
    public IEnumerator DialogueSequence(float f_interval)
    {
        I_amountOfTextAnimationDone = 0;

        //TODO: ganti supaya pakai TransitionManager.
        Debug.LogError("WARNING: Ganti codingan SetupRenungan supaya memakai ruangan saat ini");
        DialogSO SO_currDialog = SO_listOfDialogueSO.SO_GetDialogSO(ENM_Room.Church, ENM_currDialog);

        while (I_currDialogComponentIndex < SO_currDialog.SCR_dialogComponent.Count)
        {
            ACT_PlayDialogue?.Invoke(I_currDialogComponentIndex);
            I_currDialogComponentIndex++;
            //Keluar duluan biar ga usah nunggu selama f_interval, tapi nunggunya tergantung text animation.
            if (I_currDialogComponentIndex >= SO_currDialog.SCR_dialogComponent.Count) break;
            yield return new WaitForSeconds(f_interval);
        }

        //Kalau semua teks udah selesai animasinya, baru lanjut. Kalau belum, stay di sini.
        yield return new WaitUntil(() => I_amountOfTextAnimationDone >= I_currDialogComponentIndex);

        Debug.Log("All dialogues done!");

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
        //Kalau ga ada renungan dalam queue, jangan setup renungan.
        if (TimeManager.Instance.I_queuedSermon <= 0)
            return;

        //TODO: ganti supaya pakai TransitionManager.
        Debug.LogError("WARNING: Ganti codingan SetupRenungan supaya memakai ruangan saat ini");
        ENM_currDialog = SO_listOfDialogueSO.SO_GetDialogSO(ENM_Room.Church).ENM_dialogTitle;
        I_currDialogComponentIndex = 0;
    }
}
