using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class yang mengatur UI dari day night cycle, terutama untuk animasi dan pergantian warna ruangan dan background.
/// </summary>
public class DayNightCycleUI : MonoBehaviour
{
    /// <summary>
    /// Waktu saat ini
    /// </summary>
    private DateTime DT_localTime;
    /// <summary>
    /// Jadwal sebelumnya yang pernah muncul. Ini untuk keperluan animasi, misal dari jadwal A pukul 10 transisi ke jadwal B pukul 15. Maka SCR_previousDayNightCycleSchedule adalah Jadwal A.
    /// </summary>
    private DayNightSchedule SCR_previousDayNightCycleSchedule;

    /// <summary>
    /// Durasi animasi transisi antar jadwal
    /// </summary>
    public float F_transitionDuration = 5f;

    /// <summary>
    /// Untuk keperluan testing saja, ini untuk mengubah waktu local time biar lebih maju atau mundur.
    /// </summary>
    private float F_offsetMenitTesting;

    /// <summary>
    /// Karena Raden pasang spritenya lewat codingan, jadi harus gini untuk sementara waktu
    /// </summary>
    [HideInInspector] public Sprite SPR_currentSelectedPlaylistButton;

    /// <summary>
    /// Karena Raden pasang spritenya lewat codingan, jadi harus gini untuk sementara waktu. + aku mager buat bikin yang lebih rapi
    /// </summary>
    public UIPlaylist SCR_UIPlaylist;

    private void Awake()
    {
        HideAll();
    }

    /// <summary>
    /// Function untuk mengecek dan mengubah background. Ini dipanggil di TimeManager, barengan dengan waktu untuk mengecek renungan.
    /// </summary>
    public void ChangingBackground()
    {
        // Kalau ternyata ga ada background yang dapat digunakan, maka background ga bakal diganti.
        if (DayNightCycleManager.Instance.List_SCR_timeToSwitch.Count <= 0)
        {
            Debug.LogWarning("WARNING: Ga ada background yang dapat digunakan untuk day night cycle!");
            return;
        }

        //Ada AddMinutes untuk keperluan testing. Kalau ga perlu lagi, delete aja (jadi DateTime.Now doang)
        DT_localTime = DateTime.Now.AddMinutes(F_offsetMenitTesting);
        Debug.Log($"Waktu sekarang: {DT_localTime}");

        //Defaultnya background yang paling pertama
        DayNightSchedule SCR_currentSchedule = DayNightCycleManager.Instance.List_SCR_timeToSwitch[0];

        //Ngeloop semua jadwal
        foreach (DayNightSchedule SCR_timeToSwitch in DayNightCycleManager.Instance.List_SCR_timeToSwitch)
        {
            //Kalau misalnya jadwalnya udah lewat dari waktu saat ini
            if (DT_localTime.Hour > SCR_timeToSwitch.SCR_timeToSwitch.I_hour ||
                (DT_localTime.Hour == SCR_timeToSwitch.SCR_timeToSwitch.I_hour && DT_localTime.Minute >= SCR_timeToSwitch.SCR_timeToSwitch.I_minute))
            {
                //Maka ganti current schedule jadi jadwal itu.
                SCR_currentSchedule = SCR_timeToSwitch;
            }
        }

        //Kalau misalnya ga sama ATAU kalau sebelumnya belum pernah berubah (previous schedule kosong), berarti lakukan transisi
        if (SCR_previousDayNightCycleSchedule == null ||
            SCR_currentSchedule.SCR_timeToSwitch.I_hour != SCR_previousDayNightCycleSchedule.SCR_timeToSwitch.I_hour ||
            SCR_currentSchedule.SCR_timeToSwitch.I_minute != SCR_previousDayNightCycleSchedule.SCR_timeToSwitch.I_minute)
        {
            SetupBackground(SCR_currentSchedule);
        }

        //Also jangan lupa ganti previous schedule jadi schedule saat ini biar bisa digunakan untuk transisi di masa depan.
        SCR_previousDayNightCycleSchedule = SCR_currentSchedule;
    }

