using System;
using UnityEngine;

namespace Game.Combat.Utils
{
    [RequireComponent(typeof(Health))]
    public class HealthLogger : MonoBehaviour
    {
        private Health _health;
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

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthChanged += HandleHealth;

            HandleHealth(_health.HealthCurrent, _health.HealthMax);
        }
        
        private void OnDisable()
        {
            _health.OnHealthChanged -= HandleHealth;
        }
    }
}