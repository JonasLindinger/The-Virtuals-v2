using System.Threading.Tasks;
using _Project.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.SceneManagement
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        [SerializeField] private Image _loadingBar;
        [SerializeField] private float _fillSpeed = 0.5f;
        [SerializeField] private Canvas _loadingCanvas;
        [SerializeField] private Camera _loadingCamera;
        [SerializeField] private SceneGroup[] _sceneGroups;
        
        private float _targetProgress;
        private bool _isLoading;

        public readonly SceneGroupManager Manager = new SceneGroupManager();

        private void Start()
        {
            ShowLoadingCanvas(false);
            Manager.OnSceneLoaded += (sceneName) => { Debug.Log("Loaded: " + sceneName); };
            Manager.OnSceneUnloaded += (sceneName) => { Debug.Log("Unloaded: " + sceneName); };
            Manager.OnSceneGroupLoaded += () => { Debug.Log("Scene group loaded"); };
        }

        private void Update()
        {
            if (!_isLoading) return;

            float currentFillAmount = _loadingBar.fillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - _targetProgress);
            
            float dynamicFillSpeed = progressDifference * _fillSpeed;
            
            _loadingBar.fillAmount = Mathf.Lerp(currentFillAmount, _targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            _loadingBar.fillAmount = 0;
            _targetProgress = 1f;

            // Just a safety check for the index
            if (index < 0 || index >= _sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += (target) =>
            {
                _targetProgress = Mathf.Max(target, _targetProgress);
            };

            ShowLoadingCanvas();
            await Manager.LoadScenes(_sceneGroups[index], progress);
            ShowLoadingCanvas(false);
        }

        private void ShowLoadingCanvas(bool show = true)
        {
            _isLoading = show;
            _loadingCanvas.gameObject.SetActive(show);
            _loadingCamera.gameObject.SetActive(show);
        }
    }
}