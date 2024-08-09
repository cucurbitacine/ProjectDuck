using CucuTools.DamageSystem;

namespace Game.Combat
{
    public static class DamageSystemExt
    {
        public static DamageEvent SendDamage(this DamageSource source, DamageReceiver receiver)
        {
            return source.SendDamage(source.CreateDamage(receiver), receiver);
        }
    }
}