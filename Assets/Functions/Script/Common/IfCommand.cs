using System;
using Functions.Json;
using Functions.Manager;
using Functions.Util;

namespace Functions.Script.Common
{
    [Serializable]
    public class IfCommand : ICommand
    {
        private string scope;
        private string name;
        private string op;
        private string value;
        private ConditionalJson positive;
        private ConditionalJson negative;

        public IfCommand(Json.ProcessJson prc)
        {
            if (!String.IsNullOrWhiteSpace(prc.scope))
            { scope = prc.scope; }
            else
            { scope = "default"; }
            name = prc.name;
            if (!String.IsNullOrWhiteSpace(prc.op))
            { op = prc.op; }
            else
            { op = "="; }
            value = prc.value;
            positive = prc.positive;
            negative = prc.negative;
            if (String.IsNullOrWhiteSpace(name)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "name"));
            if (positive != null && String.IsNullOrWhiteSpace(positive.method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "positive.method"));
            if (negative != null && String.IsNullOrWhiteSpace(negative.method)) throw new Exception(LocaleUtil.GetMessage("E_S0001", "negative.method"));
        }

        public bool Process(SlgSceneManager mng)
        {
            if (mng.ScriptManager.Variable.ContainsKey(scope) && mng.ScriptManager.Variable[scope].GetVariable(name) == value)
            {
                if (positive != null)
                {
                    return mng.ScriptManager.CallMethod(positive.name, positive.method);
                }
            }
            else
            {
                if (negative != null)
                {
                    return mng.ScriptManager.CallMethod(negative.name, negative.method);
                }
            }
            return false;
        }
    }
}

