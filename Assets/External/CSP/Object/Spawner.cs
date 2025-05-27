using _Project.Scripts.Utility;
using Singletons;
using Unity.Netcode;
using UnityEngine;

namespace CSP.Object
{
    public class Spawner : MonoBehaviourSingleton<Spawner>
    {
        [Header("References")]
        [SerializeField] private NetworkClient networkClientPrefab;
        
        #if Server

        private void Start()
        {
            #if Server
            // Auto spawn Client object for communication
            NetworkManager.Singleton.OnConnectionEvent += (manager, eventData) =>
            {
                if (eventData.EventType == ConnectionEvent.ClientConnected)
                    SpawnObjectAnonymousWithOwner(eventData.ClientId, networkClientPrefab.gameObject, transform);
            };
            #endif
        }
        
        public static void SpawnObjectAnonymousWithOwner(ulong clientId, GameObject networkObject, Transform spawnPoint)
        {
            var newObject = Instantiate(networkObject.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();
            newObject.AlwaysReplicateAsRoot = false;
            newObject.SynchronizeTransform = false;
            newObject.ActiveSceneSynchronization = false;
            newObject.SceneMigrationSynchronization = false;
            newObject.AutoObjectParentSync = false;
            newObject.SyncOwnerTransformWhenParented = false;
            newObject.AllowOwnerToParent = false;
            
            // Only the client and we as the server should see the object
            newObject.SpawnWithObservers = false;
            
            // This object should be destroyed when the owner leaves
            newObject.DontDestroyWithOwner = false;
            
            newObject.SpawnWithOwnership(clientId);
            newObject.NetworkShow(clientId);
        }
        
        public static void SpawnObjectAnonymousWithOwnerPermanent(ulong clientId, GameObject networkObject, Transform spawnPoint)
        {
            var newObject = Instantiate(networkObject.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();
            newObject.AlwaysReplicateAsRoot = false;
            newObject.SynchronizeTransform = false;
            newObject.ActiveSceneSynchronization = false;
            newObject.SceneMigrationSynchronization = false;
            newObject.AutoObjectParentSync = false;
            newObject.SyncOwnerTransformWhenParented = false;
            newObject.AllowOwnerToParent = false;
            
            // Only the client and we as the server should see the object
            newObject.SpawnWithObservers = false;
            
            // This object should not be destroyed when the owner leaves
            newObject.DontDestroyWithOwner = true;
            
            newObject.SpawnWithOwnership(clientId);
            newObject.NetworkShow(clientId);
        }
        
        public static void SpawnObjectPublicWithOwner(ulong clientId, GameObject networkObject, Transform spawnPoint)
        {
            var newObject = Instantiate(networkObject.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();
            newObject.AlwaysReplicateAsRoot = false;
            newObject.SynchronizeTransform = false;
            newObject.ActiveSceneSynchronization = false;
            newObject.SceneMigrationSynchronization = false;
            newObject.AutoObjectParentSync = false;
            newObject.SyncOwnerTransformWhenParented = false;
            newObject.AllowOwnerToParent = false;
            
            // This object should be seen by everyone
            newObject.SpawnWithObservers = true;
            
            // This object should be destroyed when the owner leaves
            newObject.DontDestroyWithOwner = false;
            
            newObject.SpawnWithOwnership(clientId);
        }
        
        public static void SpawnObjectPublicWithOwnerPermanent(ulong clientId, GameObject networkObject, Transform spawnPoint)
        {
            var newObject = Instantiate(networkObject.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<NetworkObject>();
            newObject.AlwaysReplicateAsRoot = false;
            newObject.SynchronizeTransform = false;
            newObject.ActiveSceneSynchronization = false;
            newObject.SceneMigrationSynchronization = false;
            newObject.AutoObjectParentSync = false;
            newObject.SyncOwnerTransformWhenParented = false;
            newObject.AllowOwnerToParent = false;
            
            // This object should be seen by everyone
            newObject.SpawnWithObservers = true;
            
            // This object should NOT be destroyed when the owner leaves
            newObject.DontDestroyWithOwner = true;
            
            newObject.SpawnWithOwnership(clientId);
        }
        #endif
    }
}