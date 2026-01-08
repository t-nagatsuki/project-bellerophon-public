using System;
using Functions.Manager;
using UnityEngine.SceneManagement;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace Functions.Script.Common
{
    [Serializable]
    public class SystemSettingsCommand : ICommand
    {
        public SystemSettingsCommand(Json.ProcessJson prc)
        {
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.DisplaySystemSettings(mng);
            return true;
        }
    }
}
