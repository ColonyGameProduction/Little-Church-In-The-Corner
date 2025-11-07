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
}
