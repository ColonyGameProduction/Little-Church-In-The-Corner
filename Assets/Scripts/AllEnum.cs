#region Chat
public enum ENM_CharFace
{
    // Muka cowok
    Cowo = 1,
    // Muka cewek
    Cewe = 2,
}

public enum ENM_DialogTitle
{
    None = 0,
    Test1 = 1,
    Test2 = 2,
    Sample = 3,
    SamplePanjang = 4,
    QnATest1 = 5,
    QnATest2 = 6,
    Renungan1 = 7,
    Konsultasi1 = 8,
}

public enum ENM_ChatBubbleBackground
{
    Kiri = 0,
    Kanan = 1,
}
#endregion

#region Music
public enum ENM_PlaylistType
{
    Worship = 1,
    Praise = 2,
    Local = 3,
}

public enum ENM_MusicCode
{
    Worship1 = 101,
    Worship2 = 102,
    Worship3 = 103,

    Praise1 = 201,
    Praise2 = 202,
    Praise3 = 203,

    SongLocal1 = 301,
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

public enum ENM_PanelState
{
    Mini,
    Full,
    Playlist
}
#endregion

#region HUD
public enum ENM_Room
{
    // Aku pindahin office jadi yang pertama gara-gara di tombol UInya, office yang pertama
    Office = 0,
    Church = 1,
    Bedroom = 2,
}
#endregion

#region Skin
public enum ENM_SkinItem
{
    Floor1,
    Floor2,
    Floor3,
    Floor4,
    Floor5,
    Floor6,
    Wall1,
    Wall2,
    Wall3,
    Wall4,
    Wall5,
    Wall6,
    ObjectTest1,
    ObjectTest2,
    ObjectTest3,
    ObjectTest4,
    ObjectTest5,
}

public enum ENM_SkinType
{
    Floor,
    Wall,
    ObjectTest,
    Activity,
}
#endregion