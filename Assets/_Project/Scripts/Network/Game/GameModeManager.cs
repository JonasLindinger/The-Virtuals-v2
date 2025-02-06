using _Project.Scripts.Network.Connection;
using _Project.Scripts.Utility;
using UnityEngine;

namespace _Project.Scripts.Network.Game
{
    public class GameModeManager : PersistentSingleton<GameModeManager>
    {
        public static GameMode CurrentGameMode => Instance._gameMode;
        [SerializeField] private GameMode _gameMode;
        
        #if Server
        private void Start()
        {
            CustomNetworkManager.OnServerStarted += StartGameMode;
        }

        public override void LateDestroy()
        {
            CustomNetworkManager.OnServerStarted -= StartGameMode;
        }

        private void StartGameMode()
        {
            _gameMode.Init();
            ConnectionCallbacks.OnClientConnected += _gameMode.PlayerConnected;
            ConnectionCallbacks.OnClientDisconnected += _gameMode.PlayerDisconnected;
        }
        #endif
    }
}