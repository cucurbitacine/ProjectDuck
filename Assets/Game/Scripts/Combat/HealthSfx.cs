using CucuTools.DamageSystem;
using Game.Scripts.SFX;
using UnityEngine;

namespace Game.Scripts.Combat
{
    public class HealthSfx : MonoBehaviour
    {
        [SerializeField] private float timeout = 0.5f;
                
        [Header("References")]
        [SerializeField] private Health health;
        
        [Header("SFX")]
        [SerializeField]  private SoundFX damageSfx;
        [SerializeField]  private SoundFX deathSfx;
        
        private float _lastTime;
        
        private void HandleDamage(DamageEvent damageEvent)
        {
            if (health.IsDead) return;
            
            var time = Time.time;
            
            if (damageEvent.damage.amount > 0 && time - _lastTime > timeout)
            {
                damageSfx.Play();

                _lastTime = time;
            }
        }
        
        private void HandleDeath()
        {
            if (deathSfx)
            {
                deathSfx.Play();
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