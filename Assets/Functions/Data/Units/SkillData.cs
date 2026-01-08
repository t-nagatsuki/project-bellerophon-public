using System;
using Functions.Json;
using Functions.Skills;
using Functions.Util;

namespace Functions.Data.Units
{
    [Serializable]
    public class SkillData
    {
        public ISkill Skill { get; set; }
        public string Rename { get; set; }
        public int Lv { get; set; }
        public bool Display { get; set; }
        public string Rewrite { get; set; }

        public SkillData(SkillJson json)
        {
            Skill = DataUtil.GetSkill(json.name);
            Rename = json.rename;
            Lv = json.lv;
            Display = json.display;
            Rewrite = json.description;
        }

        public string Name => string.IsNullOrWhiteSpace(Rename) ? Skill.Name : Rename;
        public string Description => string.IsNullOrWhiteSpace(Rewrite) ? Skill.Description : Rewrite;
    }
}
