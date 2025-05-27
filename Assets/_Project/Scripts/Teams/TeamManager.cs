using System.Collections.Generic;
using Unity.Netcode;

namespace _Project.Scripts.Teams
{
    public static class TeamManager
    {
        public static Team[] Get1V1Teams()
        {
            if (NetworkManager.Singleton.ConnectedClients.Count != 2) return null;
            
            Team blueTeam = new Team
            {
                TeamType = TeamType.Blue,
                Players = new List<ulong>
                {
                    NetworkManager.Singleton.ConnectedClientsIds[0],
                }
            };

            Team redTeam = new Team
            {
                TeamType = TeamType.Red,
                Players = new List<ulong>
                {
                    NetworkManager.Singleton.ConnectedClientsIds[1],
                }
            };
            
            return new Team[] { blueTeam, redTeam };
        }
    }
}