using System;
using System.Collections.Generic;
using Functions.Util;
using Mond;

namespace Functions.Data.Units
{
    [Serializable]
    public class PermanenceCharacterData
    {
        public string CharacterId;
        public CharacterData Character;
        public string CharacterName;
        public bool Hidden;
        public Dictionary<string, ResourceValueData> Statuses;
        public SuitableData Space;
        public SuitableData Air;
        public SuitableData Ground;
        public SuitableData Underwater;

        public PermanenceCharacterData(string _cid, int _lv, CharacterData _chara, string _chara_name, bool _hidden)
        {
            CharacterId = _cid;
            Character = _chara;
            if (!String.IsNullOrWhiteSpace(_chara_name))
            { CharacterName = _chara_name; }
            else
            { CharacterName = _chara.CharacterName; }

            Statuses = new Dictionary<string, ResourceValueData>();
            Statuses["lv"] = new ResourceValueData("lv", "LV", _lv);
            Statuses["exp"] = new ResourceValueData("exp", "EXP", _lv * 100);
            Statuses["exp"].Now = 0;
            Statuses["concentration"] = new ResourceValueData(_chara.Concentration.statusName, _chara.Concentration.displayName, _chara.Concentration.status + _chara.GrowthConcentration * (_lv - 1), false);
            Statuses["reaction"] = new ResourceValueData(_chara.Reaction.statusName, _chara.Reaction.displayName, _chara.Reaction.status + _chara.GrowthReaction * (_lv - 1), false);
            Statuses["ability"] = new ResourceValueData(_chara.Ability.statusName, _chara.Ability.displayName, _chara.Ability.status + _chara.GrowthAbility * (_lv - 1), false);
            Statuses["perception"] = new ResourceValueData(_chara.Perception.statusName, _chara.Perception.displayName, _chara.Perception.status + _chara.GrowthPerception * (_lv - 1), false);
            Statuses["intention"] = new ResourceValueData(_chara.Intention.statusName, _chara.Intention.displayName, _chara.Intention.status + _chara.GrowthIntention * (_lv - 1), false);
            Statuses["endurance"] = new ResourceValueData(_chara.Endurance.statusName, _chara.Endurance.displayName, _chara.Endurance.status + _chara.GrowthEndurance * (_lv - 1), false);
            Statuses["expertise"] = new ResourceValueData(_chara.Expertise.statusName, _chara.Expertise.displayName, _chara.Expertise.status + _chara.GrowthExpertise * (_lv - 1), false);
            Statuses["sp"] = new ResourceValueData(_chara.SP.statusName, _chara.SP.displayName, _chara.SP.status + _chara.GrowthSP * (_lv - 1));

            Space = new SuitableData(_chara.Space, _lv);
            Air = new SuitableData(_chara.Air, _lv);
            Ground = new SuitableData(_chara.Ground, _lv);
            Underwater = new SuitableData(_chara.Underwater, _lv);

            Hidden = _hidden;
        }

        public PermanenceCharacterData(PermanenceCharacterSaveData _dat)
        {
            CharacterId = _dat.CharacterId;
            CharacterName = _dat.CharacterName;
            Statuses = DataUtil.DeserializeDictionary(_dat.Statuses);
            Space = _dat.Space;
            Air = _dat.Air;
            Ground = _dat.Ground;
            Underwater = _dat.Underwater;
        }

        public void SetStatus(string _status, string _op, string _value)
        {
            if (!Statuses.ContainsKey(_status.ToLower()))
            { return; }
            var status = Statuses[_status.ToLower()];
            status.Calc(_op, _value);
            switch (_status.ToLower())
            {
                case "exp":
                    if (status.Now == status.Max)
                    { LevelUp(); }
                    break;
                case "lv":
                    Statuses["exp"].Max = status.Now * 100;
                    Statuses["exp"].Now = 0;
                    break;
            }
        }

        public void LevelUp()
        {
            Statuses["lv"].Calc("+", 1);
            Statuses["exp"].Now = 0;
            Statuses["exp"].Max = Statuses["lv"].Now * 100;
            Statuses["concentration"].Calc("+", Character.GrowthConcentration);
            Statuses["reaction"].Calc("+", Character.GrowthReaction);
            Statuses["ability"].Calc("+", Character.GrowthAbility);
            Statuses["perception"].Calc("+", Character.GrowthPerception);
            Statuses["intention"].Calc("+", Character.GrowthIntention);
            Statuses["endurance"].Calc("+", Character.GrowthEndurance);
            Statuses["expertise"].Calc("+", Character.GrowthExpertise);
            Statuses["sp"].Max = Character.GrowthConcentration;
            Statuses["sp"].Calc("+", Character.GrowthConcentration);
            Space.LevelUp(Statuses["lv"].Now);
            Air.LevelUp(Statuses["lv"].Now);
            Ground.LevelUp(Statuses["lv"].Now);
            Underwater.LevelUp(Statuses["lv"].Now);
        }

        public MondValue GetMondValue()
        {
            var obj = MondValue.Object();
            foreach (var status in Statuses)
            {
                obj[status.Key] = status.Value.Now;
            }
            obj["space"] = (int)Space.suitable;
            obj["air"] = (int)Air.suitable;
            obj["ground"] = (int)Ground.suitable;
            obj["underwater"] = (int)Underwater.suitable;
            
            // TODO : スキルデータの考慮
            
            return obj;
        }

        public ResourceValueData SP => Statuses["sp"];

        public ResourceValueData LV => Statuses["lv"];

        public ResourceValueData EXP => Statuses["exp"];

        public ResourceValueData Concentration => Statuses["concentration"];

        public ResourceValueData Reaction => Statuses["reaction"];

        public ResourceValueData Ability => Statuses["ability"];

        public ResourceValueData Perception => Statuses["perception"];

        public ResourceValueData Intention => Statuses["intention"];

        public ResourceValueData Endurance => Statuses["endurance"];

        public ResourceValueData Expertise => Statuses["expertise"];
    }
}
