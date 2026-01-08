using System;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class CommonSettingsJson
    {
        public bool debug_mode;
        public bool tool_mode;
        public TileJson cursor;
        public string[] map_cursor;
        public string[] highlight_area;
        public string[] arrangement_area;
        public string[] move_area;
        public string[] attack_area;
        public string group_marker;
        public string self_marker;
        public string target_marker;
        public SystemMenuJson[] system_menu;
    }

    [Serializable]
    [YamlObject]
    public partial class SystemMenuJson
    {
        public string text;
        public bool bold;
        public bool italic;
        public string name;
        public string method;
        public int group;
    }
}