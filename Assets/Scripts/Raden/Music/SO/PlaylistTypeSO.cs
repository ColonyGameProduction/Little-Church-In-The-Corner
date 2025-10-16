using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaylistTypeSO", menuName = "Music/PlaylistTypeSO")]
public class PlaylistTypeSO : ScriptableObject
{
    public ENM_PlaylistType ENM_playlistType;
    public List<Songs> SCR_playlist;

    // nyari song berdasarkan enum di satu playlist
    public Songs SCR_GetSongs(ENM_MusicCode ENM_musicCode)
    {
        return SCR_playlist.Find(songs => songs.ENM_musicCode == ENM_musicCode);
    }
}