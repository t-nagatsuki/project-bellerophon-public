using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Functions.Data.Scripts
{
    [Serializable]
    public class ScriptsVariableData
    {
        [FormerlySerializedAs("ScopeId")]
        public string scopeId;
        public Dictionary<string, string> Variable;

        public ScriptsVariableData(string id)
        {
            scopeId = id;
            Variable = new Dictionary<string, string>();
        }

        public void SetVariable(string name, string val)
        {
            Variable ??= new Dictionary<string, string>();
            Variable[name] = val;
        }

        public string GetVariable(string name)
        {
            if (Variable == null || !Variable.TryGetValue(name, out var variable))
            { return null; }
            return variable;
        }
    }
}
