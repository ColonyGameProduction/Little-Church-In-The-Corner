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
        return new DateTime(1, 1, 1, I_hour, I_minute, 0);
    }
}
