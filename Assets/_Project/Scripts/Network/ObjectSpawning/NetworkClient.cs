using System.Collections.Generic;
using Unity.Netcode;

namespace _Project.Scripts.Network.ObjectSpawning
{
    public class NetworkClient : NetworkBehaviour
    {
        public static List<NetworkClient> Clients { get; private set; } = new List<NetworkClient>();

        public override void OnNetworkSpawn()
        {
            Clients.Add(this);
        }
    }
}