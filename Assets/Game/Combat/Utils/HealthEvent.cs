using UnityEngine;
using UnityEngine.Events;

namespace Game.Combat.Utils
{
    [RequireComponent(typeof(Health))]
    public class HealthEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent<int, int> healthEvent = new UnityEvent<int, int>();
        [SerializeField] private UnityEvent deathEvent = new UnityEvent();
        
        private Health _health;

        private void HandleHealth(int current, int maximum)
        {
            healthEvent.Invoke(current, maximum);
        }

        private void HandleDeath()
        {
            deathEvent.Invoke();
        }
        
        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnHealthChanged += HandleHealth;
            _health.OnDied += HandleDeath;
        }
        
        private void OnDisable()
        {
            _health.OnHealthChanged -= HandleHealth;
            _health.OnDied -= HandleDeath;
        }
    }
}