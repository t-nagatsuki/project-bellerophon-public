using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine.SceneManagement;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace Functions.Script.Common
{
    [Serializable]
    public class IntermissionCommand : ICommand
    {
        private string name;
        private string method;
        
        public IntermissionCommand(Json.ProcessJson prc)
        {
            name = prc.name;
            method = prc.method;
            if (String.IsNullOrWhiteSpace(method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "method"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.DisplayIntermission(mng, name, method);
            return true;
        }
    }
}
