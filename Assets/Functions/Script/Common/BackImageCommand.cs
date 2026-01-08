using System;
using Functions.Manager;
using UnityEngine;

namespace Functions.Script.Common
{
    [Serializable]
    public class BackImageCommand : ICommand
    {
        private string path;
        private Color32 color;
        
        public BackImageCommand(Json.ProcessJson prc)
        {
            path = prc.name;
            color = prc.color;
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetBackImage(path, color);
            return true;
        }
    }
}
