using Game.Scripts.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private Slider slider;

        public void SetHealth(Health newHealth)
        {
            if (health)
            {
                health.OnHealthChanged -= HandleHealth;
            }

            health = newHealth;

            if (health)
            {
                health.OnHealthChanged += HandleHealth;
                
                HandleHealth(health.HealthCurrent, health.HealthMax);
            }
        }
        
        private void HandleHealth(int current, int maximum)
        {
            if (slider)
            {
                slider.value = (float)current / maximum;
            }
        }

        private void OnEnable()
        {
            if (health)
            {
                health.OnHealthChanged += HandleHealth;
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
