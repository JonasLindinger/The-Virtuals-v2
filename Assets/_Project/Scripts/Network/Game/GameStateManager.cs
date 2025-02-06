namespace _Project.Scripts.Network.Game
{
    public class GameStateManager
    {
        #if Server

        public static GameState CurrentState { get; private set; } = GameState.Starting;
        
        public static void SetState(GameState state) => CurrentState = state;
        
        #endif
    }
}