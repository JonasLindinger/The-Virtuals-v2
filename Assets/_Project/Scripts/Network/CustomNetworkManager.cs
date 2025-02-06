using System;
using System.Threading.Tasks;
using _Project.Scripts.Network.Connection;
using _Project.Scripts.Network.Game;
using _Project.Scripts.SceneManagement;
using _Project.Scripts.Utility;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Project.Scripts.Network
{
    [RequireComponent(typeof(UnityTransport))]
    public class CustomNetworkManager : NetworkManager
    {
        public static CustomNetworkManager Instance { get; private set; }
        
        #if Server
        public static event Action OnServerStarted = delegate { }; 
        #endif
        
        private UnityTransport _transport;

        private void Start()
        {
            // Set up Singleton
            Instance = this;

            SetUp();

            #if Server
            // Start Server if this is a server build
            StartGameServer();
            #endif
        }
        
        #if Client

        public async void ConnectToServer(string ip, ushort port)
        {
            // Loading network scene
            await SceneLoader.Instance.LoadSceneGroup(1);

            // Start Client using the ip and port from settings
            // Todo: Use the given ip and port from multiplay
            _transport.SetConnectionData(ip, port);
            Singleton.NetworkConfig.ConnectionData = ConnectionPayload.GetPayload();
            if (StartClient()) 
            {
                Debug.Log("Client started");
            }
            else 
            {
                Debug.LogError("Client could not connect to server"); 
            }
        }
        
        #elif Server
        
        private async void StartGameServer()
        {
            // Loading network scene
            await SceneLoader.Instance.LoadSceneGroup(1);

            // Start Server using the ip and port from settings
            // Todo: Use the given ip and port from multiplay
            _transport.SetConnectionData(Settings.IP, Settings.Port);
            if (StartServer())
            {
                Debug.Log("Starting Server");
                OnServerStarted?.Invoke();
            }
            else
            {
                Debug.LogError("Server count not be started");
            }
        }
        
        #endif

        private void SetUp()
        {
            // Reference
            _transport = GetComponent<UnityTransport>();
            
            // Connect Network Transport and set Network Tick Rate
            Singleton.NetworkConfig.NetworkTransport = _transport;
            Singleton.NetworkConfig.TickRate = Settings.NetworkTickRate;
            Singleton.NetworkConfig.ConnectionApproval = true;
            Singleton.NetworkConfig.EnableSceneManagement = false;
            
            // Subscribe to events
            #if Server
            Singleton.ConnectionApprovalCallback = ConnectionApproval.OnConnectionRequest;
            #endif
            Singleton.OnConnectionEvent += (manager, eventData) => { ConnectionCallbacks.OnConnectionEvent(eventData); };
        }
    }
}