using System;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Common
{
    [Serializable]
    public class ClearVariableCommand : ICommand
    {
        private string scope;
        private string name;

        public ClearVariableCommand(Json.ProcessJson prc)
        {
            scope = prc.scope;
            name = prc.name;
        }

        public bool Process(SlgSceneManager mng)
        {
            if (string.IsNullOrWhiteSpace(scope) && string.IsNullOrWhiteSpace(name))
            { mng.ScriptManager.Variable.Clear(); }
            return false;
        }
    }
}

