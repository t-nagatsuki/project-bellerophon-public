using System;
using System.Collections.Generic;
using System.Linq;
using Functions.Util;
using Mond;

namespace Functions.Data.Units
{
    [Serializable]
    public class PermanenceUnitData
    {
        public string UnitId;
        public UnitData Unit;
        public string UnitName;
        public bool Hidden;
        public Dictionary<string, ResourceValueData> Statuses;
        public Dictionary<string, ResourceValueData> Weapons;

        public PermanenceUnitData(string _uid, UnitData _unit, string _unit_name, bool _hidden)
        {
            UnitId = _uid;
            Unit = _unit;
            if (!String.IsNullOrWhiteSpace(_unit_name))
            { UnitName = _unit_name; }
            else
            { UnitName = Unit.UnitName; }

            Statuses = new Dictionary<string, ResourceValueData>();
            Statuses["hp"] = new ResourceValueData(_unit.HP.statusName, _unit.HP.displayName, _unit.HP.status);
            Statuses["en"] = new ResourceValueData(_unit.EN.statusName, _unit.EN.displayName, _unit.EN.status);

            Weapons = new Dictionary<string, ResourceValueData>();
            foreach (var dat in Unit.Weapons)
            {
                Weapons[dat.WeaponName] = new ResourceValueData(dat.WeaponName, dat.WeaponName, dat.Bullets);
            }

            Hidden = _hidden;
        }
    
        public PermanenceUnitData(PermanenceUnitSaveData _dat)
        {
            UnitId = _dat.UnitId;
            UnitName = _dat.UnitName;
            Statuses = DataUtil.DeserializeDictionary(_dat.Statuses);
            Weapons = DataUtil.DeserializeDictionary(_dat.Weapons);
        }

        public void SetStatus(string _status, string _op, string _value)
        {
            if (!Statuses.ContainsKey(_status.ToLower()))
            { return; }
            var status = Statuses[_status.ToLower()];
            status.Calc(_op, _value);
        }

        public IOrderedEnumerable<WeaponData> GetAvailableWeapons()
        {
            return Unit.Weapons.Where(
                x => Weapons[x.WeaponName].Now != 0 && x.Energy <= EN.Now
            ).OrderByDescending(x => x.AttackPower);
        }

        public IOrderedEnumerable<WeaponData> GetAvailableWeapons(int distance, ArrangementData target)
        {
            return Unit.Weapons.Where(
                x => x.RangeMin <= distance && x.RangeMax >= distance && (x.Bullets == 0 || Weapons[x.WeaponName].Now > 0) && x.Energy <= EN.Now && x.CheckCanAttack(target.MoveType)
            ).OrderByDescending(x => x.AttackPower);
        }

        public MondValue GetMondValue()
        {
            var obj = MondValue.Object();
            foreach (var status in Statuses)
            {
                obj[status.Key] = status.Value.Now;
            }

            obj["unit"] = Unit.GetMondValue();
            return obj;
        }
        
        public ResourceValueData HP => Statuses["hp"];

        public ResourceValueData EN => Statuses["en"];
    }
}
