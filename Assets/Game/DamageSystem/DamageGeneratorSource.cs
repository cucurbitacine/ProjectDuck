using UnityEngine;

namespace Game.DamageSystem
{
    public class DamageGeneratorSource : DamageSource
    {
        [Header("Damage")]
        [Min(1)] [SerializeField] private int minDamage = 1;
        [Min(1)] [SerializeField] private int maxDamage = 1;
        [Min(0f)] [SerializeField] private float criticalChance = 0f;
        [Min(0f)] [SerializeField] private float criticalFactor = 1f;
        
        public override Damage CreateDamage(DamageReceiver receiver)
        {
            return Damage.Generate(minDamage, maxDamage, criticalChance, criticalFactor);
        }
    }
}