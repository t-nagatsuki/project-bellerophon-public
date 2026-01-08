using System;
using Functions.Manager;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace Functions.Script.Common
{
    [Serializable]
    public class ExitCommand : ICommand
    {
        public ExitCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            return true;
        }
    }
}
