using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;

namespace Functions.Script.Common
{
    [Serializable]
    public class WaitCommand : ICommand
    {
        private float wait;

        public WaitCommand(Json.ProcessJson prc)
        {
            wait = prc.time;
            if (wait < 0) throw new Exception(LocaleUtil.GetMessage("E_S0006", "time", "0"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SetWait(wait);
            return true;
        }
    }
}

