using System.Collections.Generic;
using _Project.Scripts.Matchmaking;
using _Project.Scripts.Player;
using _Project.Scripts.Teams;
using SceneManagement;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network
{
    public class GameStateListener : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int playerCountToStart = 2;

        private Team[] _teams;
        private GameMode _gamemode;
        
        #if Server
        private static List<GameStateListener> _instances = new List<GameStateListener>();
        
        private static int _currentPlayerCount;
        
        public override void OnNetworkSpawn()
        {
            _instances.Add(this);
            
            _currentPlayerCount++;

            if (_currentPlayerCount == playerCountToStart)
            {
                // Todo: Add new gamemodes and change this logic accordingly
                _gamemode = GameMode.Local;
                _teams = TeamManager.Get1V1Teams();
                InitializeGame();
                
                if (_teams == null)
                {
                    Debug.LogError("Somethign went wrong while trying to get teams for 1v1 gamemode.");
                    return;
                }
                
                foreach (var instance in _instances)
                    instance.OnGoInGameRPC(_teams, (byte) _gamemode);
            }
        }

        public override void OnNetworkDespawn()
        {
            _currentPlayerCount--;
            
            // Todo: Handle if game already started
        }
        #endif

        [Rpc(SendTo.Owner, Delivery = RpcDelivery.Reliable)]
        private void OnGoInGameRPC(Team[] teams, byte gamemode)
        {
            _gamemode = (GameMode) gamemode;
            _teams = teams;
            
            InitializeGame();
        }

        private async void InitializeGame()
        {
            // Todo: Do something with the teams (Scoreboard, etc.)
            
            await SceneLoader.GetInstance().LoadSceneGroup(2);
            
            #if Server
            SpawnpointManager.GetInstance().SpawnPlayers(_teams, _gamemode);
            #endif
        }
    }
}