namespace _Project.Scripts.Network.Game
{
    #if Server
    public enum GameState
    {
        Starting,
        WaitingForPlayers,
        Loading,
        Playing,
        Ending
    }
    #endif
}