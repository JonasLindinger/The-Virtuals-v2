using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;
using UnityEngine;
using Image = Steamworks.Data.Image;

namespace _Project.Scripts.Steam
{
    public static class SteamDisplayAvatar
    {
        #if Client
        private static Dictionary<SteamId, Texture2D> _profilePictures = new Dictionary<SteamId, Texture2D>();
        
        public static async Task<Texture2D> GetAvatarTexture(SteamId steamId, bool checkCache = true)
        {
            // Check Cache
            if (_profilePictures.ContainsKey(steamId) && checkCache)
                return _profilePictures[steamId];
            
            // Get the task
            var avatar = GetAvatar(steamId);

            // Use Task.WhenAll, to cache multiple items at the same time
            await Task.WhenAll(avatar);

            // Cache Items
            Texture2D texture =  avatar.Result?.Covert();

            _profilePictures.Add(steamId, texture);
            
            return texture;
        }
        
        private static async Task<Image?> GetAvatar(SteamId steamId)
        {
            try
            {
                // Get Avatar using await
                return await Steamworks.SteamFriends.GetLargeAvatarAsync(steamId);
            }
            catch (Exception e)
            {
                // If something goes wrong, log it
                Debug.Log(e);
                return null;
            }
        }
        
        private static Texture2D Covert(this Image image)
        {
            // Create a new Texture2D
            var avatar = new Texture2D( (int)image.Width, (int)image.Height, TextureFormat.ARGB32, false);
	
            // Set filter type, or else its really blury
            avatar.filterMode = FilterMode.Trilinear;

            // Flip image
            for ( int x = 0; x < image.Width; x++ )
            {
                for ( int y = 0; y < image.Height; y++)
                {
                    var p = image.GetPixel(x, y);
                    avatar.SetPixel(x, (int)image.Height - y, new UnityEngine.Color( p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f ));
                }
            }
	
            avatar.Apply();
            return avatar;
        }
        #endif
    }
}