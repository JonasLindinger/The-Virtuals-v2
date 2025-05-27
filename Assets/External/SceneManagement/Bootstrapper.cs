using Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Bootstrapper : MonoBehaviourSingleton<Bootstrapper>
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