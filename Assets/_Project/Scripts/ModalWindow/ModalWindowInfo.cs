using System;
using UnityEngine;

namespace _Project.Scripts.ModalWindow
{
    public struct ModalWindowInfo
    {
        public string Title;
        public Sprite Image;
        public string Message;
        public Action Confirm;
        public Action Cancel;
        public Action Alternate;
    }
}