#region Chat
public enum ENM_CharFace
{
    Test1 = 1
}

public enum ENM_DialogTitle
{
    None = 0,
    Test1 = 1,
    Test2 = 2,
    Sample = 3,
    SamplePanjang = 4,
}
#endregion

//Ini nanti ganti ke punyanya Raden aja
//public enum ENM_Room
//{
//    Church = 1,
//    Work = 2,
//    Relax = 3
//}

#region Music
public enum ENM_PlaylistType
{
    Rock = 1,
    Jazz = 2
}

public enum ENM_MusicCode
{
    Song1 = 101,
    Song2 = 102,
    Song3 = 103,
}

public enum ENM_PauseAndPlay
{
    Paused,
    Play,
}

public enum ENM_LoopMethod
{
    NoLoop,
    LoopSong,
    LoopPlaylist
}
#endregion

#region HUD
public enum ENM_Room
{
    Church = 0,
    Office = 1,
    Bedroom = 2,
}
#endregion