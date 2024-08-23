using CucuTools.DamageSystem;
using Game.VFX;
using UnityEngine;

namespace Game.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HitEffect))]
    public class DamageEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        
        [Header("References")]
        [SerializeField] private Health health;

        private HitEffect _hitEffect;
        
        public void SetHealth(Health newHealth)
        {
            if (health)
            {
                health.OnDamageReceived -= HandleDamage;
            }
            
            health = newHealth;
            
            if (health)
            {
                health.OnDamageReceived += HandleDamage;
            }
        }
        
        private void HandleDamage(DamageEvent damageEvent)
        {
            if (health.IsDead) return;
            
            if (damageEvent.damage.amount > 0)
            {
                Hit(damageColor);
            }
            else if (damageEvent.damage.amount < 0)
            {
                Hit(healColor);
            }
        }

        private void Hit(Color hitColor)
        {
            _hitEffect.Hit(hitColor);
        }
        
        private void Awake()
        {
            _hitEffect = GetComponent<HitEffect>();
        }

        private void OnValidate()
        {
            if (health == null) health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health)
            {
                health.OnDamageReceived += HandleDamage;
            }
        }

        private void OnDisable()
        {
            if (health)
            {
                health.OnDamageReceived -= HandleDamage;
            }
        }
    }
}