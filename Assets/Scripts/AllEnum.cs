public class AllEnum
{
    #region Chat
    public enum ENM_CharFace
    {
        Test1 = 1
    }

    public enum ENM_DialogTitle
    {
        Test1 = 1,
        Test2 = 2,
    }
    #endregion

    #region Music
    public enum ENM_PlaylistType
    {
        Rock = 1,
        Jazz = 2
    }

    public enum ENM_MusicCode
    {
        Song1 = 101,
        Song2 = 102
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
}