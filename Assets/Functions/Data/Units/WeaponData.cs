using System;
using System.Collections.Generic;
using Functions.Enum;
using Functions.Json;
using Mond;

namespace Functions.Data.Units
{
    [Serializable]
    public class WeaponData
    {
        public string WeaponName;
        public AttackType AttackType;
        public int RangeMin;
        public int RangeMax;
        public SuitableData Space;
        public SuitableData Air;
        public SuitableData Ground;
        public SuitableData Underwater;
        public int Accuracy;
        public int Critical;
        public int AttackPower;
        public int Bullets;
        public int Energy;
        
        public List<SkillData> Skills;

        public WeaponData(Json.WeaponJson _json)
        {
            Skills = new List<SkillData>();
            WeaponName = _json.name;

            if (!String.IsNullOrWhiteSpace(_json.attack_type))
            {
                switch (_json.attack_type.ToLower())
                {
                    case "short":
                        AttackType = AttackType.Short;
                        break;
                    case "long":
                        AttackType = AttackType.Long;
                        break;
                    case "combination":
                        AttackType = AttackType.Combination;
                        break;
                }
            }
            else
            { AttackType = AttackType.Combination; }

            RangeMin = _json.range.min;
            RangeMax = _json.range.max;

            Space = new SuitableData("宇宙", _json.suitability?.space);
            Air = new SuitableData("空中", _json.suitability?.air);
            Ground = new SuitableData("地上", _json.suitability?.ground);
            Underwater = new SuitableData("水中", _json.suitability?.underwater);

            Accuracy = _json.accuracy;
            Critical = _json.critical;
            AttackPower = _json.atk;
            Bullets = _json.bullets;
            Energy = _json.energy;
            
            foreach (var skill in _json.skill ?? Array.Empty<SkillJson>())
            {
                if (string.IsNullOrWhiteSpace(skill.name))
                { continue; }
                Skills.Add(new SkillData(skill));
            }
        }

        public MondValue GetMondValue()
        {
            var obj = MondValue.Object();
            obj["attack_type"] = (int)AttackType;
            obj["range_min"] = RangeMin;
            obj["range_max"] = RangeMax;
            obj["space"] = (int)Space.suitable;
            obj["air"] = (int)Air.suitable;
            obj["ground"] = (int)Ground.suitable;
            obj["underwater"] = (int)Underwater.suitable;
            obj["accuracy"] = Accuracy;
            obj["critical"] = Critical;
            obj["attack_power"] = AttackPower;
            obj["bullets"] = Bullets;
            obj["energy"] = Energy;
            
            // TODO : スキルデータの考慮
            
            return obj;
        }

        public bool CheckCanAttack(MoveType move)
        {
            switch (move)
            {
                case MoveType.Space:
                    return Space.suitable != Suitable.E;
                case MoveType.Air:
                    return Air.suitable != Suitable.E;
                case MoveType.Ground:
                    return Ground.suitable != Suitable.E;
                case MoveType.Underwater:
                    return Underwater.suitable != Suitable.E;
            }
            return false;
        }
    }
}
