using Steamworks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Network.Connection
{
    public class ConnectionPayload
    {
        #if Client
        
        public static byte[] GetPayload()
        {
            Payload payload = new Payload();
            payload.Username = SteamClient.Name;
            #if DEBUG
            payload.SteamId = (SteamId) (ulong) Random.Range(0, 1000000);
            #else 
            payload.SteamId = SteamClient.SteamId;
            #endif
            
            string payloadJson = JsonUtility.ToJson(payload);
            
            return System.Text.Encoding.UTF8.GetBytes(payloadJson);
        }
        
        #endif
    }
}