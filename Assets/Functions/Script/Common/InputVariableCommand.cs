using System;
using Functions.Data;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Common
{
    [Serializable]
    public class InputVariableCommand : ICommand
    {
        private string scope;
        private string name;
        private string[] text;
        private int minNum;
        private int maxNum;

        public InputVariableCommand(Json.ProcessJson prc)
        {
            if (!String.IsNullOrWhiteSpace(prc.scope))
            { scope = prc.scope; }
            else
            { scope = "default"; }
            name = prc.name;
            text = prc.text;
            if (text == null) text = Array.Empty<string>();
            minNum = prc.min_num;
            maxNum = prc.max_num;
            if (String.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
            if (minNum < 0) throw new Exception(LocaleUtil.GetMessage("E_S0004", "min_num", 0));
            if (minNum < 1) throw new Exception(LocaleUtil.GetMessage("E_S0004", "max_num", 1));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.SlgWindowManager.SetVariableInputWindow(mng, text, scope, name, minNum, maxNum);
            return false;
        }
    }
}

