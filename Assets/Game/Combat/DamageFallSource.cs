using Game.DamageSystem;
using UnityEngine;

namespace Game.Combat
{
    public class DamageFallSource : DamageSource
    {
        [Header("Fall Damage")]
        [Min(0)] [SerializeField] private float speedMax = 15f;
        [Min(0)] [SerializeField] private int damageAmount = 1;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            //Debug.Log($"Hit on {other.collider.name} with {other.relativeVelocity.magnitude:F2} speed");
            
            if (speedMax > 0f && other.relativeVelocity.magnitude > speedMax)
            {
                if (TryGetComponent<DamageReceiver>(out var receiver))
                {
                    var damage = CreateDamage(receiver);

                    damage.amount = (int)Mathf.LerpUnclamped(0, damageAmount, other.relativeVelocity.magnitude / speedMax);

                    SendDamage(receiver, damage);
                }
            }
        }

        public override Damage CreateDamage(DamageReceiver receiver)
        {
            return new Damage()
            {
                amount = damageAmount,
            };
        }
    }
}