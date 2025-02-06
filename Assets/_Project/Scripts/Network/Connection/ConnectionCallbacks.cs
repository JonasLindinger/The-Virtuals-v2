using System;
using System.Linq;
using _Project.Scripts.Network.ObjectSpawning;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network.Connection
{
    public static class ConnectionCallbacks
    {
        #if Server
        public static event Action<Client> OnClientConnected = delegate { };
        public static event Action<Client> OnClientDisconnected = delegate { };
        #endif
        
        public static void OnConnectionEvent(ConnectionEventData eventData)
        {
            bool isRegistered = Client.DoesClientExistByClientId(eventData.ClientId);
            Client client = Client.FindClientByClientId(eventData.ClientId);
            
            switch (eventData.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    #if Client
                    Debug.Log("Connected to Server");
                    
                    #elif Server
                    if (!isRegistered)
                    {
                        Debug.LogWarning("Something went wrong");
                        return;
                    }
                    Debug.Log("Client Connected");
                    OnClientConnected?.Invoke(client);
                    #endif
                    break;
                case ConnectionEvent.ClientDisconnected:
                    #if Client
                    Debug.Log(NetworkManager.Singleton.DisconnectReason);
                    #elif Server
                    if (!isRegistered)
                    {
                        Debug.LogWarning("Refused connection");
                        return;
                    }
                    
                    Debug.Log("Client Disconnected");
                    OnClientDisconnected?.Invoke(client);
                    client.UpdateConnection(false);
                        
                    // Todo: Add penality and if there are less than 2 players, end the game
                    // Todo: If the game hasn't started yet, close the server and kick all players (he doged the game)
                    #endif
                    break;
            }
        }
    }
}