using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    private List<GameObject> GO_playlistButtons = new List<GameObject>();
    private List<GameObject> GO_songButtons = new List<GameObject>();

    private void Start()
    {
        SetupAllPlaylistButton();
        SetupAllPlaylistSongsFirstTime();
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
            });

            GO_playlistButtons.Add(GO_newButton);
        }
    }

    // nentuin mana playlist yang jalan duluan
    public void SetupAllPlaylistSongsFirstTime()
    {
        if (SCR_MM.SO_currPlaylistTypeSO == null)
        {
            SCR_MM.SO_currPlaylistTypeSO = SCR_MM.SO_listOfPlaylistSO.SO_playlistTypeSO[0];
        }

        SetupAllPlaylistSongs(SCR_MM.SO_currPlaylistTypeSO.ENM_playlistType);
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

        PlaylistTypeSO playlistTypeSO = SCR_MM.SO_listOfPlaylistSO.SO_GetPlaylistTypeSO(playlistType);
        SCR_MM.SO_currPlaylistTypeSO = playlistTypeSO;

        foreach (Songs songs in playlistTypeSO.SCR_playlist)
        {
            GameObject GO_newButton = Instantiate(PB_songsButton, TF_songsButtonParent);
            TextMeshProUGUI TMPUGUI_text = GO_newButton.GetComponentInChildren<TextMeshProUGUI>();
            TMPUGUI_text.text = songs.S_titleAndAuthor;

            Button BTN_btn = GO_newButton.GetComponent<Button>();
            Songs _selectedSong = songs;

            /*Songs songComp = GO_newButton.GetComponent<Songs>();
            if (songComp == null)
            {
                songComp = GO_newButton.AddComponent<Songs>();
            }

            songComp.ADO_music = songs.ADO_music;
            songComp.S_titleAndAuthor = songs.S_titleAndAuthor;
            songComp.ENM_musicCode = songs.ENM_musicCode;*/

            BTN_btn.onClick.AddListener(() =>
            {
                SCR_MM.PlaySong(_selectedSong);
            });
        }
    }
}
