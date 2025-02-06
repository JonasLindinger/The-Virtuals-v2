using System.Collections.Generic;
using _Project.Scripts.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.InfoSlider
{
    public class InfoSliderManager : PersistentSingleton<InfoSliderManager>
    {
        [Header("Folding Animation")]
        [SerializeField] private RectTransform _movingPart;
        [SerializeField] private float _closedYPosition = -572;
        [SerializeField] private float _extendedYPosition = 0;
        [SerializeField] private float _animationDuration = 0.25f;
        [SerializeField] private float _displayTime = 3f;
        [Header("References")]
        [SerializeField] private TMP_Text _text;
        #if Client
        private Queue<string> _infoQueue = new Queue<string>();
        
        private bool _isShowing = false;
        private const float TimeBetweenTicks = 10;
        private void Start()
        {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(TimeBetweenTicks);
            
            // Hide this window as default
            _movingPart.DOMoveX(_closedYPosition, 0);
        }

        public void Show(string text)
        {
            _infoQueue.Enqueue(text);
            
            // Start showing the window with the exeption, that we are already showing something
            if (!_isShowing) 
                ShowModalWindow(_infoQueue.Dequeue());
        }
        
        private void ShowModalWindow(string text)
        {
            _isShowing = true;

            _text.text = text;

            Show();
        }

        private void Show()
        {
            _movingPart.DOMoveX(_extendedYPosition, _animationDuration).OnComplete(() =>
            {
                Invoke(nameof(Close), _displayTime);
            });
        }

        private void Close()
        {
            _movingPart.DOMoveX(_closedYPosition, _animationDuration).OnComplete(() =>
            {
                Reset();
            });
        }
        
        private void Reset()
        {
            // Set flag to false
            _isShowing = false;
            
            // Show next
            if (_infoQueue.Count > 0)
                ShowModalWindow(_infoQueue.Dequeue());
        }
        #endif
    }
}