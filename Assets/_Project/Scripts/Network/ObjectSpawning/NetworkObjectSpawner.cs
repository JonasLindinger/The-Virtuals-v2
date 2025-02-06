using System;
using _Project.Scripts.Network.Connection;
using _Project.Scripts.Utility;
using UnityEngine;

namespace _Project.Scripts.Network.ObjectSpawning
{
    public class NetworkObjectSpawner : PersistentSingleton<NetworkObjectSpawner>
    {
        [Header("Prefabs")]
        [SerializeField] private NetworkClient _networkClientPrefab;
        [SerializeField] private NetworkPlayer _networkPlayerPrefab;
        
        #if Server
        private void Start()
        {
            ConnectionCallbacks.OnClientConnected += SpawnNetworkClient;
        }
        
        public override void LateDestroy()
        {
            ConnectionCallbacks.OnClientConnected -= SpawnNetworkClient;
        }

        private void SpawnNetworkClient(Client clientInfo)
        {
            // Spawn local
            var client = Instantiate(_networkClientPrefab);
            
            // Useless synchronization disable
            client.NetworkObject.SynchronizeTransform = false;
            client.NetworkObject.SceneMigrationSynchronization = false;
            client.NetworkObject.DestroyWithScene = false;
            
            // Keep the client object, so that it is possible to reconnect for this client
            client.NetworkObject.DontDestroyWithOwner = true;
            
            // Spawn with ownership. The object will be hidden from any other client
            client.NetworkObject.SpawnWithObservers = false;
            client.NetworkObject.SpawnWithOwnership(clientInfo.ClientId);
            client.NetworkObject.NetworkShow(clientInfo.ClientId);
        }

        public void SpawnNetworkPlayers(Client[] clients)
        {
            foreach (var player in clients)
                SpawnNetworkPlayer(player);
        }
        
        private void SpawnNetworkPlayer(Client client)
        {
            // Spawn local
            var player = Instantiate(_networkPlayerPrefab);
            
            // Useless synchronization disable
            player.NetworkObject.SynchronizeTransform = false;
            player.NetworkObject.SceneMigrationSynchronization = false;
            player.NetworkObject.DestroyWithScene = false;
            
            // Keep the client object, so that it is possible to reconnect for this client
            player.NetworkObject.DontDestroyWithOwner = true;
            
            // Spawn with ownership. The object will be shown to all clients
            player.NetworkObject.SpawnWithObservers = true;
            player.NetworkObject.SpawnWithOwnership(client.ClientId);
        }
        #endif
    }
}