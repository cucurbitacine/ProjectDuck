using Game.DamageSystem;
using UnityEngine;

namespace Game.Combat
{
    public class DamageHealth : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private DamageReceiver damageReceiver;

        private void Initialize()
        {
            if (health == null) health = GetComponentInParent<Health>();
            if (damageReceiver == null) damageReceiver = GetComponentInParent<DamageReceiver>();
        }
        
        private void HandleDamage(Damage damage)
        {
            if (health)
            {
                health.Damage(damage.amount);
            }
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (damageReceiver)
            {
                damageReceiver.OnDamageReceived += HandleDamage;
            }
        }

        private void OnDisable()
        {
            if (damageReceiver)
            {
                damageReceiver.OnDamageReceived -= HandleDamage;
            }
        }
    }
}