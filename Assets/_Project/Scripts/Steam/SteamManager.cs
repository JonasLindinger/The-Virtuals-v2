using _Project.Scripts.ModalWindow;
using _Project.Scripts.Utility;
using SceneManagement;
using Steamworks;
using UnityEngine;

namespace _Project.Scripts.Steam
{
    public class SteamManager : MonoBehaviour
    {
        #if Client
        private void Start()
        {
            // Steam and Backend
            ConnectToSteam();
            
            // Savety check
            if (!SteamClient.IsValid) return;
            
            // Loading into Main Menu
            SceneLoader.GetInstance().LoadSceneGroup(0);
        }

        private void ConnectToSteam()
        {
            try
            {
                SteamParty.Init();
                SteamClient.Init(Settings.SteamId);
                if (SteamClient.IsValid)
                {
                    Debug.Log("Connected to Steam as " + SteamClient.Name + "(" + SteamClient.SteamId + ")");
                }
                else
                {
                    Debug.LogWarning("Something went wrong while connecting to Steam.");
                    
                    // Show a ModalWindow with a warning
                    ModalWindowInfo info = new ModalWindowInfo
                    {
                        Title = "Steam Connection Error",
                        Message = "Something went wrong while connecting to Steam. Please check your internet and confirm that your Steam client is running.",
                        Confirm = ApplicationManager.Quit,
                    };
                    ModalWindowManager.Instance.Show(info);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Something went wrong while connecting to Steam. ");
                
                // Show a ModalWindow with a warning
                ModalWindowInfo info = new ModalWindowInfo
                {
                    Title = "Steam Connection Error " + e.InnerException,
                    Message = "Something went wrong while connecting to Steam. Please check your internet and confirm that your Steam client is running. ",
                    Confirm = ApplicationManager.Quit,
                };
                ModalWindowManager.Instance.Show(info);
            }
        }
        
        private void Update()
        {
            if (SteamClient.IsValid)
                SteamClient.RunCallbacks();
        }

        private void OnApplicationQuit()
        {
            if (SteamClient.IsValid)
            {
                Debug.Log("Shutdown Steam");
                SteamClient.Shutdown();
            }
        }
        #endif
    }
}