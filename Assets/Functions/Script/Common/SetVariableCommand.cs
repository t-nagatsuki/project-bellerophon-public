using System;
using Functions.Data;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Common
{
    [Serializable]
    public class SetVariableCommand : ICommand
    {
        private string scope;
        private string name;
        private string value;

        public SetVariableCommand(Json.ProcessJson prc)
        {
            if (!String.IsNullOrWhiteSpace(prc.scope))
            { scope = prc.scope; }
            else
            { scope = "default"; }
            name = prc.name;
            value = prc.value;
            if (String.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
        }

        public bool Process(SlgSceneManager mng)
        {
            mng.ScriptManager.SetVariable(scope, name, value);
            return false;
        }
    }
}

