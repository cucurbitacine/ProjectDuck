using UnityEngine;

namespace Game.Combat
{
    public class HealthLogger : MonoBehaviour
    {
        [SerializeField] private Health health;

        private int _previousHealth; 
        
        private void HandleHealth(int current, int max)
        {
            var deltaDamage = current - _previousHealth;

            var percent = $"{(100 * current / max):F0}%";
            var healthLevel = $"{current} / {max}";
            var delta = $"{(deltaDamage > 0 ? $"+{deltaDamage}" : $"{deltaDamage}")}";
            
            Debug.Log($"{name}: {percent} [{healthLevel}] {delta}");

            _previousHealth = current;
        }
        
        private void OnEnable()
        {
            if (health)
            {
                health.OnHealthChanged += HandleHealth;

                HandleHealth(health.HealthCurrent, health.HealthMax);
            }
        }
        
        private void OnDisable()
        {
            if (health)
            {
                health.OnHealthChanged -= HandleHealth;
            }
        }
    }
}