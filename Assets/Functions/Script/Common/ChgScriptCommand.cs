using System;
using Functions.Attributes;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Common
{
    [Serializable]
    [Command("chg_script")]
    public class ChgScriptCommand : ICommand
    {
        private string name;
        private string method;

        public ChgScriptCommand(Json.ProcessJson prc)
        {
            name = prc.name;
            method = prc.method;
            if (String.IsNullOrWhiteSpace(method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "method"));
        }

        public bool Process(SlgSceneManager mng)
        {
            return mng.ScriptManager.CallMethod(name, method);
        }
    }
}

