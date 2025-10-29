using SFB; // standalon file browser (buat buka file explorer)
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable] // ini tuh data buat nyimpen inpo satu local musik
public class LocalSongData
{
    public string S_filePath;
    public string S_titleAndAuthor;
    public ENM_MusicCode ENM_musicCode;
}

[System.Serializable] // ini tuh wrapper buat serialize list local musik ke JSON
public class LocalSongsWrapper
{
    public List<LocalSongData> SCR_songs = new List<LocalSongData>();
}

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

    [Header("Local Songs")] // ini tuh list local musik yang udah di add ke dalem game
    public List<LocalSongData> SCR_localSongs = new List<LocalSongData>();
    private string S_savePath => Path.Combine(Application.persistentDataPath, "local_songs.json");

    [Header("Playlist Lokal")]
    public PlaylistTypeSO SO_playlistLocal;

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
        //ACT_playSong += PlaySong();
        if (SO_currPlaylistTypeSO != null && SO_currPlaylistTypeSO.SCR_playlist.Count > 0)
        {
            SCR_currSong = SO_currPlaylistTypeSO.SCR_playlist[0];
            PlaySong(SCR_currSong);
        }

        // load semua local song yang udah ditambah sama pengguna
        LoadLocalSongs();
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

        // udah diganti ya seyenkkk
        //songs.PlayingThisSong();
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

    // ngubah posisi lagu sesuai value slider
    public void OnSliderValueChanged(Slider slider)
    {
        if (audioSource.clip == null) 
        {
            return;
        }

        audioSource.time = slider.value * audioSource.clip.length;
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

    // fungsi ini tuh buat buka file browser dan pilih2 lagu dari local storage pengguna
    public void ImportLocalSong()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Music File", "", new[] {
        new ExtensionFilter("Audio Files", "mp3", "wav", "ogg")
        }, false);

        if (paths.Length == 0) return;
        string path = paths[0];
        StartCoroutine(LoadAndAddLocalSong(path));
    }

    // ini korutin buat nge-add file audio dari local path, terus namahin ke Playlist_Local abis itu di simpen di JSON
    private IEnumerator LoadAndAddLocalSong(string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load: " + path);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

            Songs newSong = new Songs
            {
                ADO_music = clip,
                S_titleAndAuthor = Path.GetFileNameWithoutExtension(path),
                ENM_musicCode = ENM_MusicCode.SongLocal1
            };

            // Tambahin ke playlist local
            SO_playlistLocal.SCR_playlist.Add(newSong);

            // Simpen path ke list local
            SCR_localSongs.Add(new LocalSongData
            {
                S_filePath = path,
                S_titleAndAuthor = newSong.S_titleAndAuthor,
                ENM_musicCode = newSong.ENM_musicCode
            });

            SaveLocalSongs();

            FindAnyObjectByType<UIPlaylist>()?.SetupAllPlaylistSongs(SO_currPlaylistTypeSO.ENM_playlistType);
        }
    }
    
    // ini fungsi buat nyimpen semua local musik ke file JSON di local storage, jadi ini aktif pas pengguna abis nge-add musicnya
    public void SaveLocalSongs()
    {
        LocalSongsWrapper wrapper = new LocalSongsWrapper { SCR_songs = SCR_localSongs };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(S_savePath, json);
        Debug.Log("Local songs saved: " + S_savePath);
    }

    // ini fungsi buat nge-add semua local musik dari JSON pas gamenya mulai
    public void LoadLocalSongs()
    {
        if (!File.Exists(S_savePath)) return;

        SO_playlistLocal.SCR_playlist.Clear();

        string json = File.ReadAllText(S_savePath);
        LocalSongsWrapper wrapper = JsonUtility.FromJson<LocalSongsWrapper>(json);

        foreach (var songData in wrapper.SCR_songs)
        {
            StartCoroutine(LoadAudioFromPath(songData));
        }
    }

    // ini buat nge-add 1 file audio sesuai data di JSON, ini tuh dipanggil sama LoadAudioFromPath() buat tiap musik
    private IEnumerator LoadAudioFromPath(LocalSongData data)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + data.S_filePath, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                Songs newSong = new Songs
                {
                    ADO_music = clip,
                    S_titleAndAuthor = data.S_titleAndAuthor,
                    ENM_musicCode = data.ENM_musicCode
                };
                SO_playlistLocal.SCR_playlist.Add(newSong);
            }
        }
    }
}