using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mengatur logic waktu dan penjadwalan renungan
/// </summary>
public class LocalTime : MonoBehaviour
{
    /// <summary>
    /// Waktu saat ini, tergantung device pemain
    /// </summary>
    public DateTime DT_localTime;
    /// <summary>
    /// Kalau waktunya sesuai dengan jadwal, maka trigger action ini
    /// </summary>
    public static event Action ACT_interactIsReady;

    /// <summary>
    /// Jadwal yang bisa dilihat dan diatur di Inspector
    /// </summary>
    public SerializedTime[] SCR_sermonSchedule;

    /// <summary>
    /// Jadwal aslinya, nanti diconvert dari SCR_sermonSchedule
    /// </summary>
    private List<Schedule> List_SCR_sermonSchedule;

    /// <summary>
    /// Ini untuk yield new return WaitForSeconds, tapi karena delaynya selalu 1 menit (jadi cuma ngecek ada renungan baru atau engga setiap satu menit), jadi diset di awal aja.
    /// </summary>
    private WaitForSeconds WFS_oneMinuteTimer;

    private void Start()
    {
        //Convert jadwal yang diset di Inspector ke jadwal yang bisa diedit dan semacamnya.
        List_SCR_sermonSchedule = new List<Schedule>();
        foreach (SerializedTime time in SCR_sermonSchedule)
        {
            List_SCR_sermonSchedule.Add(new Schedule(time.DT_ToDateTime(), false));
        }

        WFS_oneMinuteTimer = new WaitForSeconds(60);
        StartCoroutine(WaitForRenungan());
    }

    /// <summary>
    /// Cek renungan dari jadwal setiap satu menit.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRenungan()
    {
        while (true)
        {
            CheckForRenungan();
            yield return WFS_oneMinuteTimer;
        }
    }

    /// <summary>
    /// Cek apakah bisa akses renungan baru berdasarkan jadwal dan waktu saat ini
    /// </summary>
    private void CheckForRenungan()
    {
        //Lagi ada renungan yang jalan, jadi jangan mulai renungan lain
        if (ChatManager.Instance.SO_currDialog)
            return;

        DT_localTime = DateTime.Now;
        bool B_renunganIsReady = false;

        foreach (Schedule schedule in List_SCR_sermonSchedule)
        {
            //Debug.Log($"Schedule {schedule.B_hasBeenShown}: {schedule.DT_time.Hour} : {schedule.DT_time.Minute} VS datetime now {DT_localTime.Hour} : {DT_localTime.Minute}");

            //Kalau jadwalnya udah pernah dijalanin sebelumnya, lewatin aja
            if (schedule.B_hasBeenShown) continue;

            //Kalau misalnya jadwalnya udah lewat dari waktu saat ini dan belum dijalanin
            if(DT_localTime.Hour > schedule.DT_time.Hour ||
                (DT_localTime.Hour == schedule.DT_time.Hour && DT_localTime.Minute >= schedule.DT_time.Minute))
            {
                schedule.B_hasBeenShown = true;
                B_renunganIsReady = true;

                //Game Design:
                //Kalau misalnya lewat beberapa jadwal sekaligus (misal, baru interact dan jalanin renungan di jam 5 sore, padahal sudah ada renungan yang nunggu dari jam 12 dan jam 3), apakah pas interact:
                //1. jalanin renungan, lalu yang sisanya dibuang. Saat ini, sistemnya udah kayak gini.
                //2. jalanin renungan, tapi yang lain disimpan (diqueue), jadi abis renungannya habis, bisa langsung play renungan lain karena udah ada di queue. Sistem ini bisa diperoleh dengan cara menambahkan break; di akhir if condition ini.
                //break;
            }
        }

        if(B_renunganIsReady) RenunganIsReady();
    }

    /// <summary>
    /// Kalau renungan udah siap, invoke action (setup renungan dan tampilin tombol interact)
    /// </summary>
    private void RenunganIsReady()
    {
        ACT_interactIsReady?.Invoke();
    }
}
