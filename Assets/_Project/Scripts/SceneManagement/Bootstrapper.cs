using _Project.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.SceneManagement
{
    public class Bootstrapper : PersistentSingleton<Bootstrapper>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async void Init()
        {
            Debug.Log("Bootstrapper...");
            if (SceneManager.GetActiveScene().name == "Bootstrapper")
                return;
            await SceneManager.LoadSceneAsync("Bootstrapper", LoadSceneMode.Single);
        }   
    }
}