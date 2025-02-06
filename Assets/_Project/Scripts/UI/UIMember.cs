using _Project.Scripts.Steam;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class UIMember : MonoBehaviour
    {
        [SerializeField] private TMP_Text _friendName;
        [SerializeField] private RawImage _profileImage;

        #if Client
        public Friend Friend;
        
        #region Friend

        public async void SetUp(Friend friend)
        {
            Friend = friend;
            
            _friendName.text = friend.Name;
            _profileImage.texture = await SteamDisplayAvatar.GetAvatarTexture(friend.Id);
        }

        #endregion
        #endif
    }
}