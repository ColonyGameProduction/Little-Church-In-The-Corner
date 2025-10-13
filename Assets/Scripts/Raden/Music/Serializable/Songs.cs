using UnityEngine;

[System.Serializable]
public class Songs
{
    public AudioClip ADO_music;
    public string S_titleAndAuthor;
    public ENM_MusicCode ENM_musicCode;

    // nantian ini tunggu ada playlist uinya duls hehe
    public void OnEnable()
    {

    }

    //Justru action nya dipanggil disini den (okay)
    public void PlayingThisSong()
    {
        // udah di ganti ya seyenkkk
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ACT_playSong?.Invoke(this);
        }
    }
}