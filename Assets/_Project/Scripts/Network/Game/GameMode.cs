using _Project.Scripts.Network.Connection;
using UnityEngine;

namespace _Project.Scripts.Network.Game
{
    public abstract class GameMode : MonoBehaviour
    {
        [HideInInspector] public string _name = "New Game Mode";
        [HideInInspector] public int _wantedPlayers = 2;
        [HideInInspector] public bool _started = false;

        #if Server
        public abstract void Init();
        public abstract void StartGame();
        public abstract void PlayerConnected(Client client);
        public abstract void PlayerDisconnected(Client client);
        #endif
    }
}