    /// <summary>
    /// Cek apakah aset day night cycle yang sebelumnya sama dengan yang mau diubah saat ini. Kalau sama, maka ga usah ada transisi.
    /// </summary>
    /// <param name="CG_current">Aset saat ini</param>
    /// <param name="List_CG_allPrevious">Semua aset sebelumnya. Bakal dicek satu-satu.</param>
    /// <returns>True kalau ternyata aset sekarang sama dengan salah satu aset sebelumnya.</returns>
    public bool CheckIfPreviousScheduleIsTheSame(CanvasGroup CG_current, CanvasGroup[] List_CG_allPrevious)
    {
        foreach (CanvasGroup CG_previous in List_CG_allPrevious)
        {
            if (CG_previous.gameObject == CG_current.gameObject)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Function sebenarnya yang mengubah background. Function ini juga mengatur animasi transisinya.
    /// </summary>
    /// <param name="SCR_dayNightData"></param>
    private void SetupBackground(DayNightSchedule SCR_dayNightData)
    {
        //Ada ini karena function di atas bakal override SCR_previousDayNightCycleSchedule
        DayNightSchedule SCR_previousDayNightData = SCR_previousDayNightCycleSchedule;

        //Animasi untuk warna dekorasi pohon dan background
        if (SCR_previousDayNightData != null)
        {
            // Canvas group yang sebelumnya difade jadi transparan
            foreach (CanvasGroup canvasGroup in SCR_previousDayNightData.List_CG_canvasGroups)
            {
                // Kalau misalnya sama dengan yang saat ini, maka jangan fade out.
                // Tapi ini "if"nya ngecek kalau ga sama. 
                if (!CheckIfPreviousScheduleIsTheSame(canvasGroup, SCR_dayNightData.List_CG_canvasGroups))
                {
                    LeanTween
                        .alphaCanvas(canvasGroup, 0f, F_transitionDuration)
                        .setEase(LeanTweenType.easeInOutCubic);
                }
            }

            // Canvas group yang sekarang difade jadi kelihatan
            foreach (CanvasGroup canvasGroup in SCR_dayNightData.List_CG_canvasGroups)
            {
                // Kalau misalnya sama dengan yang sebelumnya, ga usah fade in soalnya emang udah kelihatan
                if (!CheckIfPreviousScheduleIsTheSame(canvasGroup, SCR_previousDayNightData.List_CG_canvasGroups))
                {
                    LeanTween
                        .alphaCanvas(canvasGroup, 1f, F_transitionDuration)
                        .setEase(LeanTweenType.easeInOutCubic);
                }
            }
        }
        //Kalau misalnya sebelumnya belum pernah ada perubahan apa pun, maka ga usah pakai animasi.
        else
        {
            foreach (CanvasGroup CG_canvasGroup in SCR_dayNightData.List_CG_canvasGroups)
            {
                CG_canvasGroup.alpha = 1f;
            }
        }

        // Karena Raden pasang spritenya pake codingan, jadi harus gini dulu untuk sementara
        SPR_currentSelectedPlaylistButton = SCR_dayNightData.SPR_playlistSelectedButton;
        if(SCR_UIPlaylist.IMG_selectedButton) SCR_UIPlaylist.IMG_selectedButton.sprite = SPR_currentSelectedPlaylistButton;
    }

    /// <summary>
    /// Ini tujuannya untuk menyembunyikan semua aset yang ada day night cyclenya. Anggap aja mereset semuanya biar yang bener bisa ditampilin.
    /// </summary>
    private void HideAll()
    {
        foreach (DayNightSchedule SCR_schedule in DayNightCycleManager.Instance.List_SCR_timeToSwitch)
        {
            foreach (CanvasGroup CG_canvasGroup in SCR_schedule.List_CG_canvasGroups)
            {
                CG_canvasGroup.alpha = 0f;
            }
        }
    }

    /// <summary>
    /// Function untuk ganti offset waktu local time saat ini. Untuk keperluan testing aja.
    /// 
    /// Ini dipakai di button testing.
    /// </summary>
    /// <param name="F_menit">Jumlah menit untuk memajukan atau memundurkan waktu. Kalau mau mundurin waktu, pakai angka negatif.</param>
    public void TestGantiOffsetWaktu(float F_menit)
    {
        F_offsetMenitTesting += F_menit;

        //Abis ganti, langsung cek background.
        ChangingBackground();
    }
}
