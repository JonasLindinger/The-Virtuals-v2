using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Matchmaking;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.UI
{
    public class UIGameModeList : MonoBehaviour
    {
        [FormerlySerializedAs("_gamemodeList")]
        [Header("References")]
        [SerializeField] private Transform _gameModeList;
        [SerializeField] private UIGameMode _gameModePrefab;
        [SerializeField] private List<GameModeInfo> _gameModes;
        
        #if Client
        private List<UIGameMode> _uiGameModes = new List<UIGameMode>();

        private void Start()
        {
            DisplayMembers();
        }

        private async void DisplayMembers()
        {
            await ClearList();
            await ShowFriendsInList();
        }
        
        private async Task ClearList()
        {
            foreach (Transform child in _gameModeList)
            {
                Destroy(child.gameObject);
            }
            
            while (_gameModeList.childCount != 0)
                await Task.Delay(1);
        }

        private async Task ShowFriendsInList()
        {
            // Instantiate all GameModes
            foreach (var gameMode in _gameModes)
            {
                _uiGameModes.Add(InstantiateMember(gameMode));
            }
                
            while (_gameModeList.childCount != _gameModes.Count)
                await Task.Delay(1);
        }
        
        private UIGameMode InstantiateMember(GameModeInfo gameMode)
        {
            UIGameMode uiGameMode = Instantiate(_gameModePrefab, _gameModeList);
            uiGameMode.SetUp(gameMode);
            return uiGameMode;
        }
        #endif
    }
}