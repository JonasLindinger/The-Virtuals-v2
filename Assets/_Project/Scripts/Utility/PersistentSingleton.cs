using System;
using UnityEngine;

namespace _Project.Scripts.Utility
{
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        [Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
        public bool _unparentOnAwake = true;
        
        public static T Instance;

        private protected void Awake()
        {
            InitializeSingleton();
            LateAwake();
        }

        public virtual void LateAwake() { }

        private protected void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }

            LateDestroy();
        }
        
        public virtual void LateDestroy() {}

        private protected void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Application is not playing, skipping Singleton initialization");
                return;
            }

            if (_unparentOnAwake)
            {
                transform.SetParent(null);
            }

            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                enabled = true;
            }
            else
            {
                Debug.LogWarning("Dupplicate Singleton: " + typeof(T));
                if (this != Instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}