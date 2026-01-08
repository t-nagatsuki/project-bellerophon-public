using System;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class MessagesJson
    {
        public MessageJson[] messages;
    }

    [Serializable]
    [YamlObject]
    public partial class MessageJson
    {
        public string id;
        public SituationJson[] attack;
        public SituationJson[] guard;
        public SituationJson[] avoid;
        public SituationJson[] damage;
        public SituationJson[] destroy;
    }

    [Serializable]
    [YamlObject]
    public partial class SituationJson
    {
        public string[] message;
        public int hit_under;
        public int hit_over;
        public int hp_under;
        public int hp_over;
    }
}