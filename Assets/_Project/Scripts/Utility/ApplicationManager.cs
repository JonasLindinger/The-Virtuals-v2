using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Utility
{
    public static class ApplicationManager
    {
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