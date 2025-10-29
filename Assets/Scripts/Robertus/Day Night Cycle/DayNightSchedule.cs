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

    // Pakai canvas group untuk keperluan animasi transisinya. Canvas group itu untuk gambar ruangan. Seharusnya ini sementara karena aslinya bakal pakai 3D model semua.
    public CanvasGroup CG_backgroundCanvasGroup;
    public Color COL_backgroundColor;
}
