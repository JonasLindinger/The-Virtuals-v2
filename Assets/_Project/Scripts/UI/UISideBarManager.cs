using _Project.Scripts.ModalWindow;
using _Project.Scripts.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI
{
    public class UISideBarManager : MonoBehaviour
        #if Client
        , IPointerEnterHandler, IPointerExitHandler
        #endif
    {
        [Header("Folding Animation")]
        [SerializeField] private RectTransform _movingPart;
        [SerializeField] private bool _shouldStartExtended = false;
        [SerializeField] private float _closedXPosition;
        [SerializeField] private float _extendedXPosition = 400;
        [SerializeField] private float _foldAnimationDuration = 0.25f;
        [Space(10)]
        [Header("Animation")]
        [SerializeField] private float _hightlightAnimationDuration = 0.35f;
        [Space(10)]
        [Header("References")]
        [SerializeField] private GameObject[] _menus;
        [SerializeField] private GameObject[] _icons;
        [SerializeField] private Transform _hightlighter;

        #if Client
        private bool _isAnimating;
        private bool _lastAnimationWasExtend;
        private bool _isExtended;

        #region Folding Animation
        
        private void Start()
        {
            int menu = 0;
            OpenMenuWithoutHighlighter(_menus[menu]);
            _hightlighter.position = _icons[menu].transform.position;
            
           if (_shouldStartExtended) 
               Extend();
           else
           {
               Close();
           }
        }

        public void Extend()
        {
            _isExtended = true;
            if (_isAnimating)
                return;
            _movingPart.DOAnchorPosX(_extendedXPosition, _foldAnimationDuration);
            _isAnimating = true;
            _lastAnimationWasExtend = true;
            Invoke(nameof(Reset), _foldAnimationDuration);
        }

        public void Close()
        {
            _isExtended = false;
            if (_isAnimating)
                return;
            
            _movingPart.DOAnchorPosX(_closedXPosition, _foldAnimationDuration);
            _isAnimating = true;
            _lastAnimationWasExtend = false;
            Invoke(nameof(Reset), _foldAnimationDuration);
        }

        private void Reset()
        {
            _isAnimating = false;
            if (_lastAnimationWasExtend != _isExtended)
            {
                if (_lastAnimationWasExtend)
                    Close();
                else
                    Extend();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Extend();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Close();
        }
        
        #endregion
        
        #region Buttons

        public void OpenMenu(GameObject menu)
        {
            int idx = -1;

            for (int i = 0; i < _menus.Length; i++)
            {
                GameObject currentMenu = _menus[i];
                if (currentMenu == menu)
                    idx = i;
                currentMenu.SetActive(currentMenu.GetInstanceID() == menu.GetInstanceID());
            }

            if (idx == -1)
            {
                Debug.LogWarning("Something went wrong.");
                return;
            }

            _hightlighter.DOMove(_icons[idx].transform.position, _hightlightAnimationDuration);
        }
        
        public void OpenMenuWithoutHighlighter(GameObject menu)
        {
            int idx = -1;

            for (int i = 0; i < _menus.Length; i++)
            {
                GameObject currentMenu = _menus[i];
                currentMenu.SetActive(currentMenu.GetInstanceID() == menu.GetInstanceID());
            }
        }
        
        public void QuitApplication()
        {
            ModalWindowInfo info = new ModalWindowInfo
            {
                Title = "Quit Application? ",
                Message = "Are you sure you want to quit the application?",
                Confirm = () => ApplicationManager.Quit(),
                Cancel = () => { }
            };
            
            ModalWindowManager.Instance.Show(info);
        }
        
        #endregion
        #endif
    }
}