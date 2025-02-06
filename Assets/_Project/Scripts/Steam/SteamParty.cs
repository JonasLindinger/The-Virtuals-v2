using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _Project.Scripts.InfoSlider;
using _Project.Scripts.ModalWindow;
using _Project.Scripts.Utility;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Steam
{
    public class SteamParty : MonoBehaviour
    {
        #if Client
        private static bool _initialized = false;
        
        public static event Action OnPartyJoined = delegate {  };
        public static event Action OnPartyLeft = delegate {  };
        public static event Action OnPartyMemberJoined = delegate {  };
        public static event Action OnPartyMemberLeft = delegate {  };
        
        public static Lobby? CurrentLobby;
        
        // Blacklist of lobby Ids, so we don't join the same lobby twice and get invites twice
        private static List<SteamId> _inviteLobbyIds = new List<SteamId>();

        private void OnApplicationQuit()
        {
            LeaveParty();
        }

        public static void Init()
        {
            if (_initialized) return;
            
            // Steam Callbacks
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            SteamMatchmaking.OnLobbyMemberKicked += OnLobbyMemberKicked;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
            
            _initialized = true;
        }

        #region Callbacks
        
        // When you accept the invite or join on a friend
        private static async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            RoomEnter joinedLobby = await lobby.Join();
            if (joinedLobby != RoomEnter.Success)
            {
                Debug.LogWarning("Failed to join lobby: " + joinedLobby.ToString());
                
                // Show a ModalWindow with a warning
                ModalWindowInfo info = new ModalWindowInfo
                {
                    Title = "Error joining party" + joinedLobby.ToString(),
                    Message = "Something went wrong while joining Party. Please try again. Error: " + joinedLobby.ToString(),
                };
                ModalWindowManager.Instance.Show(info);
                return;
            }
            
            CurrentLobby = lobby;
            Debug.Log("Joined Lobby");
            OnPartyJoined?.Invoke();
        }
        
        // When a lobby member is kicked
        private static void OnLobbyMemberKicked(Lobby lobby, Friend kicked, Friend kicker)
        {
            if (kicked.IsMe)
            {
                CurrentLobby = null;
                OnPartyLeft?.Invoke();
            }
        }

        // Friend send you a steam invite
        private static void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log("Invite from " + friend.Name);
            
            // Ignore invites that you already have
            if (_inviteLobbyIds.Contains(lobby.Id))
                return;
            
            // Ignore invites that are for the current lobby
            if (CurrentLobby != null)
                if (CurrentLobby.HasValue)
                    if (CurrentLobby.Value.Id == lobby.Id)
                        return;
            
            _inviteLobbyIds.Add(lobby.Id);
            
            // Show a ModalWindow with a question
            ModalWindowInfo info = new ModalWindowInfo
            {
                Title = "Join Party?",
                Message = "Do you want to join the party of " + friend.Name + "?",
                Confirm = () =>
                {
                    lobby.Join();
                    CurrentLobby = lobby;
                    Debug.Log("Joined Lobby");
                    _inviteLobbyIds.Remove(lobby.Id);
                },
                Cancel = () => { _inviteLobbyIds.Remove(lobby.Id); }
            };
            ModalWindowManager.Instance.Show(info);
        }

        // When a lobby member leaves
        private static void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            if (friend.IsMe)
            {
                CurrentLobby = null;
                Debug.Log("I left the lobby");
                OnPartyLeft?.Invoke();
            }
            else
            {
                Debug.Log("Member " + friend.Name + " left the lobby");
                OnPartyMemberLeft?.Invoke();
            }
            
            InfoSliderManager.Instance.Show(friend.Name + " left the lobby");
        }

        // When a lobby member joins
        private static void OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            Debug.Log("Member " + friend.Name + " joined the lobby");
            OnPartyMemberJoined?.Invoke();
            InfoSliderManager.Instance.Show(friend.Name + " joined the lobby");
        }

        // When you enter a lobby
        private static void OnLobbyEntered(Lobby lobby)
        {
            // If we joined by invite, we remove the lobby from the blacklist
            if (_inviteLobbyIds.Contains(lobby.Id))
                _inviteLobbyIds.Remove(lobby.Id);
            
            Debug.Log("Entered Lobby");
            CurrentLobby = lobby;
            OnPartyJoined?.Invoke();
        }

        // When you create a lobby
        private static void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogWarning("Failed to create lobby");
                
                // Show a ModalWindow with a warning
                ModalWindowInfo info = new ModalWindowInfo
                {
                    Title = "Error creating party",
                    Message = "Failed to create party. Please try again. Error: " + result.ToString(),
                };
                ModalWindowManager.Instance.Show(info);
                return;
            }

            lobby.SetFriendsOnly();
            lobby.SetJoinable(true); // Todo: Make this an option
            // Todo: Make a button to leave the party
        }
        
        #endregion

        public static async Task<Lobby> GetALobbyToInviteFriend()
        {
            // Check if we aren't in a lobby, so we have to create one
            if (CurrentLobby == null)
                return await CreateParty();
            else if (!CurrentLobby.HasValue)
                return await CreateParty();
            
            // We are in a lobby and return the lobby
            return CurrentLobby.Value;
        }
        
        private static async Task<Lobby> CreateParty()
        {
            CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(Settings.MaxPartySize);
            Debug.Log("Party created");
            return CurrentLobby.Value;
        }

        public static void LeaveParty()
        {
            // Savety checks
            if (CurrentLobby == null)
            {
                // Show a ModalWindow with a warning
                ModalWindowInfo info = new ModalWindowInfo
                {
                    Title = "There is no party to leave",
                    Message = "You are currently not in a party, so you can't leave one. This might me a bug. If you want, you can report this to the developers.",
                };
                ModalWindowManager.Instance.Show(info);
                
                Debug.LogWarning("No party to leave");
                return;
            }
            if (!CurrentLobby.HasValue)
            {
                // Show a ModalWindow with a warning
                ModalWindowInfo info = new ModalWindowInfo
                {
                    Title = "There is no party to leave",
                    Message = "You are currently not in a party, so you can't leave one. This might me a bug. If you want, you can report this to the developers.",
                };
                ModalWindowManager.Instance.Show(info);
                Debug.LogWarning("No party to leave");
                return;
            }

            CurrentLobby.Value.Leave();
            CurrentLobby = null;
            
            Debug.Log("Party left");
        }

        public static List<Friend> GetPartyMembers()
        {
            return CurrentLobby.Value.Members.ToList();
        }
        #endif
    }
}