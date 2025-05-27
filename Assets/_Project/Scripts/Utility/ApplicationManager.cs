using System;
using CSP;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Utility
{
    public class ApplicationManager : MonoBehaviour
    {
        #if Client
        private void Start()
        {
            LimitFPS();
        }

        private void LimitFPS()
        {
            // Todo: Make this a setting and update on change
            Application.targetFrameRate = 240;
        }
        #endif

        public static void Quit()
        {
            Debug.Log("Quitting Application");
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}