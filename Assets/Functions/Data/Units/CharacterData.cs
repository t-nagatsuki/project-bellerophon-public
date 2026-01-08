using System;
using System.Collections.Generic;
using Functions.Enum;
using Functions.Json;
using UnityEngine;

namespace Functions.Data.Units
{
    [Serializable]
    public class CharacterData
    {
        public string CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string CharacterImagePath { get; set; }
        public Texture2D CharacterImage { get; set; }
        public string Music { get; set; }

        public SuitableData Space { get; set; }
        public SuitableData Air { get; set; }
        public SuitableData Ground { get; set; }
        public SuitableData Underwater { get; set; }

        public StatusValueData Concentration { get; set; }
        public StatusValueData Reaction { get; set; }
        public StatusValueData Ability { get; set; }
        public StatusValueData Perception { get; set; }
        public StatusValueData Intention { get; set; }
        public StatusValueData Endurance { get; set; }
        public StatusValueData Expertise { get; set; }
        public StatusValueData SP { get; set; }
        
        public int GrowthConcentration { get; set; }
        public int GrowthReaction { get; set; }
        public int GrowthAbility { get; set; }
        public int GrowthPerception { get; set; }
        public int GrowthIntention { get; set; }
        public int GrowthEndurance { get; set; }
        public int GrowthExpertise { get; set; }
        public int GrowthSP { get; set; }

        public List<SkillData> Skills { get; set; } = new();

        public CharacterData(string id)
        {
            CharacterId = id;
            CharacterName = id;
            Space = new SuitableData("宇宙", nameof(Suitable.E));
            Air = new SuitableData("空中", nameof(Suitable.E));
            Ground = new SuitableData("地上", nameof(Suitable.E));
            Underwater = new SuitableData("水中", nameof(Suitable.E));

            Concentration = new StatusValueData("concentration", "集中", 0);
            Reaction = new StatusValueData("reaction", "反応", 0);
            Ability = new StatusValueData("ability", "技量", 0);
            Perception = new StatusValueData("perception", "知覚", 0);
            Intention = new StatusValueData("intention", "意思", 0);
            Endurance = new StatusValueData("endurance", "耐久", 0);
            Expertise = new StatusValueData("expertise", "熟練", 0);
            SP = new StatusValueData("sp", "SP", 0);
        }

        public CharacterData(Json.CharacterJson json)
        {
            CharacterId = json.id;
            CharacterName = json.name;
            CharacterImagePath = json.image;
            Music = json.music;

            Space = new SuitableData("宇宙", json.suitability?.space);
            Air = new SuitableData("空中", json.suitability?.air);
            Ground = new SuitableData("地上", json.suitability?.ground);
            Underwater = new SuitableData("水中", json.suitability?.underwater);

            Concentration = new StatusValueData("concentration", "集中", json.status.concentration);
            Reaction = new StatusValueData("reaction", "反応", json.status.reaction);
            Ability = new StatusValueData("ability", "技量", json.status.ability);
            Perception = new StatusValueData("perception", "知覚", json.status.perception);
            Intention = new StatusValueData("intention", "意思", json.status.intention);
            Endurance = new StatusValueData("endurance", "耐久", json.status.endurance);
            Expertise = new StatusValueData("expertise", "熟練", json.status.expertise);
            SP = new StatusValueData("sp", "SP", json.status.sp);

            if (json.growth != null)
            {
                GrowthConcentration = json.growth.concentration;
                GrowthReaction = json.growth.reaction;
                GrowthAbility = json.growth.ability;
                GrowthPerception = json.growth.perception;
                GrowthIntention = json.growth.intention;
                GrowthEndurance = json.growth.endurance;
                GrowthExpertise = json.growth.expertise;
                GrowthSP = json.growth.sp;
                if (json.growth.suitability != null)
                {
                    foreach (var suitability in json.growth.suitability)
                    {
                        if (!string.IsNullOrWhiteSpace(suitability.space))
                        { Space.Growth[suitability.lv] = Space.ConvertSuitable(suitability.space); }
                        if (!string.IsNullOrWhiteSpace(suitability.air))
                        { Air.Growth[suitability.lv] = Air.ConvertSuitable(suitability.air); }
                        if (!string.IsNullOrWhiteSpace(suitability.ground))
                        { Ground.Growth[suitability.lv] = Ground.ConvertSuitable(suitability.ground); }
                        if (!string.IsNullOrWhiteSpace(suitability.underwater))
                        { Underwater.Growth[suitability.lv] = Underwater.ConvertSuitable(suitability.underwater); }
                    }
                }
            }

            foreach (var skill in json.skill ?? Array.Empty<SkillJson>())
            {
                if (string.IsNullOrWhiteSpace(skill.name))
                { continue; }
                Skills.Add(new SkillData(skill));
            }
        }

        public Dictionary<int, GrowthSuitabilityData> GetGrowthSuitability()
        {
            Dictionary<int, GrowthSuitabilityData> growths = new();
            foreach (var lv in Space.Growth.Keys)
            {
                if (!growths.ContainsKey(lv))
                    growths[lv] = new GrowthSuitabilityData();
                growths[lv].EnabledSpace = true;
                growths[lv].Space = Space.Growth[lv];
            }
            foreach (var lv in Air.Growth.Keys)
            {
                if (!growths.ContainsKey(lv))
                    growths[lv] = new GrowthSuitabilityData();
                growths[lv].EnabledAir = true;
                growths[lv].Air = Air.Growth[lv];
            }
            foreach (var lv in Ground.Growth.Keys)
            {
                if (!growths.ContainsKey(lv))
                    growths[lv] = new GrowthSuitabilityData();
                growths[lv].EnabledGround = true;
                growths[lv].Ground = Ground.Growth[lv];
            }
            foreach (var lv in Underwater.Growth.Keys)
            {
                if (!growths.ContainsKey(lv))
                    growths[lv] = new GrowthSuitabilityData();
                growths[lv].EnabledUnderwater = true;
                growths[lv].Underwater = Underwater.Growth[lv];
            }

            return growths;
        }
    }
}
