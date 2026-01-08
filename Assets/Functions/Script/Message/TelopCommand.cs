using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Message
{
    [Serializable]
    public class TelopCommand : ICommand
    {
        private string[] text;
        private float telopTime;
        private float fadeTime;

        public TelopCommand(Json.ProcessJson prc)
        {
            text = prc.text;
            telopTime = prc.time;
            fadeTime = prc.fade;
            if (text == null) text = Array.Empty<string>();
            if (telopTime < 0) throw new Exception(LocaleUtil.GetMessage("E_S0006", "time", "0"));
            if (fadeTime < 0) throw new Exception(LocaleUtil.GetMessage("E_S0006", "fade", "0"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetTelop(
                mng.ScriptManager.AnalysisEmbeddedvariable(text), 
                telopTime, 
                fadeTime
            );
            return true;
        }
    }
}

