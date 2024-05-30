using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.DamageSystem
{
    [Serializable]
    public class Damage
    {
        public DamageSource source;
        public DamageReceiver receiver;
        
        [Space]
        public int amount;
        public bool critical;

        public static Damage Generate(int minDamage = 1, int maxDamage = 1, float criticalChance = 0f, float criticalFactor = 1f)
        {
            minDamage = Mathf.Max(1, minDamage);
            maxDamage = Mathf.Max(minDamage, maxDamage);
            criticalChance = Mathf.Clamp01(criticalChance);
            criticalFactor = Mathf.Max(0f, criticalFactor);
            
            var randomValue = Random.value;
            
            var damage = new Damage
            {
                amount = Random.Range(minDamage, maxDamage + 1),
                critical = 0f < randomValue && randomValue <= criticalChance
            };
            
            if (damage.critical)
            {
                damage.amount += (int)(damage.amount * criticalFactor);
            }

            return damage;
        }
        

        public override string ToString()
        {
            var damageName = $"{amount}{(critical?" CRITICAL":"")}";
            var receiverName = receiver ? receiver.name : "NO RECEIVER";
            var sourceName = source ? source.name : "NO SOURCE";
            
            return $"{damageName} [{receiverName}] <- [{sourceName}]";
        }
    }
}