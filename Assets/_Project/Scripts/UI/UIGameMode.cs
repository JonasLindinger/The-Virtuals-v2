using System;
using _Project.Scripts.Matchmaking;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class UIGameMode  : MonoBehaviour
        #if Client
        ,IPointerEnterHandler, IPointerExitHandler
        #endif
    {
        [Header("Folding Animation")]
        [SerializeField] private RectTransform _movingPart;
        [SerializeField] private bool _shouldStartExtended = false;
        [SerializeField] private float _closedYPosition = 110;
        [SerializeField] private float _extendedYPosition = 0;
        [SerializeField] private float _closedHeight = 60;
        [SerializeField] private float _extendedHeight = 110;
        [SerializeField] private float _animationDuration = 0.15f;
        [Space(10)] 
        [SerializeField] private TMP_Text _text;
        
        #if Client
        private bool _isAnimating;
        private bool _lastAnimationWasExtend;
        private bool _isExtended;

        private float _defaultWidth;
        
        private RectTransform _rectTransform;
        private VerticalLayoutGroup _layoutGroup;
        
        private GameModeInfo _gameModeInfo;
        
        #region Folding Animation

        private void Start()
        {
            _layoutGroup = transform.parent.GetComponent<VerticalLayoutGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _defaultWidth = _rectTransform.sizeDelta.x;
            
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
            _movingPart.DOAnchorPosY(_extendedYPosition, _animationDuration).OnUpdate(() => _layoutGroup.SetLayoutVertical());
            _rectTransform.DOSizeDelta(new Vector2(_defaultWidth, _extendedHeight), _animationDuration);
            _isAnimating = true;
            _lastAnimationWasExtend = true;
            Invoke(nameof(Reset), _animationDuration);
        }

        public void Close()
        {
            _isExtended = false;
            if (_isAnimating)
                return;
            
            // Moving the expand section down
            _movingPart.DOAnchorPosY(_closedYPosition, _animationDuration);
            // Resizing the friend section
            _rectTransform.DOSizeDelta(new Vector2(_defaultWidth, _closedHeight), _animationDuration)
                // Updating the layout group for applying the resizing
                .OnUpdate(() => _layoutGroup.SetLayoutVertical());
            _isAnimating = true;
            _lastAnimationWasExtend = false;
            Invoke(nameof(Reset), _animationDuration);
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

        public void SetUp(GameModeInfo gameModeInfo)
        {
            _gameModeInfo = gameModeInfo;
            
            _text.text = gameModeInfo._displayName;
        }
        
        public void Play()
        {
            MatchmakingManager.Instance.StartMatchmaking(_gameModeInfo);
        }
        #endif
    }
}