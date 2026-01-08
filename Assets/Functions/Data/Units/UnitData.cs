using System;
using System.Collections.Generic;
using Functions.Data.Maps;
using Functions.Enum;
using Functions.Json;
using Mond;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Functions.Data.Maps.TileData;

namespace Functions.Data.Units
{
    [Serializable]
    public class UnitData
    {
        public string UnitId;
        public string UnitName;
        public bool Machine;
        public Texture2D UnitImage;
        public string UnitImagePath;
        public TileBase[] UnitBaseTile;
        public TileBase[] UnitFrontTile;

        public SuitableData Space;
        public SuitableData Air;
        public SuitableData Ground;
        public SuitableData Underwater;

        public StatusValueData Accuracy;
        public StatusValueData Maneuver;
        public StatusValueData Power;
        public StatusValueData Armor;
        public StatusValueData Reduction;
        public StatusValueData Move;
        public StatusValueData HP;
        public StatusValueData EN;

        public List<SkillData> Skills;
        public List<WeaponData> Weapons;

        public UnitData(string id)
        {
            UnitId = id;
            UnitName = id;
            Space = new SuitableData("宇宙", nameof(Suitable.E));
            Air = new SuitableData("空中", nameof(Suitable.E));
            Ground = new SuitableData("地上", nameof(Suitable.E));
            Underwater = new SuitableData("水中", nameof(Suitable.E));
            
            Accuracy = new StatusValueData("accuracy", "精度", 0);
            Maneuver = new StatusValueData("maneuver", "機動", 0);
            Power = new StatusValueData("power", "出力", 0);
            Armor = new StatusValueData("armor", "装甲", 0);
            Reduction = new StatusValueData("reduction", "軽減", 0);
            Move = new StatusValueData("move", "移動", 0);
            HP = new StatusValueData("hp", "HP", 0);
            EN = new StatusValueData("en", "EN", 0);
        }
        
        public UnitData(UnitJson json)
        {
            Skills = new List<SkillData>();
            Weapons = new List<WeaponData>();
            UnitId = json.id;
            UnitName = json.name;
            UnitImagePath = json.display;
            Machine = json.machine;

            Space = new SuitableData("宇宙", json.suitability?.space);
            Air = new SuitableData("空中", json.suitability?.air);
            Ground = new SuitableData("地上", json.suitability?.ground);
            Underwater = new SuitableData("水中", json.suitability?.underwater);

            Accuracy = new StatusValueData("accuracy", "精度", json.status.accuracy);
            Maneuver = new StatusValueData("maneuver", "機動", json.status.maneuver);
            Power = new StatusValueData("power", "出力", json.status.power);
            Armor = new StatusValueData("armor", "装甲", json.status.armor);
            Reduction = new StatusValueData("reduction", "軽減", json.status.reduction);
            Move = new StatusValueData("move", "移動", json.status.move);
            HP = new StatusValueData("hp", "HP", json.status.hp);
            EN = new StatusValueData("en", "EN", json.status.en);

            foreach (var skill in json.skill ?? Array.Empty<SkillJson>())
            {
                if (string.IsNullOrWhiteSpace(skill.name))
                { continue; }
                Skills.Add(new SkillData(skill));
            }

            foreach (var wep in json.weapon ?? Array.Empty<WeaponJson>())
            {
                Weapons.Add(new WeaponData(wep));
            }
        }

        public MondValue GetMondValue()
        {
            var obj = MondValue.Object();
            obj["space"] = (int)Space.suitable;
            obj["air"] = (int)Air.suitable;
            obj["ground"] = (int)Ground.suitable;
            obj["underwater"] = (int)Underwater.suitable;

            obj["accuracy"] = Accuracy.status;
            obj["maneuver"] = Maneuver.status;
            obj["power"] = Power.status;
            obj["armor"] = Armor.status;
            obj["reduction"] = Reduction.status;
            obj["move"] = Move.status;
            
            // TODO : スキルデータの考慮
            
            return obj;
        }

        public TileBase GetBaseTile(DirectionType dir)
        {
            var d = (int)dir;
            if (UnitBaseTile.Length <= d)
            { return UnitBaseTile[0]; }
            return UnitBaseTile[d];
        }

        public TileBase GetFrontTile(DirectionType dir)
        {
            var d = (int)dir;
            if (UnitFrontTile.Length == 0)
            { return null; }
            if (UnitFrontTile.Length <= d)
            { return UnitFrontTile[0]; }
            return UnitFrontTile[d];
        }

        public MoveType GetMostOptimalMoveType(TileData tile)
        {
            var suitable = Space.suitable;
            var result = MoveType.Space;
            if (Air.suitable > Suitable.E && tile.MoveCost.GetValueOrDefault(MoveType.Air, new MoveCostData(MoveType.Air, -1)).MoveCost > 0 && suitable < Air.suitable)
            {
                suitable = Air.suitable;
                result = MoveType.Air;
            }
            if (Ground.suitable > Suitable.E && tile.MoveCost.GetValueOrDefault(MoveType.Ground, new MoveCostData(MoveType.Ground, -1)).MoveCost > 0 && suitable < Ground.suitable)
            {
                suitable = Ground.suitable;
                result = MoveType.Ground;
            }
            if (Underwater.suitable > Suitable.E && tile.MoveCost.GetValueOrDefault(MoveType.Underwater, new MoveCostData(MoveType.Underwater, -1)).MoveCost > 0 && suitable < Underwater.suitable)
            {
                result = MoveType.Underwater;
            }
            return result;
        }
    }
}
