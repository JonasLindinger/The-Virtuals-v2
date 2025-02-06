using _Project.Scripts.Network;
using _Project.Scripts.SceneManagement;
using _Project.Scripts.Utility;
using UnityEngine;

namespace _Project.Scripts.Matchmaking
{
    public class MatchmakingManager : PersistentSingleton<MatchmakingManager>
    {
        #if Client
        public async void StartMatchmaking(GameModeInfo gameModeInfo)
        {
            Debug.Log("Play: " + gameModeInfo._displayName);

            switch (gameModeInfo._gameMode)
            {
                case GameMode.Unranked:
                    break;
                case GameMode.Ranked:
                    break;
                case GameMode.Custom:
                    CustomNetworkManager.Instance.ConnectToServer(Settings.IP, Settings.Port);
                    break;
            }
        }
        #endif
    }
}