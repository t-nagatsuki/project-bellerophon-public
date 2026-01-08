using Cysharp.Threading.Tasks;
using Functions.Data;
using Functions.Data.Battle;
using Functions.Data.Maps;
using Functions.Data.Units;
using Functions.Enum;
using Unity.Mathematics;
using UnityEngine;

namespace Functions.Util
{
    public static class BattleUtil
    {
        public static async UniTask<BattleInfoData> CalcBattleInfo(
            MondUtil utilMond,
            BattleInfoData info, 
            ProcessMode mode, 
            ArrangementData attacker,
            ArrangementData defender,
            PermanenceCharacterData atkChara, 
            PermanenceUnitData atkUnit, 
            PermanenceCharacterData defChara, 
            PermanenceUnitData defUnit, 
            TileData atkTile, 
            TileData defTile)
        {
            // 攻撃側の計算
            var attackStats = await CalculateCombatStats(
                utilMond,
                attacker, defender,
                atkChara, atkUnit, atkTile,
                defChara, defUnit, defTile,
                info.AttackWeapon,
                info.Reaction);

            info.Hit = math.clamp(attackStats.Hit - attackStats.Avoid, 5, 100);
            info.Critical = math.clamp(attackStats.Critical, 1, 100);
            info.AttackPower = attackStats.Power;
            info.DefenceRate = attackStats.Defence;
            Debug.Log($"hit : {info.Hit}, avoid : {attackStats.Avoid}, critical : {info.Critical}, power : {info.AttackPower}, defence : {info.DefenceRate}");

            if (mode == ProcessMode.SelectReaction || info.Reaction != ReactionMode.Counter)
            { return info; }
            
            // 反撃側の計算
            var counterStats = await CalculateCombatStats(
                utilMond,
                defender, attacker,
                defChara, defUnit, defTile,
                atkChara, atkUnit, atkTile,
                info.CounterWeapon,
                ReactionMode.Counter);
            
            info.CounterHit = math.clamp(counterStats.Hit - counterStats.Avoid, 5, 100);
            info.CounterCritical = math.clamp(counterStats.Critical, 1, 100);
            info.CounterPower = counterStats.Power;
            info.CounterDefenceRate = counterStats.Defence;
            Debug.Log($"counter_hit : {info.CounterHit}, counter_avoid : {counterStats.Avoid}, counter_critical : {info.CounterCritical}, counter_power : {info.CounterPower}, counter_defence : {info.CounterDefenceRate}");

            return info;
        }
        
        private static async UniTask<CombatStatusData> CalculateCombatStats(
            MondUtil utilMond,
            ArrangementData attacker,
            ArrangementData defender,
            PermanenceCharacterData atkChara,
            PermanenceUnitData atkUnit,
            TileData atkTile,
            PermanenceCharacterData defChara,
            PermanenceUnitData defUnit,
            TileData defTile,
            WeaponData weapon,
            ReactionMode reaction)
        {
            var hitBonus = GetBonus(attacker.MoveType, atkUnit, atkChara);
            var wepHitBonus = GetWeaponBonus(defender.MoveType, weapon);
            var avoidBonus = GetBonus(defender.MoveType, defUnit, defChara);

            utilMond.SetState("attack_chara", atkChara.GetMondValue());
            utilMond.SetState("attack_unit", atkUnit.GetMondValue());
            utilMond.SetState("attack_weapon", weapon.GetMondValue());
            utilMond.SetState("defend_chara", defChara.GetMondValue());
            utilMond.SetState("defend_unit", defUnit.GetMondValue());
            utilMond.SetState("unit_hit_bonus", hitBonus);
            utilMond.SetState("weapon_hit_bonus", wepHitBonus);
            utilMond.SetState("avoid_bonus", avoidBonus);
            utilMond.SetState("reaction", (int)reaction);

            var hit = (int)await utilMond.LoadScript("calc_hit");
            utilMond.SetState("calc_hit_result", hit);
    
            var avoid = (int)await utilMond.LoadScript("calc_avoid");
            utilMond.SetState("calc_avoid_result", avoid);
    
            var crt = (int)await utilMond.LoadScript("calc_critical");
            utilMond.SetState("calc_critical_result", crt);
    
            var pow = (int)await utilMond.LoadScript("calc_power");
            var defence = (int)await utilMond.LoadScript("calc_defence");

            return new CombatStatusData
            {
                Hit = hit,
                Avoid = avoid,
                Critical = crt,
                Power = pow,
                Defence = defence
            };
        }

        private static int GetBonus(MoveType move, PermanenceUnitData unit, PermanenceCharacterData chara)
        {
            switch (move)
            {
                case MoveType.Space:
                    return 5 * ((int)unit.Unit.Space.suitable - 2) + 5 * ((int)chara.Space.suitable - 2);
                case MoveType.Air:
                    return 5 * ((int)unit.Unit.Air.suitable - 2) + 5 * ((int)chara.Air.suitable - 2);
                case MoveType.Ground:
                    return 5 * ((int)unit.Unit.Ground.suitable - 2) + 5 * ((int)chara.Ground.suitable - 2);
                case MoveType.Underwater:
                    return 5 * ((int)unit.Unit.Underwater.suitable - 2) + 5 * ((int)chara.Underwater.suitable - 2);
            }
            return 0;
        }

        private static int GetWeaponBonus(MoveType move, WeaponData weapon)
        {
            switch (move)
            {
                case MoveType.Space:
                    return 5 * ((int)weapon.Space.suitable - 2);
                case MoveType.Air:
                    return 5 * ((int)weapon.Air.suitable - 2);
                case MoveType.Ground:
                    return 5 * ((int)weapon.Ground.suitable - 2);
                case MoveType.Underwater:
                    return 5 * ((int)weapon.Underwater.suitable - 2);
            }
            return 0;
        }
    }
}