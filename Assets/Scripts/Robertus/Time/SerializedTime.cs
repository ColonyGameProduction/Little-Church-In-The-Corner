using System;
using UnityEngine;

/// <summary>
/// Waktu dalam satuan JAM:MENIT
/// Ada ini biar bisa ditampilin di custom inspector
/// </summary>
[Serializable]
public class SerializedTime
{
    public int I_hour;
    public int I_minute;

    public DateTime DT_ToDateTime()
    {
        DateTime DT_now = DateTime.Now;
        return new DateTime(DT_now.Year, DT_now.Month, DT_now.Day, I_hour, I_minute, 0);
    }
}
