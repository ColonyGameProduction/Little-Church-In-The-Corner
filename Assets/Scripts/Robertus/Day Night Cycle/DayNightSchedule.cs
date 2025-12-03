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

    /// <summary>
    /// Tombol selection. Pakai ini dan bukan canvas group karena Raden pasang selected button dengan codingan
    /// </summary>
    public Sprite SPR_playlistSelectedButton;

    /// <summary>
    /// Ini warna light rays untuk waktu itu
    /// </summary>
    public Color COL_lightRaysColor;
    
    /// <summary>
    /// Warna lighting di seluruh scene
    /// </summary>
    public Color COL_lightingColor;

    /// <summary>
    /// Warna spotlight di setiap ruangan.
    /// </summary>
    public Color COL_spotlightColor;

    /// <summary>
    /// Seberapa terang pencahayaan utama
    /// </summary>
    public float F_mainLightingIntensity;

    /// <summary>
    /// Seberapa terang pencahayaan kedua (buat terangin tembok dkk.)
    /// </summary>
    public float F_secondaryLightingIntensity;
}
