using Functions.Data.Units;
using Functions.Enum;

namespace Functions.Data.Battle
{
    public class BattleInfoData
    {
        public ArrangementData Attacker;
        public WeaponData AttackWeapon;
        public float Hit;
        public float Critical;
        public float AttackPower;
        public float DefenceRate;
        public ArrangementData Defender;
        public ReactionMode Reaction;
        public WeaponData CounterWeapon;
        public float CounterHit;
        public float CounterCritical;
        public float CounterPower;
        public float CounterDefenceRate;
    }
}
