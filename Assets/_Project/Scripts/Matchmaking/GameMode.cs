namespace _Project.Scripts.Matchmaking
{
    public enum GameMode : short
    {
        #if Client
        Unranked = 1,
        Ranked = 2,
        Custom = 3,
        #endif
    }
}