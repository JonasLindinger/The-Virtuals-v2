using CSP.Simulation;
using Unity.Netcode;

namespace CSP.Object
{
    public abstract class NetworkedObject : NetworkBehaviour, INetworkedObject
    {
        protected virtual void Start()
        {
            Register();
            OnStart();
        }
        
        public void Register()
        {
            SnapshotManager.RegisterNetworkedObject(NetworkObjectId, this);
        }

        public virtual void OnStart()
        {
            // Just for overriding purposes
        }
        public abstract IState GetCurrentState();
        public abstract void ApplyState(uint tick, IState state);
    }
}