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
            //Cara kerjanya: dia bakal fade out transparansi dari dekorasi sebelumnya dan fade in transparansi dari dekorasi saat ini.
            //"value", yaitu F_alphaValue, bakal berubah dari 1f ke 0f.
            //Makanya ada angka 1f di awal, dan 0f setelahnya
            LeanTween
            .value(1f, 0f, F_transitionDuration)
            .setEase(LeanTweenType.easeInOutCubic)
            .setOnUpdate((float F_alphaValue) =>
            {
                if(SCR_previousDayNightData.CG_playlistBackgroundCanvasGroup.gameObject != SCR_dayNightData.CG_playlistBackgroundCanvasGroup.gameObject)
                {
                    //playlist Background
                    SCR_previousDayNightData.CG_playlistBackgroundCanvasGroup.alpha = F_alphaValue;
                    SCR_dayNightData.CG_playlistBackgroundCanvasGroup.alpha = 1f - F_alphaValue;
                }

                if(SCR_previousDayNightData.CG_buttonPlaylistCanvasGroup.gameObject != SCR_dayNightData.CG_buttonPlaylistCanvasGroup.gameObject)
                {
                    //button
                    SCR_previousDayNightData.CG_buttonPlaylistCanvasGroup.alpha = F_alphaValue;
                    SCR_dayNightData.CG_buttonPlaylistCanvasGroup.alpha = 1f - F_alphaValue;
                }

                // Kalau misalnya yang sebelum dan yang sekarang sama, ga usah ada animasi
                if (SCR_previousDayNightData.CG_backgroundDecorationCanvasGroup.gameObject != SCR_dayNightData.CG_backgroundDecorationCanvasGroup.gameObject)
                {
                    //Dekorasi sebelumnya bakal berubah alphanya dari 1f ke 0f
                    SCR_previousDayNightData.CG_backgroundDecorationCanvasGroup.alpha = F_alphaValue;
                    //Dekorasi saat ini bakal berubah alphanya dari 0f ke 1f
                    SCR_dayNightData.CG_backgroundDecorationCanvasGroup.alpha = 1f - F_alphaValue;
                }

                if (SCR_previousDayNightData.CG_backgroundCanvasGroup.gameObject != SCR_dayNightData.CG_backgroundCanvasGroup.gameObject)
                {
                    //Background
                    SCR_previousDayNightData.CG_backgroundCanvasGroup.alpha = F_alphaValue;
                    SCR_dayNightData.CG_backgroundCanvasGroup.alpha = 1f - F_alphaValue;
                }

                //Header
                if (SCR_previousDayNightData.CG_header.gameObject != SCR_dayNightData.CG_header.gameObject)
                {
                    SCR_previousDayNightData.CG_header.alpha = F_alphaValue;
                    SCR_dayNightData.CG_header.alpha = 1f - F_alphaValue;
                }

                //Highlight slider buat button ruangan
                if (SCR_previousDayNightData.CG_roomButtonsHighlight.gameObject != SCR_dayNightData.CG_roomButtonsHighlight.gameObject)
                {
                    SCR_previousDayNightData.CG_roomButtonsHighlight.alpha = F_alphaValue;
                    SCR_dayNightData.CG_roomButtonsHighlight.alpha = 1f - F_alphaValue;
                }

                //Tombol ruangan office
                if (SCR_previousDayNightData.CG_officeRoomButton.gameObject != SCR_dayNightData.CG_officeRoomButton.gameObject)
                {
                    SCR_previousDayNightData.CG_officeRoomButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_officeRoomButton.alpha = 1f - F_alphaValue;
                }

                //Tombol ruangan gereja
                if (SCR_previousDayNightData.CG_churchRoomButton.gameObject != SCR_dayNightData.CG_churchRoomButton.gameObject)
                {
                    SCR_previousDayNightData.CG_churchRoomButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_churchRoomButton.alpha = 1f - F_alphaValue;
                }

                //Tombol ruangan kamar tidur
                if (SCR_previousDayNightData.CG_bedroomRoomButton.gameObject != SCR_dayNightData.CG_bedroomRoomButton.gameObject)
                {
                    SCR_previousDayNightData.CG_bedroomRoomButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_bedroomRoomButton.alpha = 1f - F_alphaValue;
                }

                //Tombol hamburger yang di samping
                if (SCR_previousDayNightData.CG_hamburgerButton.gameObject != SCR_dayNightData.CG_hamburgerButton.gameObject)
                {
                    SCR_previousDayNightData.CG_hamburgerButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_hamburgerButton.alpha = 1f - F_alphaValue;
                }

                //Saved Sermon: background luar
                if (SCR_previousDayNightData.CG_savedSermonOuterBackground.gameObject != SCR_dayNightData.CG_savedSermonOuterBackground.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonOuterBackground.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonOuterBackground.alpha = 1f - F_alphaValue;
                }

                //Saved Sermon: background dalam
                if (SCR_previousDayNightData.CG_savedSermonInnerBackground.gameObject != SCR_dayNightData.CG_savedSermonInnerBackground.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonInnerBackground.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonInnerBackground.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: tombol baca
                if (SCR_previousDayNightData.CG_savedSermonReadButton.gameObject != SCR_dayNightData.CG_savedSermonReadButton.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonReadButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonReadButton.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: background konfirmasi penghapusan
                if (SCR_previousDayNightData.CG_savedSermonDeleteConfirmationBackground.gameObject != SCR_dayNightData.CG_savedSermonDeleteConfirmationBackground.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonDeleteConfirmationBackground.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonDeleteConfirmationBackground.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: background pas baca renungannya
                if (SCR_previousDayNightData.CG_savedSermonReadBackground.gameObject != SCR_dayNightData.CG_savedSermonReadBackground.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonReadBackground.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonReadBackground.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: tombol done
                if (SCR_previousDayNightData.CG_savedSermonDoneButton.gameObject != SCR_dayNightData.CG_savedSermonDoneButton.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonDoneButton.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonDoneButton.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: list scrollbar
                if (SCR_previousDayNightData.CG_savedSermonListScrollbar.gameObject != SCR_dayNightData.CG_savedSermonListScrollbar.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonListScrollbar.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonListScrollbar.alpha = 1f - F_alphaValue;
                }

                //Saved sermon: chat scrollbar
                if (SCR_previousDayNightData.CG_savedSermonChatScrollbar.gameObject != SCR_dayNightData.CG_savedSermonChatScrollbar.gameObject)
                {
                    SCR_previousDayNightData.CG_savedSermonChatScrollbar.alpha = F_alphaValue;
                    SCR_dayNightData.CG_savedSermonChatScrollbar.alpha = 1f - F_alphaValue;
                }

                //Chat scrollbar
                if (SCR_previousDayNightData.CG_chatScrollbar.gameObject != SCR_dayNightData.CG_chatScrollbar.gameObject)
                {
                    SCR_previousDayNightData.CG_chatScrollbar.alpha = F_alphaValue;
                    SCR_dayNightData.CG_chatScrollbar.alpha = 1f - F_alphaValue;
                }
            });
        }
        //Kalau misalnya sebelumnya belum pernah ada perubahan apa pun, maka ga usah pakai animasi.
        else
        {
            SCR_dayNightData.CG_backgroundDecorationCanvasGroup.alpha = 1f;
            SCR_dayNightData.CG_backgroundCanvasGroup.alpha = 1f;
            SCR_dayNightData.CG_header.alpha = 1f;
            SCR_dayNightData.CG_roomButtonsHighlight.alpha = 1f;
            SCR_dayNightData.CG_officeRoomButton.alpha = 1f;
            SCR_dayNightData.CG_churchRoomButton.alpha = 1f;
            SCR_dayNightData.CG_bedroomRoomButton.alpha = 1f;
            SCR_dayNightData.CG_hamburgerButton.alpha = 1f;
            SCR_dayNightData.CG_savedSermonOuterBackground.alpha = 1f;
            SCR_dayNightData.CG_savedSermonInnerBackground.alpha = 1f;
            SCR_dayNightData.CG_savedSermonReadButton.alpha = 1f;
            SCR_dayNightData.CG_savedSermonDeleteConfirmationBackground.alpha = 1f;
            SCR_dayNightData.CG_savedSermonReadBackground.alpha = 1f;
            SCR_dayNightData.CG_savedSermonDoneButton.alpha = 1f;
            SCR_dayNightData.CG_savedSermonListScrollbar.alpha = 1f;
            SCR_dayNightData.CG_savedSermonChatScrollbar.alpha = 1f;
            SCR_dayNightData.CG_chatScrollbar.alpha = 1f;
            SCR_dayNightData.CG_playlistBackgroundCanvasGroup.alpha = 1f;
            SCR_dayNightData.CG_buttonPlaylistCanvasGroup.alpha = 1f;
        }
    }

    /// <summary>
    /// Ini tujuannya untuk menyembunyikan semua aset yang ada day night cyclenya. Anggap aja mereset semuanya biar yang bener bisa ditampilin.
    /// </summary>
    private void HideAll()
    {
        foreach (DayNightSchedule SCR_schedule in DayNightCycleManager.Instance.List_SCR_timeToSwitch)
        {
            SCR_schedule.CG_backgroundDecorationCanvasGroup.alpha = 0f;
            SCR_schedule.CG_backgroundCanvasGroup.alpha = 0f;
            SCR_schedule.CG_header.alpha = 0f;
            SCR_schedule.CG_roomButtonsHighlight.alpha = 0f;
            SCR_schedule.CG_officeRoomButton.alpha = 0f;
            SCR_schedule.CG_churchRoomButton.alpha = 0f;
            SCR_schedule.CG_bedroomRoomButton.alpha = 0f;
            SCR_schedule.CG_hamburgerButton.alpha = 0f;
            SCR_schedule.CG_savedSermonOuterBackground.alpha = 0f;
            SCR_schedule.CG_savedSermonInnerBackground.alpha = 0f;
            SCR_schedule.CG_savedSermonReadButton.alpha = 0f;
            SCR_schedule.CG_savedSermonDeleteConfirmationBackground.alpha = 0f;
            SCR_schedule.CG_savedSermonReadBackground.alpha = 0f;
            SCR_schedule.CG_savedSermonDoneButton.alpha = 0f;
            SCR_schedule.CG_savedSermonListScrollbar.alpha = 0f;
            SCR_schedule.CG_savedSermonChatScrollbar.alpha = 0f;
            SCR_schedule.CG_chatScrollbar.alpha = 0f;
            SCR_dayNightData.CG_playlistBackgroundCanvasGroup.alpha = 0f;
            SCR_dayNightData.CG_buttonPlaylistCanvasGroup.alpha = 0f;
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
