using System.Collections.Generic;
using Unity.Netcode;

namespace _Project.Scripts.Teams
{
    public class Team : INetworkSerializable
    {
        public string TeamName => TeamType.ToString();
        public TeamType TeamType;
        public List<ulong> Players;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // Serialize the TeamType as a byte
            byte teamTypeByte = (byte) TeamType;
            if (serializer.IsWriter)
            {
                serializer.SerializeValue(ref teamTypeByte);
            }
            else
            {
                serializer.SerializeValue(ref teamTypeByte);
                TeamType = (TeamType) teamTypeByte;
            }

            // --- Manual List<ulong> serialization ---
            if (serializer.IsWriter)
            {
                // write the number of players
                int count = Players.Count;
                serializer.SerializeValue(ref count);

                // write each player ID
                for (int i = 0; i < count; i++)
                {
                    ulong id = Players[i];
                    serializer.SerializeValue(ref id);
                }
            }
            else
            {
                // read count
                int count = 0;
                serializer.SerializeValue(ref count);

                // read each player ID into the list
                Players = new List<ulong>(count);
                for (int i = 0; i < count; i++)
                {
                    ulong id = 0;
                    serializer.SerializeValue(ref id);
                    Players.Add(id);
                }
            }
        }
    }
}