using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem.Composites;

public class UIPlaylist : MonoBehaviour
{
    [Header("References")]
    public MusicManager SCR_MM;

    [Header("Para Prefab")]
    public GameObject PB_playlistButton;
    public GameObject PB_songsButton;

    [Header("Para Parent")]
    public Transform TF_playlistButtonParent;
    public Transform TF_songsButtonParent;

    [Header("Playlist Button Sprites")]
    public Sprite SPR_playlistNormal;

    [Header("playlist theme")] // --> ini chat variablenya hehe
    public Sprite SPR_playlistPagi;
    public Sprite SPR_playlistSore;
    public Sprite SPR_playlistMalam;
    public Image IMG_playlistTheme;

    private GameObject GO_currentSelectedPlaylistButton = null;

    private List<GameObject> GO_playlistButtons = new List<GameObject>();
    private List<GameObject> GO_songButtons = new List<GameObject>();

    private Dictionary<Songs, Image> DICT_songButtonsHighlight = new Dictionary<Songs, Image>();

    public Image IMG_selectedButton;

    private void Start()
    {
        SetupAllPlaylistButton();
        SetupAllPlaylistSongsFirstTime();

        MusicManager.Instance.ACT_playSong += HighlightSongsButton;
    }

    // buat nge setup playlist tab button
    public void SetupAllPlaylistButton()
    {
        // bersiin tombol2 lama
        foreach (Transform TF_child in TF_playlistButtonParent)
        {
            Destroy(TF_child.gameObject);
        }
        GO_playlistButtons.Clear();

        // loop semua playlist yang ada di MusicManager
        foreach (PlaylistTypeSO playlistTypeSO in SCR_MM.SO_listOfPlaylistSO.SO_playlistTypeSO)
        {
            // setup button buat playlist tab gess
            GameObject GO_newButton = Instantiate(PB_playlistButton, TF_playlistButtonParent);
            TextMeshProUGUI TMPUGUI_text = GO_newButton.GetComponentInChildren<TextMeshProUGUI>();
            TMPUGUI_text.text = playlistTypeSO.ENM_playlistType.ToString();

            Button BTN_btn = GO_newButton.GetComponent<Button>();
            ENM_PlaylistType playlistType = playlistTypeSO.ENM_playlistType;

            // cek script Songs
            BTN_btn.onClick.AddListener(() =>
            {
                SetupAllPlaylistSongs(playlistType);
                HighlightPlaylistButton(GO_newButton);
            });

            GO_playlistButtons.Add(GO_newButton);
        }
    }

    private void HighlightPlaylistButton(GameObject selectedButton)
    {
        // deklarasi warna normal sama warna selected
        Color normalColor;
        ColorUtility.TryParseHtmlString("#14465D", out normalColor);
        Color selectedColor = Color.white;

        // reset semua button ke sprite normalan
        foreach (GameObject btn in GO_playlistButtons)
        {
            btn.GetComponent<Image>().sprite = SPR_playlistNormal;

            TextMeshProUGUI txt = btn.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null)
            {
                txt.color = normalColor;
            }
        }

        Image img = selectedButton.GetComponent<Image>();

        // ganti yang button yg dipilih ke selected sprite
        if (img != null)
        {
            img.sprite = DayNightCycleManager.Instance.SCR_dayNightCycleUI.SPR_currentSelectedPlaylistButton;
            IMG_selectedButton = img;
        }

        TextMeshProUGUI selectedText = selectedButton.GetComponentInChildren<TextMeshProUGUI>();

        // ganti warna text di button jadi selected color
        if (selectedText != null)
        {
            selectedText.color = selectedColor;
        }

        // simpen referensi
        GO_currentSelectedPlaylistButton = selectedButton;
    }

    /// <summary>
    /// Buat highlight lagu mana yang lagi diputar di playlist
    /// </summary>
    /// <param name="songs">Lagu saat ini</param>
    private void HighlightSongsButton(Songs songs)
    {
        foreach (KeyValuePair<Songs, Image> KVP_highlight in DICT_songButtonsHighlight)
        {
            KVP_highlight.Value.color = Color.clear;
        }

        if (DICT_songButtonsHighlight.TryGetValue(songs, out Image IMG_highlight))
        {
            IMG_highlight.color = Color.white;
        }
        else Debug.LogWarning("WARNING: ga ketemu tombol dengan lagu " + songs.ENM_musicCode);
    }

    // nentuin mana playlist yang jalan duluan
    public void SetupAllPlaylistSongsFirstTime()
    {
        if (SCR_MM.SO_currPlaylistTypeSO == null)
        {
            SCR_MM.SO_currPlaylistTypeSO = SCR_MM.SO_listOfPlaylistSO.SO_playlistTypeSO[0];
        }

        SetupAllPlaylistSongs(SCR_MM.SO_currPlaylistTypeSO.ENM_playlistType);

        // highlight playlist pertama
        if (GO_playlistButtons.Count > 0)
        {
            HighlightPlaylistButton(GO_playlistButtons[0]);
        }
    }

    // buat nge setup song musik button
    public void SetupAllPlaylistSongs(ENM_PlaylistType playlistType)
    {
        // bersiin tombol2 lama
        foreach (Transform TF_child in TF_songsButtonParent)
        {
            Destroy(TF_child.gameObject);
        }
        GO_songButtons.Clear();
        DICT_songButtonsHighlight.Clear();

        PlaylistTypeSO playlistTypeSO = SCR_MM.SO_listOfPlaylistSO.SO_GetPlaylistTypeSO(playlistType);
        SCR_MM.SO_currPlaylistTypeSO = playlistTypeSO;

        foreach (Songs songs in playlistTypeSO.SCR_playlist)
        {
            GameObject GO_newButton = Instantiate(PB_songsButton, TF_songsButtonParent);
            TextMeshProUGUI TMPUGUI_text = GO_newButton.GetComponentInChildren<TextMeshProUGUI>();
            TMPUGUI_text.text = songs.S_titleAndAuthor;

            Songs newSongs = new Songs();

            //GO_newButton.AddComponent<>();
            Button BTN_btn = GO_newButton.GetComponent<Button>();
            Songs _selectedSong = songs;

            DICT_songButtonsHighlight.TryAdd(songs, GO_newButton.GetComponent<Image>());

            //Songs songComp = GO_newButton.GetComponent<Songs>();
            //if (songComp == null)
            //{
            //    songComp = GO_newButton.AddComponent<Songs>();
            //}

            //songComp.ADO_music = songs.ADO_music;
            //songComp.S_titleAndAuthor = songs.S_titleAndAuthor;
            //songComp.ENM_musicCode = songs.ENM_musicCode;

            BTN_btn.onClick.AddListener(() =>
            {
                SCR_MM.PlaySong(_selectedSong);
            });
        }
    }

    // ini buat di taro di add button
    public void OnAddLocalSongButton()
    {
        FindAnyObjectByType<UniversalLocalSongImporter>().ImportLocalSong();
    }
}
