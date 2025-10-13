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
    public Button BTN_loop;
    public Button BTN_pauseAndPlay;
    public Button BTN_next;
    public Button BTN_prev;
    public Button BTN_playlist;

    [Header("Semua UI Element")]
    //public Image IMG_playMethod;
    //public Image IMG_pauseAndPlay;
    public TextMeshProUGUI TMPGUI_titleAndAuthor;
    public Slider SLR_progressBar;

    //[Header("Sprites (for changing icons)")]
    //public Sprite SPR_playIcon;
    //public Sprite SPR_pauseIcon;

    private void Start()
    {
        // ini setupan buat button listener yak
        BTN_shuffle.onClick.AddListener(() => SetupShuffleButton());
        BTN_loop.onClick.AddListener(() => SetupLoopButtonMethod());
        BTN_pauseAndPlay.onClick.AddListener(() => SetupPauseAndPlayImage());
        BTN_next.onClick.AddListener(() => SCR_MM.ToggleNextSong());
        BTN_prev.onClick.AddListener(() => SCR_MM.TogglePrevSong());

        // Event listener pas laguan berubah
        SCR_MM.ACT_playSong += UpdateSongNameUI;
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
        //jangan lupa ntar dikasih logic buat UI nya
        //yang gonta ganti image
        SCR_MM.ToggleShuffle();
        Debug.Log("Shuffle toggled!");
    }

    // setup buat button loop
    public void SetupLoopButtonMethod()
    {
        //jangan lupa ntar dikasih logic buat UI nya
        //yang gonta ganti image
        SCR_MM.ToggleLoopMethod();
        Debug.Log("Loop method: " + SCR_MM.ENM_loopMethod);
    }

    // setup buat button pause sama play (ini assetnya blon ada jadi if if-an nya nggk dipake duls)
    public void SetupPauseAndPlayImage()
    {
        SCR_MM.TogglePauseAndPlayMusic();

        if (SCR_MM.ENM_pauseAndPlay == ENM_PauseAndPlay.Paused)
        {
            //IMG_pauseAndPlay.sprite = SPR_playIcon;
        }
        else
        {
            //IMG_pauseAndPlay.sprite = SPR_pauseIcon;
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
        // nantian yak ini aga ribet ini gay
    }
}