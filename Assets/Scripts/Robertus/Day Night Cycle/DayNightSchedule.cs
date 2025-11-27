using System;
using UnityEngine;

/// <summary>
/// Class yang berisi data jadwal pergantian warna background, warna background itu sendiri, serta canvas group untuk mengatur ruangan mana yang bakal dipakai.
/// </summary>
[Serializable]
public class DayNightSchedule
{
    /// <summary>
    /// Jadwal yang bisa dilihat dan diatur di Inspector
    /// </summary>
    public SerializedTime SCR_timeToSwitch;

    // Pakai canvas group untuk keperluan animasi transisinya. Canvas group itu untuk gambar dekorasi background 2D, yang ada pohon-pohonnya.
    public CanvasGroup CG_backgroundDecorationCanvasGroup;
    // Pakai canvas group untuk keperluan animasi transisinya. Canvas group yang ini untuk background beneran, yang ada warna dan gradasinya.
    public CanvasGroup CG_backgroundCanvasGroup;

    public CanvasGroup CG_header;
    public CanvasGroup CG_roomButtonsHighlight;
    public CanvasGroup CG_officeRoomButton;
    public CanvasGroup CG_churchRoomButton;
    public CanvasGroup CG_bedroomRoomButton;
    public CanvasGroup CG_hamburgerButton;
    public CanvasGroup CG_savedSermonOuterBackground;
    public CanvasGroup CG_savedSermonInnerBackground;
    public CanvasGroup CG_savedSermonReadButton;
    public CanvasGroup CG_savedSermonDeleteConfirmationBackground;
    public CanvasGroup CG_savedSermonReadBackground;
    public CanvasGroup CG_savedSermonDoneButton;
    public CanvasGroup CG_savedSermonListScrollbar;
    public CanvasGroup CG_savedSermonChatScrollbar;
    public CanvasGroup CG_chatScrollbar;
    //buat background playlist
    public CanvasGroup CG_playlistBackgroundCanvasGroup;
    //buat button pilih playlist
    public CanvasGroup CG_buttonPlaylistCanvasGroup;
}
