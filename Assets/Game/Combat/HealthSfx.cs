using CucuTools.DamageSystem;
using Game.SFX;
using UnityEngine;

namespace Game.Combat
{
    public class HealthSfx : MonoBehaviour
    {
        [SerializeField] private float timeout = 0.5f;
        
        [Header("SFX")]
        [SerializeField]  private SoundFX damageSfx;
        [SerializeField]  private SoundFX deathSfx;
        
        [Header("References")]
        [SerializeField] private Health health;
        
        private float _lastTime;
        
        private void HandleDamage(DamageEvent damageEvent)
        {
            if (health.IsDead) return;
            
            var time = Time.time;
            
            if (damageEvent.damage.amount > 0 && time - _lastTime > timeout)
            {
                damageSfx.PlaySfx();

                _lastTime = time;
            }
        }
        
        private void HandleDeath()
        {
            if (deathSfx)
            {
                deathSfx.PlaySfx();
            }
        }
        
        private void OnEnable()
        {
            health.OnDamageReceived += HandleDamage;
            health.OnDied += HandleDeath;
        }
        
        private void OnDisable()
        {
            health.OnDamageReceived -= HandleDamage;
            health.OnDied -= HandleDeath;
        }
    }
}