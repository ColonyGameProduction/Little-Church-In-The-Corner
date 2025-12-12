using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class untuk mengatur NPC dalam gereja
/// </summary>
public class ChurchManager : MonoBehaviour
{
    /// <summary>
    /// List NPC yang ga aktif a.k.a didisable
    /// </summary>
    public List<Interact> List_SCR_inactiveNPC;
    /// <summary>
    /// List NPC yang saat ini aktif dan kelihatan di gereja, antara menunggu antrian atau memang sudah diklik/diinteract
    /// </summary>
    public List<Interact> List_SCR_activeNPC;

    public Animator AC_priestAnimatorController;

    private void Awake()
    {
        List_SCR_activeNPC = new List<Interact>();
    }

    private void Start()
    {
        AC_priestAnimatorController.SetBool("B_isStanding", true);
    }

    private void OnEnable()
    {
        TimeManager.ACT_interactIsReady += SetupNPCs;
        UIChatManager.ACT_NoCurrentSermonAvailable += DespawnInteractedNPC;
    }

    private void OnDisable()
    {
        TimeManager.ACT_interactIsReady -= SetupNPCs;
        UIChatManager.ACT_NoCurrentSermonAvailable -= DespawnInteractedNPC;
    }

    /// <summary>
    /// Function untuk memunculkan NPC. Untuk saat ini, NPC yang dimunculin random.
    /// </summary>
    public void SpawnNPC()
    {
        // Kalau ternyata penuh, maka jangan spawn lagi
        if (List_SCR_inactiveNPC.Count <= 0)
        {
            Debug.Log("There are no NPC slots available!");
        }
        else
        {
            // Ambil NPC random
            int I_spawnedIndex = Random.Range(0, List_SCR_inactiveNPC.Count);

            Interact SCR_spawnedNPC = List_SCR_inactiveNPC[I_spawnedIndex];

            // Pindahin NPCnya dari list inactive ke list active
            List_SCR_activeNPC.Add(SCR_spawnedNPC);
            List_SCR_inactiveNPC.RemoveAt(I_spawnedIndex);

            // Munculin NPC random
            SCR_spawnedNPC.HideAllNPC();
            SCR_spawnedNPC.ShowRandomNPC();
            SCR_spawnedNPC.gameObject.SetActive(true);
        }

        // Sync renungan yang ada di antrian dengan jumlah NPC yang aktif saat ini, YANG BELUM DIINTERACT a.k.a yang belum diklik (alasannya adalah karena aku bikin supaya kalau diklik, dia belum hilang sampai renungannya selesai semua)
        TimeManager.Instance.I_queuedSermon = List_SCR_activeNPC.Count(NPC => !NPC.B_hasBeenInteracted);
    }

    /// <summary>
    /// Despawn NPC
    /// </summary>
    /// <param name="GO_NPC">NPC yang bakal despawn</param>
    public void DespawnNPC(Interact GO_NPC)
    {
        GO_NPC.B_hasBeenInteracted = false;

        // Pindahin dari active ke inactive
        List_SCR_inactiveNPC.Add(GO_NPC);
        List_SCR_activeNPC.Remove(GO_NPC);

        GO_NPC.gameObject.SetActive(false);
    }

    /// <summary>
    /// Function untuk despawn semua NPC yang udah diinteract, just in case ada yang kelewatan. Ini dipanggil pas renungan udah selesai semua.
    /// </summary>
    private void DespawnInteractedNPC()
    {
        for (int i = List_SCR_activeNPC.Count - 1; i >= 0; i--)
        {
            if (!List_SCR_activeNPC[i].B_hasBeenInteracted) continue;

            DespawnNPC(List_SCR_activeNPC[i]);
        }
    }

    /// <summary>
    /// Function untuk menyeimbangkan antara antrian renungan yang ada di TimeManager dan NPC yang aktif di list activeNPC.
    /// </summary>
    public void SetupNPCs()
    {
        //Count, dia bakal menghitung berapa banyak NPC yang belum diinteract
        //Karena NPC yang udah diinteract ga boleh didespawn sebelum renungan selesai
        int I_difference = TimeManager.Instance.I_queuedSermon - List_SCR_activeNPC.Count(NPC => !NPC.B_hasBeenInteracted);
        Debug.Log("Active NPCs that haven't been interacted: " + List_SCR_activeNPC.Count(NPC => !NPC.B_hasBeenInteracted));
        Debug.Log("Queued sermon: " + TimeManager.Instance.I_queuedSermon);

        // Kalau ternyata NPCnya kurang
        if (I_difference > 0)
        {
            Debug.Log($"Spawning {I_difference}x NPC");
            for (int i = 0; i < I_difference; i++)
            {
                SpawnNPC();
            }
        }
        // Kalau ternyata NPCnya kelebihan
        else if (I_difference < 0)
        {
            Debug.Log($"Despawning {I_difference}x NPC");
            for (int i = List_SCR_activeNPC.Count - 1; i >= 0; i--)
            {
                // Ini bakal despawn NPC sesuai urutan di list, sampai differencenya seimbang
                if (I_difference >= 0) break;

                // Kalau udah diinteract, jangan despawn dulu
                if (List_SCR_activeNPC[i].B_hasBeenInteracted) continue;

                DespawnNPC(List_SCR_activeNPC[i]);

                I_difference++;
            }
        }
        //Kalau pas, ga usah ngapa-apain.
    }
}
