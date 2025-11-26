using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class untuk mengatur visual dari ruangan office, seperti tombol interact dan NPC yang ada di dalamnya.
/// </summary>
public class OfficeManager : MonoBehaviour
{
    public static OfficeManager Instance;

    /// <summary>
    /// NPC yang duduk di kursi di office
    /// </summary>
    public List<GameObject> List_GO_NPCs;
    /// <summary>
    /// Tombol interact
    /// </summary>
    public OfficeInteract SCR_officeInteract;

    /// <summary>
    /// 3D Model pastor pas lagi ga ada QnA
    /// </summary>
    public GameObject GO_priestIdle;
    /// <summary>
    /// 3D Model pastor pas lagi QnA, ngobrol dengan NPC
    /// </summary>
    public GameObject GO_priestActive;

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
        TimeManager.ACT_interactQnAIsReady += SetupInteract;
        UIChatManager.ACT_NoCurrentSermonAvailable += FinishInteract;
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactQnAIsReady -= SetupInteract;
        UIChatManager.ACT_NoCurrentSermonAvailable -= FinishInteract;
    }

    /// <summary>
    /// Function untuk memunculkan tombol interact. Tombol interact muncul kalau:
    /// 1. Sudah waktunya untuk muncul via jadwalnya yang ada di Time Manager
    /// 2. Kalau misalnya udah selesai interact tapi masih ada QnA lain di dalam queue
    /// </summary>
    public void SetupInteract()
    {
        //Kalau masih ada renungan yang jalan, jangan interact lagi
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;

        //Kalau ga ada renungan dalam queue, jangan tampilin interact button.
        if (TimeManager.Instance.I_queuedQnA <= 0)
            return;

        SCR_officeInteract.gameObject.SetActive(true);
    }

    /// <summary>
    /// Kalau misalnya sudah selesai membaca semua renungan di dalam QnA, function ini bakal ditrigger.
    /// Function ini bakal menyembunyikan NPC (karena dia anggapannya udah selesai mendengarkan QnA dari pastornya, lalu keluar dari ruangan), serta mengecek apakah ada QnA lain di dalam queue atau tidak. Kalau ada, maka tombol interact bakal dimunculin lagi.
    /// </summary>
    public void FinishInteract()
    {
        HideAllNPC();
        SetPriestActive(false);
        if (TimeManager.Instance.I_queuedQnA <= 0) SCR_officeInteract.gameObject.SetActive(false);
        else SetupInteract();
    }

    /// <summary>
    /// Function untuk setup NPC. Untuk saat ini, functionnya cuma mengaktifkan game object NPC saja, tapi ke depannya mungkin bisa ditambah hal lain seperti animasi. Idk.
    /// </summary>
    public void SetupNPC()
    {
        ShowRandomNPC();
        SetPriestActive(true);
    }

    /// <summary>
    /// Function untuk sembunyiin semua jenis NPC yang dapat dilihat.
    /// </summary>
    public void HideAllNPC()
    {
        foreach (GameObject GO_NPC in List_GO_NPCs)
        {
            GO_NPC.SetActive(false);
        }
    }

    /// <summary>
    /// Function untuk menunjukkan NPC yang random
    /// </summary>
    public void ShowRandomNPC()
    {
        if (List_GO_NPCs.Count <= 0) return;
        List_GO_NPCs[Random.Range(0, List_GO_NPCs.Count)].SetActive(true);
    }

    /// <summary>
    /// Function buat pindahin posisi pastor dari idle ke mode QnA
    /// </summary>
    /// <param name="status">Kalau True, dia bakal pindah dari Idle ke QnA. Kalau false, sebaliknya</param>
    public void SetPriestActive(bool status)
    {
        GO_priestIdle.SetActive(!status);
        GO_priestActive.SetActive(status);
    }
}
