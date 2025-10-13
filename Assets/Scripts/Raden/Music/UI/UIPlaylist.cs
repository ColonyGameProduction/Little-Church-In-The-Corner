using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIPlaylist : MonoBehaviour
{
    [Header("References")]
    public MusicManager SCR_MM;

    [Header("Prefabs")]
    public GameObject PB_playlistButton;
    public GameObject PB_songsButton;

    [Header("Parents / Holders")]
    public Transform TF_playlistButtonParent;
    public Transform TF_songsButtonParent;

    private List<GameObject> playlistButtons = new List<GameObject>();
    private List<GameObject> songButtons = new List<GameObject>();

    private void Start()
    {
        SetupAllPlaylistButton();
    }

    public void SetupAllPlaylistButton()
    {
        // Bersihkan tombol lama
        foreach (Transform child in TF_playlistButtonParent)
        {
            Destroy(child.gameObject);
        }
        playlistButtons.Clear();

        // Loop semua playlist di MusicManager
        foreach (PlaylistTypeSO playlist in SCR_MM.SO_listOfPlaylistSO.SO_playlistTypeSO)
        {
            // setup buttonnya gess
            GameObject newButton = Instantiate(PB_playlistButton, TF_playlistButtonParent);
            TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
            text.text = playlist.ENM_playlistType.ToString();

            Button btn = newButton.GetComponent<Button>();
            ENM_PlaylistType playlistType = playlist.ENM_playlistType;

            // Cek script Songs
            btn.onClick.AddListener(() =>
            {
                //setup semua playlist song
            });

            playlistButtons.Add(newButton);
        }
    }
}

