using _Project.Scripts.Network.Connection;
using _Project.Scripts.Network.ObjectSpawning;
using UnityEngine;

namespace _Project.Scripts.Network.Game
{
    public class Deathmatch : GameMode
    {
        #if Server
        public override void Init()
        {
            if (_started) return;

            _name = "Deathmatch";
            _wantedPlayers = 2;
            
            GameStateManager.SetState(GameState.WaitingForPlayers);
        }

        public override void StartGame()
        {
            if (_started) return;
            _started = true;
            
            Debug.Log("Deathmatch started");
            GameStateManager.SetState(GameState.Playing);
            NetworkObjectSpawner.Instance.SpawnNetworkPlayers(Client.GetClients());
        }

        public override void PlayerConnected(Client client)
        {
            
        }
        
        public override void PlayerDisconnected(Client client)
        {
            
        }
        #endif
    }
}