using System;
using Functions.Attributes;

namespace Functions.Skills
{
    [Serializable]
    [Skill("flavor")]
    public class FlavorSkill : ISkill
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}