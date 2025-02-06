using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.Steam;
using Steamworks;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class UIMemberList : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _membersList;
        [SerializeField] private UIMember _memberPrefab;
        
        #if Client
        private List<Friend> _members;
        private List<UIMember> _uiMembers = new List<UIMember>();

        private void Start()
        {
            DisplayMembers();

            SteamParty.OnPartyJoined += DisplayMembers;
            SteamParty.OnPartyLeft += DisplayMembers;
            SteamParty.OnPartyMemberJoined += DisplayMembers;
            SteamParty.OnPartyMemberLeft += DisplayMembers;
        }

        private void OnDestroy()
        {
            SteamParty.OnPartyJoined -= DisplayMembers;
            SteamParty.OnPartyLeft -= DisplayMembers;
            SteamParty.OnPartyMemberJoined -= DisplayMembers;
            SteamParty.OnPartyMemberLeft -= DisplayMembers;
        }

        private async void DisplayMembers()
        {
            await ClearList();
            await ShowFriendsInList();
        }
        
        private async Task ClearList()
        {
            foreach (Transform child in _membersList)
            {
                Destroy(child.gameObject);
            }
            
            while (_membersList.childCount != 0)
                await Task.Delay(1);
        }

        private async Task ShowFriendsInList()
        {
            if (SteamParty.CurrentLobby == null)
            {
                _uiMembers.Add(InstantiateMe());
                
                while (_membersList.childCount != 1)
                    await Task.Delay(1);
                return;
            }

            if (!SteamParty.CurrentLobby.HasValue)
            {
                _uiMembers.Add(InstantiateMe());
                
                while (_membersList.childCount != 1)
                    await Task.Delay(1);
            }
            else
            {
                // Instantiate all Members with me at the top
                _uiMembers.Add(InstantiateMe());
                foreach (var member in SteamParty.GetPartyMembers())
                {
                    if (member.IsMe) continue;
                    _uiMembers.Add(InstantiateMember(member));
                }
                
                while (_membersList.childCount != SteamParty.GetPartyMembers().Count)
                    await Task.Delay(1);
            }
        }
        
        private UIMember InstantiateMember(Friend member)
        {
            UIMember uiMember = Instantiate(_memberPrefab, _membersList);
            uiMember.SetUp(member);
            return uiMember;
        }

        private UIMember InstantiateMe()
        {
            UIMember uiMember = Instantiate(_memberPrefab, _membersList);
            Friend me = new Friend(SteamClient.SteamId);
            uiMember.SetUp(me);
            return uiMember;
        }
        #endif
    }
}