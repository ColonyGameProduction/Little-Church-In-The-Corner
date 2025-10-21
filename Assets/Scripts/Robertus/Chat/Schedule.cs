using System;
using UnityEngine;

/// <summary>
/// Jadwal renungan. Isinya waktu renungan baru dan juga apakah jadwalnya sudah pernah dijalanin atau belum.
/// </summary>
[Serializable]
public class Schedule
{
    public DateTime DT_time;
    /// <summary>
    /// Ada ini biar kalau disave dan diload, bisa convert ini ke DateTime.
    /// </summary>
    public long L_unixTimecode;
    public bool B_hasBeenShown;

    public Schedule(DateTime dT_time, bool b_hasBeenShown)
    {
        DT_time = dT_time;
        B_hasBeenShown = b_hasBeenShown;
        L_unixTimecode = ((DateTimeOffset)DT_time).ToUnixTimeMilliseconds();

        //Debug.Log(DT_time);
        //Debug.Log(L_unixTimecode);
        //Debug.Log(DateTimeOffset.FromUnixTimeMilliseconds(L_unixTimecode).ToLocalTime());
        //ConvertUnixToDateTime();
        //Debug.Log("Done");
    }

    /// <summary>
    /// Ini buat convert L_unixTimeCode ke DateTime
    /// </summary>
    public void ConvertUnixToDateTime()
    {
        DT_time = DateTimeOffset.FromUnixTimeMilliseconds(L_unixTimecode).ToLocalTime().DateTime;
        Debug.Log($"Converted from {L_unixTimecode} to {DT_time}");
    }

    /// <summary>
    /// Cuma biar pas diprint/Debug.Log, jadi rapi dan jelas aja.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Shown? {B_hasBeenShown} | {DT_time} | {L_unixTimecode}";
    }
}