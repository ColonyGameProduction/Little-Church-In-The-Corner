using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMusicManager : MonoBehaviour
{
    public MusicManager SCR_MM;

    [Header("Para GameObject")]
    public GameObject GO_shuffle;
    public GameObject GO_loop;
    public GameObject GO_pauseAndPlay;
    public GameObject GO_next;
    public GameObject GO_prev;
    public GameObject GO_playlist;

    [Header("Para Button")]
    public Button BTN_shuffle;
    public Button BTN_shuffleForPlaylist;
    public Button BTN_loop;
    public Button BTN_loopForPlaylist;
    public Button BTN_pauseAndPlayMini;
    public Button BTN_pauseAndPlayFull;
    public Button BTN_pauseAndPlayPlaylist;
    public Button BTN_next;
    public Button BTN_nextForPlaylist;
    public Button BTN_prev;
    public Button BTN_prevForPlaylist;
    public Button BTN_playlist;

    [Header("Semua UI Element")]
    //public Image IMG_playMethod;
    //public Image IMG_pauseAndPlay;
    public TextMeshProUGUI TMPGUI_titleAndAuthor;
    public Slider SLR_progressBar;

    [Header("shuffle icons")]
    public Sprite SPR_shuffleOn;
    public Sprite SPR_shuffleOff;
    public Image IMG_shuffleButton;
    public Image IMG_shuffleButtonForPlaylist;

    [Header("loop icons")]
    public Sprite SPR_NoLoop;
    public Sprite SPR_loopSong;
    public Sprite SPR_loopPlaylist;
    public Image IMG_loopButton;
    public Image IMG_loopButtonForPlaylist;

    [Header("Pause Play Icons")]
    public Image IMG_pauseAndPlayIconMini;
    public Image IMG_pauseAndPlayIconFull;
    public Image IMG_pauseAndPlayIconPlaylist;
    private Sprite SPR_currentPlaySprite;
    private Sprite SPR_currentPauseSprite;

    private void Start()
    {
        // ini setupan buat button listener yak
        BTN_shuffle.onClick.AddListener(() => SetupShuffleButton());
        BTN_shuffleForPlaylist.onClick.AddListener(() => SetupShuffleButton());
        BTN_loop.onClick.AddListener(() => SetupLoopButtonMethod());
        BTN_loopForPlaylist.onClick.AddListener(() => SetupLoopButtonMethod());
        BTN_pauseAndPlayMini.onClick.AddListener(() => SetupPauseAndPlayImage());
        BTN_pauseAndPlayFull.onClick.AddListener(() => SetupPauseAndPlayImage());
        BTN_pauseAndPlayPlaylist.onClick.AddListener(() => SetupPauseAndPlayImage());
        BTN_next.onClick.AddListener(() => SCR_MM.ToggleNextSong());
        BTN_nextForPlaylist.onClick.AddListener(() => SCR_MM.ToggleNextSong());
        BTN_prev.onClick.AddListener(() => SCR_MM.TogglePrevSong());
        BTN_prevForPlaylist.onClick.AddListener(() => SCR_MM.TogglePrevSong());
        // Robert: ada ini biar kalau null reference, dia ga munculin error. Kayaknya ini ga kepake juga akhirnya?
        if (BTN_playlist) BTN_playlist.onClick.AddListener(() => SetupPlaylistUI());

        // set ui icon diawal
        UpdateShuffleUI();
        UpdateLoopUI();

        // Event listener pas laguan berubah
        //SCR_MM.ACT_playSong += UpdateSongNameUI;
    }

    private void Update()
    {
        if (SCR_MM != null)
        {
            SCR_MM.ProgressBarLogic(SLR_progressBar);
        }
    }

    // setup buat button shuffle
    public void SetupShuffleButton()
    {
        SCR_MM.ToggleShuffle();

        UpdateShuffleUI();

        Debug.Log("Shuffle toggled!");
    }

    // buat update ui dari shuffle button
    public void UpdateShuffleUI()
    {
        if (SCR_MM.B_isShuffling)
        {
            IMG_shuffleButton.sprite = SPR_shuffleOn;
            IMG_shuffleButtonForPlaylist.sprite = SPR_shuffleOn;
        }
        else
        {
            IMG_shuffleButton.sprite = SPR_shuffleOff;
            IMG_shuffleButtonForPlaylist.sprite = SPR_shuffleOff;
        }
    }

    // setup buat button loop
    public void SetupLoopButtonMethod()
    {
        SCR_MM.ToggleLoopMethod();

        UpdateLoopUI();

        Debug.Log("Loop method: " + SCR_MM.ENM_loopMethod);
    }

    // buat update ui loop button
    public void UpdateLoopUI()
    {
        switch (SCR_MM.ENM_loopMethod)
        {
            case ENM_LoopMethod.NoLoop:
                IMG_loopButton.sprite = SPR_NoLoop;
                IMG_loopButtonForPlaylist.sprite = SPR_NoLoop;
                break;

            case ENM_LoopMethod.LoopSong:
                IMG_loopButton.sprite = SPR_loopSong;
                IMG_loopButtonForPlaylist.sprite = SPR_loopSong;
                break;

            case ENM_LoopMethod.LoopPlaylist:
                IMG_loopButton.sprite = SPR_loopPlaylist;
                IMG_loopButtonForPlaylist.sprite = SPR_loopPlaylist;
                break;
        }
    }

    // setup buat button pause sama play (ini assetnya blon ada jadi if if-an nya nggk dipake duls)
    public void SetupPauseAndPlayImage()
    {
        SCR_MM.TogglePauseAndPlayMusic();

        SetupPauseAndPlayImage(SPR_currentPlaySprite, SPR_currentPauseSprite);
    }

    public void SetupPauseAndPlayImage(Sprite SPR_playIcon, Sprite SPR_pauseIcon)
    {
        SPR_currentPlaySprite = SPR_playIcon;
        SPR_currentPauseSprite = SPR_pauseIcon;

        if (SCR_MM.ENM_pauseAndPlay == ENM_PauseAndPlay.Paused)
        {
            IMG_pauseAndPlayIconMini.sprite = SPR_playIcon;
            IMG_pauseAndPlayIconFull.sprite = SPR_playIcon;
            IMG_pauseAndPlayIconPlaylist.sprite = SPR_playIcon;
        }
        else
        {
            IMG_pauseAndPlayIconMini.sprite = SPR_pauseIcon;
            IMG_pauseAndPlayIconFull.sprite = SPR_pauseIcon;
            IMG_pauseAndPlayIconPlaylist.sprite = SPR_pauseIcon;
        }
    }

    // setup pan buat ui nama author sama laguannyaa
    public void SetupNameUI(Songs currentSong)
    {
        TMPGUI_titleAndAuthor.text = currentSong.S_titleAndAuthor;
    }

    // nah ini fungsi buat ganti nama songnya kalo pindah lagu
    public void UpdateSongNameUI(Songs newSong)
    {
        TMPGUI_titleAndAuthor.text = newSong.S_titleAndAuthor;
    }

    public void SetupPlaylistUI()
    {
        GO_playlist.SetActive(true);
    }
}