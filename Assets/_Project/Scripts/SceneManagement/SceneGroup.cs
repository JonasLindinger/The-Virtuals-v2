using System;
using System.Collections.Generic;
using System.Linq;

namespace _Project.Scripts.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string _groupName = "New Scene Group";
        public List<SceneData> _scenes;
        public string FindSceneNameByType(SceneType sceneType)
        {
            return _scenes.FirstOrDefault(scene => scene._sceneType == sceneType)?._reference.Name;
        }
    }
}