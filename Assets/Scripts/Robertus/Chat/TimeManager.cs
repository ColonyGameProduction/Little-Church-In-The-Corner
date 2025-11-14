using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mengatur logic waktu dan penjadwalan renungan
/// </summary>
public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    /// <summary>
    /// Waktu saat ini, tergantung device pemain
    /// </summary>
    public DateTime DT_localTime;
    /// <summary>
    /// Kalau waktunya sesuai dengan jadwal, maka trigger action ini
    /// </summary>
    public static event Action ACT_interactIsReady;

    /// <summary>
    /// Kalau waktunya sesuai dengan jadwal QnA, maka trigger action ini
    /// </summary>
    public static event Action ACT_interactQnAIsReady;

    /// <summary>
    /// Jadwal yang bisa dilihat dan diatur di Inspector
    /// </summary>
    public SerializedTime[] SCR_sermonSchedule;

    /// <summary>
    /// Jadwal aslinya, nanti diconvert dari SCR_sermonSchedule
    /// </summary>
    [HideInInspector] public List<Schedule> List_SCR_sermonSchedule;

    /// <summary>
    /// Jadwal QnA di kantor yang bisa dilihat dan diatur di Inspector
    /// </summary>
    public SerializedTime[] SCR_qnaSchedule;

    /// <summary>
    /// Jadwal aslinya, nanti diconvert dari SCR_qnaSchedule
    /// </summary>
    [HideInInspector] public List<Schedule> List_SCR_qnaSchedule;

    /// <summary>
    /// Ini untuk yield new return WaitForSeconds, tapi karena delaynya selalu 1 menit (jadi cuma ngecek ada renungan baru atau engga setiap satu menit), jadi diset di awal aja.
    /// </summary>
    private WaitForSeconds WFS_oneMinuteTimer;

    /// <summary>
    /// Banyak renungan yang ada di queue saat ini.
    /// </summary>
    private int i_queuedSermon;

    /// <summary>
    /// Banyak renungan yang ada di queue saat ini.
    /// Pakai property seperti ini biar pas ngeset valuenya, dia bisa pastiin kalau valuenya ga pernah negatif.
    /// </summary>
    public int I_queuedSermon
    {
        get
        {
            //Debug.Log($"Sermon queued: {i_queuedSermon}");
            return i_queuedSermon;
        }
        set
        {
            //Debug.Log($"Adding sermon, from {i_queuedSermon} to {value}");
            //Memastikan jumlah renungan yang ada di dalam queue di antara 0 dan batas maksimum
            i_queuedSermon = Mathf.Clamp(value, 0, I_maxQueuedSermon);
        }
    }

    /// <summary>
    /// Batas maksimum renungan yang boleh ada di queue. Selebihnya bakal hangus.
    /// </summary>
    public int I_maxQueuedSermon = 10;

    /// <summary>
    /// Banyak QnA kantor yang ada di queue saat ini.
    /// </summary>
    private int i_queuedQnA;

    /// <summary>
    /// Banyak QnA kantor yang ada di queue saat ini.
    /// Pakai property seperti ini biar pas ngeset valuenya, dia bisa pastiin kalau valuenya ga pernah negatif.
    /// </summary>
    public int I_queuedQnA
    {
        get
        {
            //Debug.Log($"QnA queued: {i_queuedQnA}");
            return i_queuedQnA;
        }
        set
        {
            //Debug.Log($"Adding QnA, from {i_queuedQnA} to {value}");
            //Memastikan jumlah QnA kantor yang ada di dalam queue di antara 0 dan batas maksimum
            i_queuedQnA = Mathf.Clamp(value, 0, I_maxQueuedQnA);
        }
    }

    /// <summary>
    /// Batas maksimum QnA kantor yang boleh ada di queue. Selebihnya bakal hangus.
    /// </summary>
    public int I_maxQueuedQnA = 3;

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

    private void OnEnable()
    {
        DataManager.ACT_loadDone += StartWaitForRenungan;
    }

    private void OnDisable()
    {
        DataManager.ACT_loadDone -= StartWaitForRenungan;
    }

    private void StartWaitForRenungan()
    {
        StartCoroutine(WaitForRenungan());
    }

    /// <summary>
    /// Dulunya ini ada di Start(), tapi karena sekarang ada fitur save/load, ini diatur di sana.
    /// Kalau misalnya ga ada save file atau last loginnya kemarin-kemarin, bikin list schedule baru.
    /// </summary>
    public void SetupListSchedule()
    {
        //Convert jadwal yang diset di Inspector ke jadwal yang bisa diedit dan semacamnya.
        List_SCR_sermonSchedule = new List<Schedule>();
        foreach (SerializedTime time in SCR_sermonSchedule)
        {
            List_SCR_sermonSchedule.Add(new Schedule(time.DT_ToDateTime(), false));
        }

        List_SCR_qnaSchedule = new List<Schedule>();
        foreach (SerializedTime time in SCR_qnaSchedule)
        {
            List_SCR_qnaSchedule.Add(new Schedule(time.DT_ToDateTime(), false));
        }
    }

    /// <summary>
    /// Ini untuk keperluan save file. Cuma wrapper untuk List<\Schedule>
    /// </summary>
    [Serializable]
    public class ListOfSchedule
    {
        public List<Schedule> List_schedules;

        public ListOfSchedule(List<Schedule> list_schedules)
        {
            List_schedules = list_schedules;
        }
    }

    /// <summary>
    /// Cek renungan dari jadwal setiap satu menit.
    /// Ini dijalankan setelah selesai loading data dari save file.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRenungan()
    {
        WFS_oneMinuteTimer = new WaitForSeconds(60);
        while (true)
        {
            CheckForRenungan();
            CheckForQnA();
            DayNightCycleManager.Instance.SCR_dayNightCycleUI.ChangingBackground();
            yield return WFS_oneMinuteTimer;
        }
    }

    /// <summary>
    /// Cek apakah bisa akses renungan baru berdasarkan jadwal dan waktu saat ini
    /// </summary>
    private void CheckForRenungan()
    {
        //Lagi ada renungan yang jalan, jadi jangan mulai renungan lain
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;

        DT_localTime = DateTime.Now;

        foreach (Schedule schedule in List_SCR_sermonSchedule)
        {
            //Kalau jadwalnya udah pernah dijalanin sebelumnya, lewatin aja
            if (schedule.B_hasBeenShown) continue;

            //Kalau misalnya jadwalnya udah lewat dari waktu saat ini dan belum dijalanin
            if(DT_localTime.Hour > schedule.DT_time.Hour ||
                (DT_localTime.Hour == schedule.DT_time.Hour && DT_localTime.Minute >= schedule.DT_time.Minute))
            {
                schedule.B_hasBeenShown = true;
                //Tambah renungan di dalam queue
                I_queuedSermon++;
            }
        }

        //Kalau misalnya ada renungan di dalam queue, setup renungan dan tampilin tombol interact
        if(I_queuedSermon > 0) RenunganIsReady();
    }

    /// <summary>
    /// Cek apakah bisa akses QnA kantor baru berdasarkan jadwal dan waktu saat ini
    /// </summary>
    private void CheckForQnA()
    {
        //Lagi ada renungan yang jalan, jadi jangan mulai renungan lain
        if (ChatManager.Instance.ENM_currDialog != ENM_DialogTitle.None)
            return;

        DT_localTime = DateTime.Now;

        foreach (Schedule schedule in List_SCR_qnaSchedule)
        {
            //Kalau jadwalnya udah pernah dijalanin sebelumnya, lewatin aja
            if (schedule.B_hasBeenShown) continue;

            //Kalau misalnya jadwalnya udah lewat dari waktu saat ini dan belum dijalanin
            if (DT_localTime.Hour > schedule.DT_time.Hour ||
                (DT_localTime.Hour == schedule.DT_time.Hour && DT_localTime.Minute >= schedule.DT_time.Minute))
            {
                schedule.B_hasBeenShown = true;
                //Tambah renungan di dalam queue
                I_queuedQnA++;
            }
        }

        //Kalau misalnya ada renungan di dalam queue, setup renungan dan tampilin tombol interact
        if (I_queuedQnA > 0) QnAIsReady();
    }

    /// <summary>
    /// Kalau renungan udah siap, invoke action (setup renungan dan tampilin tombol interact)
    /// </summary>
    private void RenunganIsReady()
    {
        ACT_interactIsReady?.Invoke();
    }

    /// <summary>
    /// Kalau QnA udah siap, invoke action (setup renungan dan tampilin tombol interact)
    /// </summary>
    private void QnAIsReady()
    {
        ACT_interactQnAIsReady?.Invoke();
    }

    public void TestTambahRenunganBaru()
    {
        if (TransitionManager.Instance.ENM_room == ENM_Room.Office)
        {
            I_queuedQnA++;
            QnAIsReady();
        }
        else if(TransitionManager.Instance.ENM_room == ENM_Room.Church)
        {
            I_queuedSermon++;
            RenunganIsReady();
        }
        else
        {
            Debug.Log("Ga bisa nambah renungan baru! Pindah ke ruangan yang diinginkan, baru coba tambah lagi.");
        }
    }
}
