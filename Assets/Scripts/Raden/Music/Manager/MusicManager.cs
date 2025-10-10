using System;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Data Playlist")]
    public ListOfPlaylistSO SO_listOfPlaylistSO;
    public PlaylistTypeSO SO_currPlaylistTypeSO;

    [Header("Current Song")]
    public Songs SCR_currSong;

    [Header("Para Enum")]
    public ENM_PauseAndPlay ENM_pauseAndPlay;
    public ENM_LoopMethod ENM_loopMethod;

    [Header("Condition")]
    public bool B_isShuffling = false;

    [Header("Event")]
    public Action<Songs> ACT_playSong;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (SO_currPlaylistTypeSO != null && SO_currPlaylistTypeSO.SCR_playlist.Count > 0)
        {
            SCR_currSong = SO_currPlaylistTypeSO.SCR_playlist[0];
            PlaySong(SCR_currSong);
        }
    }

    // ini fungsi nge nge play lagunya
    public void PlaySong(Songs songs)
    {
        if (songs == null)
        {
            return;
        }

        SCR_currSong = songs;
        audioSource.clip = songs.ADO_music;
        audioSource.Play();

        ENM_pauseAndPlay = ENM_PauseAndPlay.Play;
        AssigningSongTitleAndAuthor(songs);

        ACT_playSong?.Invoke(songs);
    }

    // nah ini buat setup tombol pause sama playnya
    public void TogglePauseAndPlayMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            ENM_pauseAndPlay = ENM_PauseAndPlay.Paused;
        }
        else
        {
            audioSource.Play();
            ENM_pauseAndPlay = ENM_PauseAndPlay.Play;
        }
    }

    // ini buat setup tombol shuffle
    public void ToggleShuffle()
    {
        B_isShuffling = !B_isShuffling;
    }

    // ini setup buat metode loop (no loop, loop, loop playlist)
    public void ToggleLoopMethod()
    {
        ENM_loopMethod = (ENM_LoopMethod)(((int)ENM_loopMethod + 1) % Enum.GetValues(typeof(ENM_LoopMethod)).Length);
    }

    // setup buat fungsi next song
    public void ToggleNextSong()
    {
        if (SO_currPlaylistTypeSO == null)
        {
            return;
        }

        var list = SO_currPlaylistTypeSO.SCR_playlist;
        int I_currIndex = list.IndexOf(SCR_currSong);

        int I_nextIndex;

        if (B_isShuffling)
        {
            I_nextIndex = UnityEngine.Random.Range(0, list.Count);
        }
        else
        {
            I_nextIndex = (I_currIndex + 1) % list.Count;
        }

        PlaySong(list[I_nextIndex]);
    }

    // ini setup buat fungsi prev song
    public void TogglePrevSong()
    {
        if (SO_currPlaylistTypeSO == null)
        {
            return;
        }

        var list = SO_currPlaylistTypeSO.SCR_playlist;
        int I_currIndex = list.IndexOf(SCR_currSong);

        int I_prevIndex;

        if (B_isShuffling)
        {
            I_prevIndex = UnityEngine.Random.Range(0, list.Count);
        }
        else
        {
            I_prevIndex = (I_currIndex - 1 + list.Count) % list.Count;
        }

        PlaySong(list[I_prevIndex]);
    }

    // ini fungsi buat nampilin lagunya judulnya apa terus siapa pemiliknya
    public void AssigningSongTitleAndAuthor(Songs songs)
    {
        Debug.Log($"Now Playing: {songs.S_titleAndAuthor}");
    }

    // ini setup buat slider progress lagu udah nyanyi ampe mana
    public void ProgressBarLogic(Slider slider)
    {
        if (audioSource.clip == null)
        {
            return;
        }

        slider.value = audioSource.time / audioSource.clip.length;

        // kalo lagu abis
        if (!audioSource.isPlaying && audioSource.time >= audioSource.clip.length)
        {
            HandleSongEnd();
        }
    }

    // ini nentuin pas lagu selesai dia bakal kanjut kah atau ngeloop kah atau ngeloop tapi playlistnya yang di loop
    private void HandleSongEnd()
    {
        switch (ENM_loopMethod)
        {
            case ENM_LoopMethod.NoLoop:
                ToggleNextSong();
                break;

            case ENM_LoopMethod.LoopSong:
                PlaySong(SCR_currSong);
                break;

            case ENM_LoopMethod.LoopPlaylist:
                ToggleNextSong();
                break;
        }
    }

    // ini metode ngesave lagu terakhir yang di puter hihih (tapi ini metode penyimpanan sementara)
    public void SaveSongData()
    {
        PlayerPrefs.SetString("LastSong", SCR_currSong.S_titleAndAuthor);
        PlayerPrefs.Save();
    }
}