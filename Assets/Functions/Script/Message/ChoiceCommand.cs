using System;
using Functions.Json;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Message
{
    [Serializable]
    public class ChoiceCommand : ICommand
    {
        private string[] text;
        private ChoiceJson[] choices;

        public ChoiceCommand(Json.ProcessJson prc)
        {
            text = prc.text;
            choices = prc.choices;
            if (text == null) text = Array.Empty<string>();
            if (choices == null) throw new Exception(LocaleUtil.GetMessage("E_S0003", "choices"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetChoiceWindow(
                mng, 
                mng.ScriptManager.AnalysisEmbeddedvariable(text), 
                choices
            );
            return true;
        }
    }
}

