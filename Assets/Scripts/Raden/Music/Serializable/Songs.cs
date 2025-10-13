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
        // BTW disini aja den buat OnClick Button nya
        // jadi lu pas instantiate button nya dari UIMusicManager, kasih komponent script ini
        // trus disini lu kasih onclick nya si PlayingThis Song yang dibawah
        // jadi lu add listener nya ditaro di UIPlaylist
    }

    
    public void PlayingThisSong()
    {
        // udah di ganti ya seyenkkk
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ACT_playSong?.Invoke(this);
        }
    }
}