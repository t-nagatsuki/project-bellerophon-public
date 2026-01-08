using System;
using VYaml.Annotations;

namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class CopyrightJson
    {
        public CopyrightBodyJson[] copyright;
    }

    [Serializable]
    [YamlObject]
    public partial class CopyrightBodyJson
    {
        public string title;
        public CopyrightTextJson[] text;
    }

    [Serializable]
    [YamlObject]
    public partial class CopyrightTextJson
    {
        public string title;
        public string text;
        public string link;
    }
}