using Functions.Data.Units;
using Functions.Enum;

namespace Functions.Data.Battle
{
    public class BattleResultData
    {
        public ArrangementData Attacker;
        public WeaponData AttackWeapon;
        public bool Hit;
        public int Damage;
        public bool Critical;
        public bool Destroy;
        public ArrangementData Defender;
        public ReactionMode Reaction;
        public WeaponData CounterWeapon;
        public bool CounterHit;
        public int CounterDamage;
        public bool CounterCritical;
        public bool CounterDestroy;
    }
}
