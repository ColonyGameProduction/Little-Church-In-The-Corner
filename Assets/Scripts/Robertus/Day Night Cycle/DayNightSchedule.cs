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

    /// <summary>
    /// Sekarang pakai list canvas group biar kalau ada tambahan, ga perlu repot-repot otak-atik codingan lagi.
    /// </summary>
    public CanvasGroup[] List_CG_canvasGroups;

    public Sprite SPR_playlistSelectedButton;
}
