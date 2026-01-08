using System;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class CharactersJson
    {
        public CharacterJson[] character;
    }

    [Serializable]
    [YamlObject]
    public partial class CharacterJson
    {
        public string id;
        public string name;
        public string image;
        public string music;

        public CharacterStatusJson status;
        public SuitabilityJson suitability;
        public GrowthJson growth;
        public SkillJson[] skill;
    }

    [Serializable]
    [YamlObject]
    public partial class CharacterStatusJson
    {
        public int concentration;
        public int reaction;
        public int ability;
        public int perception;
        public int intention;
        public int endurance;
        public int expertise;
        public int sp;
    }

    [Serializable]
    [YamlObject]
    public partial class GrowthJson
    {
        public int concentration;
        public int reaction;
        public int ability;
        public int perception;
        public int intention;
        public int endurance;
        public int expertise;
        public int sp;
        public GrowthSuitabilityJson[] suitability;
    }

    [Serializable]
    [YamlObject]
    public partial class GrowthSuitabilityJson
    {
        public int lv;
        public string space;
        public string air;
        public string ground;
        public string underwater;
    }
}