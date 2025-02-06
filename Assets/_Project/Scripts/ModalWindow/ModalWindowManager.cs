using System;
using System.Collections.Generic;
using _Project.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.ModalWindow
{
    public class ModalWindowManager : PersistentSingleton<ModalWindowManager>
    {
        [Header("General")]
        [SerializeField] private GameObject _canvas;
        [Space(5)]
        [Header("Header")] 
        [SerializeField] private Transform _headerArea;
        [SerializeField] private TMP_Text _titleField;
        [Space(5)]
        [Header("Content")]
        [SerializeField] private Transform _contentArea;
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        [Space(5)]
        [Header("Footer")]
        [SerializeField] private Transform _footerArea;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _alternateButton;

        #if Client
        private event Action OnConfirm = delegate { };
        private event Action OnCancel = delegate { };
        private event Action OnAlternate = delegate { };

        private Queue<ModalWindowInfo> _modalWindowQueue = new Queue<ModalWindowInfo>();
        private bool _isShowing = false;
        
        private void Start()
        {
            Close();
        }

        public void Confirm()
        {
            OnConfirm?.Invoke();
            Close();
        }
        
        public void Cancel()
        {
            OnCancel?.Invoke();
            Close();
        }
        
        public void Alternate()
        {
            OnAlternate?.Invoke();
            Close();
        }

        public void Show(ModalWindowInfo info)
        {
            _modalWindowQueue.Enqueue(info);
            
            // Start showing the window with the exeption, that we are already showing something
            if (!_isShowing) 
                ShowModalWindow(_modalWindowQueue.Dequeue());
        }
        
        private void ShowModalWindow(ModalWindowInfo info)
        {
            _isShowing = true;
            
            // Hide the header if there's no title
            bool hasTitle = !string.IsNullOrEmpty(info.Title);
            _headerArea.gameObject.SetActive(hasTitle);
            _titleField.text = info.Title;
            
            // Hide the image if there's no image
            bool hasImage = info.Image != null;
            _image.gameObject.SetActive(hasImage);
            _image.sprite = info.Image;
            
            // Hide the message if there's no message text
            bool hasMessage = !string.IsNullOrEmpty(info.Message);
            _text.gameObject.SetActive(hasMessage);
            _text.text = info.Message;
            
            // We should always have a confirm action
            OnConfirm += info.Confirm;
            
            // Hide cancel if there is no cancel action
            bool hasCancel = info.Cancel != null;
            _cancelButton.gameObject.SetActive(hasCancel);
            OnCancel += info.Cancel;
            
            // Hide alternate if there is no alternate action
            bool hasAlternate = info.Alternate != null;
            _alternateButton.gameObject.SetActive(hasAlternate);
            OnAlternate += info.Alternate;

            Show();
        }

        private void Show()
        {
            // Show the window
            _canvas.SetActive(true);
        }
        
        private void Close()
        {
            // Hide the window
            _canvas.SetActive(false);
            
            // Set flag to false
            _isShowing = false;
            
            // Show next
            if (_modalWindowQueue.Count > 0)
                ShowModalWindow(_modalWindowQueue.Dequeue());
        }
        #endif
    }
}