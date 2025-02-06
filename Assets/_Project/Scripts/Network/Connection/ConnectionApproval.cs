using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Network.Game;
using _Project.Scripts.Steam;
using _Project.Scripts.Utility;
using Newtonsoft.Json;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network.Connection
{
    public static class ConnectionApproval
    {
        #if Server
        private static bool _initialized;
        
        public static void OnConnectionRequest(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            Init();

            var payload = GetPayload(request);

            // Check if the payload is valid
            if (!payload.IsValid)
            {
                response.Approved = false;
                response.Reason = "Failed to authenticate on the Server. The SteamId or Username is invalid.";
                return;   
            }
            
            // Todo: Check for potential penaties or bans
            
            // Todo: Check if he is in the user list of multiplay server hosting | ! User the SteamID to check !

            // Check if this account is already on this Server
            if (Client.DoesClientExist(payload.SteamId))
                // Old -> This is a player that is already registered on this Server
            {
                Client client = Client.FindClient(payload.SteamId);
                if (client.IsConnected)
                {
                    // This player is already connected and online. We kick him because he is already on the server. Bug | Cheater
                    response.Approved = false;
                    response.Reason = "It seams like your already on the Server. Please check if someone plays with your account or report this to a developer.";
                    return;
                }
                else
                {
                    // Reconnect the player
                    
                    // Change the player status to connected
                    client.UpdateConnection(true);
                    
                    // Todo: Handle ownership
                    
                    response.Approved = true;
                }
                
            }
            else // New -> This is a player that isn't already registered on this Server
            {
                /* !We don't check if this player is allowed to join the Server,
                 because the Server will only start the game if everyone is connected
                // Check if we are waiting for players
                if (GameStateManager.CurrentState != GameState.WaitingForPlayers)
                {
                    response.Approved = false;
                    response.Reason = "Server isn't waiting for players";
                    return;
                }
                */
                Client newClient = new Client(
                    payload.Username, 
                    payload.SteamId, 
                    request.ClientNetworkId
                    );
                
                response.Approved = true;
            }
            
            // This at the bottom will only run if the player is approved
            
            // Todo: Mark the player for this server so that he can reconnect
            // Todo: Use his inventory, stats, etc.
        }

        private static (SteamId SteamId, string Username, bool IsValid) GetPayload(NetworkManager.ConnectionApprovalRequest request)
        {
            try
            {
                // Decoding the payload
                string payloadJson = System.Text.Encoding.UTF8.GetString(request.Payload);
                Payload payload = JsonUtility.FromJson<Payload>(payloadJson);

                SteamId steamId = (SteamId) payload.SteamId;
                
                // Validating the payload
                if (steamId.IsValid && !string.IsNullOrWhiteSpace(payload.Username))
                    return (payload.SteamId, payload.Username, true);
                else
                    return ((SteamId) 404, "Invalid", false);
            }
            catch (JsonException e) // Catching JSON-related exceptions
            {
                // Log error if necessary
                Debug.Log("Invalid JSON Payload");
                return ((SteamId) 404, "Invalid", false);
            }
            catch (Exception e)
            {
                // Log error for any other exceptions
                Debug.LogWarning("Something went wrong");
                return ((SteamId) 404, "Invalid", false);
            }
        }

        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            ConnectionCallbacks.OnClientConnected += (client) => { CheckForGameStart(); };
        }

        public static void CheckForGameStart()
        {
            int playerCount = Client.GetClients().Length;
            int requiredPlayers = GameModeManager.CurrentGameMode._wantedPlayers;
            Debug.Log("Checking for game start. Players: " + playerCount + " / " + requiredPlayers);
            if (playerCount < requiredPlayers)
                // Still waiting for players
            {
                // Todo: Run a single timer and if the timer runs out, close the server and kick all players
                return;
            }
            else if (playerCount > requiredPlayers)
                // Something went wrong. Close the Server
            {
                // There are too many players on the server (Somehow).
                // Todo: Close server and kick all players
                return;
            }
            else
            {
                // Start the game
                GameModeManager.CurrentGameMode.StartGame();
            }
        }
        
        #endif
    }
}