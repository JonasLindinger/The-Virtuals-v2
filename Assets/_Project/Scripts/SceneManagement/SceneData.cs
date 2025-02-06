using System;
using Eflatun.SceneReference;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace _Project.Scripts.SceneManagement
{
    [Serializable]
    public class SceneData
    {
        public SceneReference _reference;
        public string Name => _reference.Name;
        public SceneType _sceneType;
    }
}