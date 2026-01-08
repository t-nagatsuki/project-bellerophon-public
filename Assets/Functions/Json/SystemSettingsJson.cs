using System;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class SystemSettingsJson
    {
        public int window_mode;
        public int window_width;
        public int window_height;
        public float bgm_volume;
        public float sound_volume;
    }
}