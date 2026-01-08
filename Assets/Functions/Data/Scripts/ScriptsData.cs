using System;
using System.Collections.Generic;
using Functions.Script;
using UnityEngine;

namespace Functions.Data.Scripts
{
    [Serializable]
    public class ScriptsData
    {
        public string ScriptsId;
        public Dictionary<string, MethodData> Methods;

        public ScriptsData(string _id)
        {
            ScriptsId = _id;
            Methods = new Dictionary<string, MethodData>();
        }

        public MethodData GetMethod(string _name)
        {
            if (!Methods.TryGetValue(_name, out var method))
            {
                Debug.Log($"call method {_name} not defined");
                return null;
            }
            return method;
        }
    }

    [Serializable]
    public class MethodData
    {
        public string Name;
        public List<ICommand> Commands;

        public MethodData(string _name)
        {
            Name = _name;
            Commands = new List<ICommand>();
        }
    }
}