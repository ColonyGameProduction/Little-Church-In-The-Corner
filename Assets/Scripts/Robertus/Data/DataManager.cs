using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Class untuk mengatur segala hal yang berkaitan dengan data, save, dan load.
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    /// <summary>
    /// Waktu terakhir login.
    /// </summary>
    private DateTime DT_lastLogin;
    /// <summary>
    /// Kalau misalnya ada update renungan baru
    /// </summary>
    public TextAsset TXA_newSermonText;

    /// <summary>
    /// Angka random buat dijadiin key pas encrypt save file. PASTIKAN TIDAK BERUBAH, soalnya kalau berubah, file-file yang telah terenkripsi sebelumnya ga bakal bisa dibalikin lagi.
    /// </summary>
    private const int I_KEY = 723;

    /// <summary>
    /// Ini lokasi tempat penyimpanan SCR_schedule di device pemain
    /// </summary>
    private string S_scheduleFilePath;

    /// <summary>
    /// Action untuk diinvoke kalau udah selesai ngeload data dari save file.
    /// Tujuannya biar class yang butuh data itu nunggu dulu sampai ngeloadnya selesai.
    /// </summary>
    public static event Action ACT_loadDone;

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

        S_scheduleFilePath = Path.Combine(Application.persistentDataPath, "Data", "SCR_schedule.data"); ;
    }

    private void Start()
    {
        LoadAllData();
    }

    /// <summary>
    /// Save semua jenis data, seperti last login, SCR_schedule, queue, dst.
    /// </summary>
    public void SaveAllData()
    {
        //Ini kapan dipanggilnya btw?
        //Ini kapan dipanggilnya btw?
        //Ini kapan dipanggilnya btw?
        //Ini kapan dipanggilnya btw?
        //Ini kapan dipanggilnya btw?

        DT_lastLogin = DateTime.Now;

        //Debug.LogError("WARNING: TESTING LAST LOGIN A LONG TIME AGO");
        //DT_lastLogin = DT_lastLogin.AddDays(-1);

        PlayerPrefs.SetString("DT_lastLogin", DT_lastLogin.ToString());

        //Schedule
        SaveXOR(S_scheduleFilePath, JsonUtility.ToJson(new TimeManager.ListOfSchedule(TimeManager.Instance.List_SCR_sermonSchedule)));

        //Queue
        PlayerPrefs.SetInt("I_queuedSermon", TimeManager.Instance.I_queuedSermon);
    }

    /// <summary>
    /// Load semua data yang ada di save file
    /// </summary>
    public void LoadAllData()
    {
        string S_lastLogin = PlayerPrefs.GetString("DT_lastLogin");

        //Kalau misalnya ternyata ga ada save file di PlayerPref, maka pakai waktu saat ini.
        if(string.IsNullOrEmpty(S_lastLogin)) DT_lastLogin = DateTime.Now;
        else DT_lastLogin = DateTime.Parse(S_lastLogin);

        Debug.Log($"Last login is {DT_lastLogin}");

        //Queue
        TimeManager.Instance.I_queuedSermon = PlayerPrefs.GetInt("I_queuedSermon");
        //Hitung berapa hari sejak terakhir login.
        int I_daysSinceLastLogin = ((TimeSpan)(DateTime.Now - DT_lastLogin)).Days;

        //Schedule sebentar
        //Kalau misalnya last loginnya hari ini, load dari save file
        //else, bikin SCR_schedule baru buat hari ini
        if (I_daysSinceLastLogin <= 0) LoadSchedule(S_scheduleFilePath);
        else TimeManager.Instance.SetupListSchedule();

        Debug.Log($"It has been {I_daysSinceLastLogin} days since last login.");

        //Hitung berapa banyak renungan yang mungkin terlewat dari last login sampai sekarang.
        //Misal, kalau sudah 4 hari ga login, maka sudah pasti 3 di antara 4 hari itu adalah 1 hari full, sehingga 3 kali banyaknya jadwal renungan per hari, yaitu 5. 3 x 5 = 15 renungan yang terlewat.
        //Mengapa ga 4 hari? Karena bisa saja hari pertama dia last loginnya di tengah atau akhir hari. Technically itu masih termasuk 1 hari. Jadi, itu nanti bakal dihitung di bawah.
        int I_missedSermons = (I_daysSinceLastLogin - 1) * TimeManager.Instance.List_SCR_sermonSchedule.Count;

        Debug.Log($"You missed {I_missedSermons} sermons");

        TimeManager.Instance.I_queuedSermon += I_missedSermons;

        //Kalau misalnya masih ada ruang di queue DAN terakhir kali login lebih dari 0 hari yang lalu (kemarin, dua hari yang lalu, dst.)
        if (I_daysSinceLastLogin > 0 && TimeManager.Instance.I_queuedSermon < TimeManager.Instance.I_maxQueuedSermon)
        {
            //Masih di bawah max
            //Hitung hari pas last login dengan lebih teliti

            foreach (Schedule SCR_schedule in TimeManager.Instance.List_SCR_sermonSchedule)
            {
                //Kalau misalnya jadwalnya udah lewat dari waktu pas last login
                if (DT_lastLogin.Hour > SCR_schedule.DT_time.Hour ||
                    (DT_lastLogin.Hour == SCR_schedule.DT_time.Hour && DT_lastLogin.Minute >= SCR_schedule.DT_time.Minute))
                {
                    Debug.Log($"Schedule = {SCR_schedule} vs last login = {DT_lastLogin}");
                    TimeManager.Instance.I_queuedSermon++;
                }
            }
        }

        ACT_loadDone?.Invoke();
    }

    /// <summary>
    /// Ngeload data SCR_schedule dari device pemain, kalau ada.
    /// Kalau ga ada, atur SCR_schedule default.
    /// </summary>
    /// <param name="S_filePath">Lokasi tempat penyimpanan</param>
    private void LoadSchedule(string S_filePath)
    {
        //Kalau ternyata ada filenya:
        if (File.Exists(S_filePath))
        {
            //Ambil isi save file yang telah terenkripsi sebelumnya
            string S_encryptedString = File.ReadAllText(S_filePath);

            //Diconvert ke teks yang bisa dibaca (JSON)
            string S_decryptedString = S_EncryptDecrypt(S_encryptedString);

            //Diconvert ke class ListOfSchedule.
            TimeManager.ListOfSchedule SCR_loadedData = JsonUtility.FromJson<TimeManager.ListOfSchedule>(S_decryptedString);

            //Kalau misalnya konversinya berhasil
            if (SCR_loadedData != null)
            {
                Debug.Log(SCR_loadedData);
                //Taro hasil konversinya ke dalam List asli, yang bisa dibaca dan diakses oleh class lain
                TimeManager.Instance.List_SCR_sermonSchedule = SCR_loadedData.List_schedules;

                //Also convert Unix ke DateTime biar bisa dibaca oleh class lain.
                foreach (Schedule SCR_schedule in TimeManager.Instance.List_SCR_sermonSchedule)
                {
                    SCR_schedule.ConvertUnixToDateTime();
                    //Debug.Log(SCR_schedule);
                }
            }
            else
            {
                Debug.LogError("ERROR: Data cannot be loaded");
            }
        }
        else
        {
            //Kalau misalnya lom ada save file, maka set SCR_schedule seperti biasa.
            Debug.LogWarning("WARNING: File doesn't exist");
            TimeManager.Instance.SetupListSchedule();
            //foreach (Schedule SCR_schedule in TimeManager.Instance.List_SCR_sermonSchedule)
            //{
            //    Debug.Log(SCR_schedule);
            //}
        }
    }

    public void ResetAll()
    {

    }

    public void UpdateData()
    {

    }

    #region Encryption Algorithm
    //Semuanya diambil dari link ini, dengan beberapa modifikasi.
    //https://discussions.unity.com/t/saving-loading-with-encryption/589974

    //Takes string, encrypts string and then saves it to file
    public void SaveXOR(string S_filePath, string S_data)
    {
        FileInfo fileInfo = new FileInfo(S_filePath);
        fileInfo.Directory.Create(); // If the directory already exists, this method does nothing.

        //Kalau ga ada directorynya, ini bakal error
        //Makanya di atas ada Directory.Create()
        StreamWriter streamWriter = new StreamWriter(S_filePath);
        streamWriter.Write(S_EncryptDecrypt(S_data));
        streamWriter.Flush();
        streamWriter.Close();
    }

    //XOR encryption by key, basiclly it takes ASCII code of character and ^ by key, does that to each character of string
    public string S_EncryptDecrypt(string S_textToEncrypt)
    {
        Debug.Log("Encrypting/Decrypting\n" + S_textToEncrypt);
        StringBuilder SB_inSb = new StringBuilder(S_textToEncrypt);
        StringBuilder SB_outSb = new StringBuilder(S_textToEncrypt.Length);
        char c;
        for (int I_inputIndex = 0; I_inputIndex < S_textToEncrypt.Length; I_inputIndex++)
        {
            c = SB_inSb[I_inputIndex];
            c = (char)(c ^ I_KEY);
            SB_outSb.Append(c);
        }
        Debug.Log("Result\n" + SB_outSb.ToString());
        return SB_outSb.ToString();
    }
    #endregion
}
