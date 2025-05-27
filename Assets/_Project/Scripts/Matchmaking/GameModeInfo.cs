using UnityEngine.Serialization;

namespace _Project.Scripts.Matchmaking
{
    [System.Serializable]
    public class GameModeInfo
    {
        public string _displayName;
        public GameMode gameMode;
    }
}