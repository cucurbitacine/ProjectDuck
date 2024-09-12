using System;
using CucuTools.DamageSystem;
using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Combat
{
    
    public class DamageSourceSfx : MonoBehaviour
    {
        [SerializeField] private DamageSource damageSource;

        [Header("SFX")]
        [SerializeField] private SoundFX sfx;

        private void HandleDamage(DamageEvent damageEvent)
        {
            if (damageEvent.damage.amount > 0)
            {
                sfx.Play();
            }
        }
        
        private void OnEnable()
        {
            damageSource.OnDamageDelivered += HandleDamage;
        }
        
        private void OnDisable()
        {
            damageSource.OnDamageDelivered -= HandleDamage;
        }
    }
}
