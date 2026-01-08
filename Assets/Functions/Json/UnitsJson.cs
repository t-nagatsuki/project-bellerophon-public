using System;
using VYaml.Annotations;
namespace Functions.Json
{
    [Serializable]
    [YamlObject]
    public partial class UnitsJson
    {
        public UnitJson[] unit;
    }

    [Serializable]
    [YamlObject]
    public partial class UnitJson
    {
        public string id;
        public string name;
        public bool machine;
        public string display;
        public string base_resource;
        public string front_resource;
        public int pixels;
        public TileChipLayerJson[] base_image;
        public TileChipLayerJson[] front_image;
        public SuitabilityJson suitability;
        public UnitStatusJson status;
        public SkillJson[] skill;
        public WeaponJson[] weapon;
    }

    [Serializable]
    [YamlObject]
    public partial class UnitStatusJson
    {
        public int accuracy;
        public int maneuver;
        public int power;
        public int armor;
        public int reduction;
        public int move;
        public int hp;
        public int en;
    }


    [Serializable]
    [YamlObject]
    public partial class SuitabilityJson
    {
        public string space;
        public string air;
        public string ground;
        public string underwater;
    }

    [Serializable]
    [YamlObject]
    public partial class WeaponJson
    {
        public string name;
        public string attack_type;
        public RangeJson range;
        public SuitabilityJson suitability;
        public int accuracy;
        public int critical;
        public int atk;
        public int bullets;
        public int energy;
        public SkillJson[] skill;
    }

    [Serializable]
    [YamlObject]
    public partial class RangeJson
    {
        public int min;
        public int max;
    }
}