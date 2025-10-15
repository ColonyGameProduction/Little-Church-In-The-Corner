using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Songs : MonoBehaviour
{
    public AudioClip ADO_music;
    public string S_titleAndAuthor;
    public ENM_MusicCode ENM_musicCode;

    private Button BTN_self;

    public void OnEnable()
    {
        // BTW disini aja den buat OnClick Button nya
        // jadi lu pas instantiate button nya dari UIMusicManager, kasih komponent script ini
        // trus disini lu kasih onclick nya si PlayingThis Song yang dibawah
        // jadi lu add listener nya ditaro di UIPlaylist

        BTN_self = GetComponent<Button>();

        if (BTN_self != null)
        {
            BTN_self.onClick.RemoveAllListeners();

            BTN_self.onClick.AddListener(() =>
            {
                if (MusicManager.Instance != null)
                {
                    MusicManager.Instance.PlaySong(this);
                }
            });
        }
    }

    public void PlayingThisSong()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ACT_playSong?.Invoke(this);
        }
    }
}