using System;
using _Project.Scripts.Utility;
using CSP;
using CSP.Data;
using Steamworks;
using UnityEngine;

namespace _Project.Scripts.Matchmaking
{
    public class MatchmakingManager : PersistentSingleton<MatchmakingManager>
    {
        #if Client
        public async void StartMatchmaking(GameModeInfo gameModeInfo)
        {
            Debug.Log("Play: " + gameModeInfo._displayName);

            switch (gameModeInfo.gameMode)
            {
                case GameMode.Unranked:
                    throw new NotImplementedException();
                    break;
                case GameMode.Ranked:
                    throw new NotImplementedException();
                    break;
                case GameMode.Custom:
                    throw new NotImplementedException();
                    break;
                case GameMode.Local:
                    ConnectionPayload payload = new ConnectionPayload();
                    payload.DisplayName = SteamClient.Name;
                    payload.ClientId = SteamClient.SteamId;
                    
                    NetworkRunner.GetInstance().Run(Settings.IP, Settings.Port, payload);
                    break;
            }
        }
        #endif
    }
}