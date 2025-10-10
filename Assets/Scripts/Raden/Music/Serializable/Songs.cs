using UnityEngine;

[System.Serializable]
public class Songs
{
    public AudioClip ADO_music;
    public string S_titleAndAuthor;
    public AllEnum.ENM_MusicCode ENM_musicCode;

    // nantian ini tunggu ada playlist uinya duls hehe
    public void OnEnable()
    {

    }

    // hmm ini gua jujur ga tau ini buat apa jadi gua giniin duls
    public void PlayingThisSong()
    {
        Debug.Log($"Playing: {S_titleAndAuthor}");
    }
}