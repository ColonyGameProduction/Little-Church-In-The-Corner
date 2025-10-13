using System;

/// <summary>
/// Jadwal renungan. Isinya waktu renungan baru dan juga apakah jadwalnya sudah pernah dijalanin atau belum.
/// </summary>
public class Schedule
{
    public DateTime DT_time;
    public bool B_hasBeenShown;

    public Schedule(DateTime dT_time, bool b_hasBeenShown)
    {
        DT_time = dT_time;
        B_hasBeenShown = b_hasBeenShown;
    }
}