using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListOfPlaylistSO", menuName = "Music/ListOfPlaylistSO")]
public class ListOfPlaylistSO : ScriptableObject
{
    public List<PlaylistTypeSO> SO_playlistTypeSO = new List<PlaylistTypeSO>();

    // nyari playlist berdasarkan enum di satu kumpulan list playlist
    public PlaylistTypeSO SO_GetPlaylistTypeSO(ENM_PlaylistType ENM_playlistType)
    {
        return SO_playlistTypeSO.Find(playlistTypeSO => playlistTypeSO.ENM_playlistType == ENM_playlistType);
    }
}