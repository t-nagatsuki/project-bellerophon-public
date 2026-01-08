using System;
using UnityEngine;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class SkillJson
    {
        public string name;
        public string rename;
        public int lv;
        public bool display;
        public string description;
    }
}