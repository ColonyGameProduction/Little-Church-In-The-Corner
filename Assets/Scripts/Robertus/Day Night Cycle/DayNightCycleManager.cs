using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class untuk mengatur sistem day night cycle. Isinya cuma jadwal kapan ganti warna background (untuk sekarang).
/// </summary>
public class DayNightCycleManager : MonoBehaviour
{
    public static DayNightCycleManager Instance { get; private set; }

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

    /// <summary>
    /// List jadwal pergantian warna background
    /// </summary>
    public List<DayNightSchedule> List_SCR_timeToSwitch;
    /// <summary>
    /// Reference ke DayNightCycleUI, biar bisa panggil ChangingBackground
    /// </summary>
    public DayNightCycleUI SCR_dayNightCycleUI;
}
