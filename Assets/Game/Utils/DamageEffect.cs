using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(HitEffect))]
    public class DamageEffect : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        
        [Header("References")]
        [SerializeField] private DamageReceiver damageReceiver;

        private HitEffect _hitEffect;
        
        public void SetDamageReceiver(DamageReceiver newDamageReceiver)
        {
            if (damageReceiver)
            {
                damageReceiver.OnDamageReceived -= HandleDamage;
            }
            
            damageReceiver = newDamageReceiver;
            
            if (damageReceiver)
            {
                damageReceiver.OnDamageReceived += HandleDamage;
            }
        }
        
        private void HandleDamage(DamageEvent damageEvent)
        {
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
            if (damageReceiver == null) damageReceiver = GetComponent<DamageReceiver>();
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