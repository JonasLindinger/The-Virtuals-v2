using System.Collections.Generic;
using System.Linq;
using Steamworks;

namespace _Project.Scripts.Network.Connection
{
    public struct Client
    {
        private static List<Client> Clients =  new List<Client>();
        public static Client[] GetClients() => Clients.ToArray();
        
        public string Username { get; }
        public SteamId SteamId { get; private set; }
        public ulong ClientId { get; private set; } // The client network id
        public bool IsConnected { get; private set; }

        public Client(string username, SteamId steamId, ulong clientId, bool addToList = true)
        {
            Username = username;
            SteamId = steamId;
            ClientId = clientId;
            IsConnected = true;
            
            if (addToList)
                Add();
        }

        public static bool DoesClientExist(SteamId steamId)
        {
            return Clients.Any(client => client.SteamId == steamId);
        }
        
        public static bool DoesClientExistByClientId(ulong clientId)
        {
            return Clients.Any(client => client.ClientId == clientId);
        }
        
        public static Client FindClient(SteamId steamId)
        {
            return Clients.FirstOrDefault(client => client.SteamId == steamId);
        }
        
        public static Client FindClientByClientId(ulong clientId)
        {
            return Clients.FirstOrDefault(client => client.ClientId == clientId);
        }

        public void Add()
        {
            Clients.Add(this);
        }
        
        public void UpdateConnection(bool isConnected)
        {
            IsConnected = isConnected;
            UpdateThisClientInList();
        }
        
        private void UpdateThisClientInList()
        {
            // Update the client info in the list
            SteamId identificationId = SteamId;
            int idx =Clients.FindIndex(client => client.SteamId == identificationId);
            Clients[idx] = this;
        }
    }
}