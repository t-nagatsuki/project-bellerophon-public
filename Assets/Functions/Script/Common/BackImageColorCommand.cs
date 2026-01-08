using System;
using Functions.Manager;
using Functions.Util;
using UnityEngine;

namespace Functions.Script.Common
{
    [Serializable]
    public class BackImageColorCommand : ICommand
    {
        private Color32 color;
        private float fade;
        
        public BackImageColorCommand(Json.ProcessJson prc)
        {
            color = prc.color;
            fade = prc.fade;
            if (fade < 0) throw new Exception(LocaleUtil.GetMessage("E_S0006", "fade", "0"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetBackImageColor(color, fade);
            return true;
        }
    }
}
