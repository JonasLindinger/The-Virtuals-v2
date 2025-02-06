using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class UIFriendList : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _friendsList;
        [SerializeField] private UIFriend _friendPrefab;
        
        #if Client
        private List<Friend> _friends;
        private List<UIFriend> _uiFriends = new List<UIFriend>();
        
        private void Start()
        {
            DisplayFriends();
            SteamFriends.OnPersonaStateChange += UpdateFriend;
        }
        
        private async void DisplayFriends()
        {
            await ClearList();
            await ShowFriendsInList();
        }
        
        private async Task ClearList()
        {
            foreach (Transform child in _friendsList)
            {
                Destroy(child.gameObject);
            }
            
            while (_friendsList.childCount != 0)
                await Task.Delay(1);
        }

        private async Task ShowFriendsInList()
        {
            // Getting Friends
            _friends = SteamFriends.GetFriends().ToList();

            // Get all game-playing friends
            var gameOnlineFriends = _friends.Where(friend => friend.IsPlayingThisGame).ToList();

            // Get all online friends who are not playing this game and are not away, snoozing, or busy
            var onlineFriends = _friends.Where(friend => friend.IsOnline && !friend.IsPlayingThisGame && !(friend.IsAway || friend.IsSnoozing || friend.IsBusy)).ToList();

            // Get all friends who are away, snoozing, or busy
            var notAvailableFriends = _friends.Where(friend => friend.IsAway || friend.IsSnoozing || friend.IsBusy).ToList();

            // Get all offline friends
            var offlineFriends = _friends.Where(friend => !friend.IsOnline).ToList();

            // Todo: Check for is Blocked and put them down the list on top of the offline dudes or just ignore him and don't display him.
            foreach (var friend in gameOnlineFriends)
            {
                // Todo: Check if some friends are in a party. If they are display, that they are in a party
                _uiFriends.Add(InstantiateFriend(friend));
            }
            await Task.Delay(1);
            foreach (var friend in onlineFriends)
            {
                _uiFriends.Add(InstantiateFriend(friend));
            }
            await Task.Delay(1);
            foreach (var friend in notAvailableFriends)
            {
                _uiFriends.Add(InstantiateFriend(friend));
            }
            await Task.Delay(1);
            foreach (var friend in offlineFriends)
            {
                _uiFriends.Add(InstantiateFriend(friend));
            }

            while (_friendsList.childCount < _friends.Count)
                await Task.Delay(1);
        }

        private UIFriend InstantiateFriend(Friend friend)
        {
            UIFriend uiFriend = Instantiate(_friendPrefab, _friendsList);
            uiFriend.SetUp(friend);
            return uiFriend;
        }

        private void UpdateFriend(Friend friend)
        {
            // Safety check for null friend data
            if (friend.Equals(null) || !friend.Id.IsValid)
            {
                // If friend is null or invalid, don't proceed
                DisplayFriends();
                return;
            }
            
            Friend oldFriendData = _friends.Find(f => f.Id == friend.Id);
            
            // Savety check
            if (oldFriendData.Equals(null) || !oldFriendData.Id.IsValid)
            {
                _friends.Add(friend);
                DisplayFriends();
                return;
            }           
            
            // Track if there are any significant changes (e.g., online status or game status)
            bool hasChanged = oldFriendData.IsPlayingThisGame != friend.IsPlayingThisGame ||
                              oldFriendData.IsOnline != friend.IsOnline ||
                              oldFriendData.IsAway != friend.IsAway ||
                              oldFriendData.IsSnoozing != friend.IsSnoozing ||
                              oldFriendData.IsBusy != friend.IsBusy;
            
            // Updating list
            _friends.Remove(oldFriendData);
            _friends.Add(friend);
            
            // Updating UI
            UIFriend uiFriend = _uiFriends.Find(f => f.Friend.Id == friend.Id);
            uiFriend.SetUp(friend);

            if (hasChanged)
            {
                // Todo: Move this friend object up or down depending on the new state
                DisplayFriends();
            }
        }
        #endif
    }
}