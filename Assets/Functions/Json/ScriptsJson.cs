using System;
using UnityEngine;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class ScriptsJson
    {
        public string id;
        public MethodJson[] methods;
    }

    [Serializable]
    [YamlObject]
    public partial class MethodJson
    {
        public string name;
        public ProcessJson[] process;
    }

    [Serializable]
    [YamlObject]
    public partial class ProcessJson
    {
        public string cmd;
        public string name;
        public string method;

        public Vector3Int pos;
        public Vector3Int area_size;
        public int direction;
        public Color32 color;
        public string scope;
        public string value;
        public string[] values;
        public float time;
        public float fade;
        public bool loop;

        public string event_id;
        public int group_id;
        public string unit_id;
        public string chara_id;
        public string unit;
        public string chara;
        public int lv;
        public string unit_name;
        public string chara_name;
        public bool player;
        public int[] friendly;
        public bool non_arrangement;
        public bool animation;
        public bool permanent;
        public bool hidden;
        public string[] statuses;
        public string[] text;
        public ConditionalJson positive;
        public ConditionalJson negative;
        public string op;
        public string position;
        public string music;
        public string layer;
        public string move_type;
        public string action_type;
        public string condition_type;
        public string objective_type;
        public Vector2Int target_position;
        public int min_num;
        public int max_num;
        public ChoiceJson[] choices;
    }

    [Serializable]
    [YamlObject]
    public partial class ConditionalJson
    {
        public string name;
        public string method;
    }

    [Serializable]
    [YamlObject]
    public partial class ChoiceJson
    {
        public string[] text;
        public string name;
        public string method;
    }
}