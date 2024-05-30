using System;
using UnityEngine;

namespace Game.Combat
{
    public class Health : MonoBehaviour
    {
        public const int MinHealthMax = 1;
        public const int DeadlyHealth = 0;
        
        [field: SerializeField] public bool IsDead { get; private set; }
        [field: SerializeField, Min(DeadlyHealth)] public int HealthCurrent { get; private set; } = 90;
        [field: SerializeField, Min(MinHealthMax)] public int HealthMax { get; private set; } = 100;

        /// <typeparam name="Current">Health Current</typeparam>
        /// <typeparam name="Max">Health Max</typeparam>
        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public void SetHealth(int newHealth, int newHealthMax)
        {
            if (IsDead) return;
            
            var previousHealth = HealthCurrent;
            var previousHealthMax = HealthMax;
            
            HealthMax = Mathf.Max(MinHealthMax, newHealthMax);
            HealthCurrent = Mathf.Clamp(newHealth, DeadlyHealth, HealthMax);
            
            if (HealthCurrent == DeadlyHealth)
            {
                IsDead = true;
            }

            if (previousHealth != HealthCurrent || previousHealthMax != HealthMax)
            {
                OnHealthChanged?.Invoke(HealthCurrent, HealthMax);
                
                if (IsDead)
                {
                    OnDied?.Invoke();
                }
            }
        }

        public void SetHealth(int newHealth)
        {
            SetHealth(newHealth, HealthMax);
        }
        
        public void SetHealthMax(int newHealthMax)
        {
            SetHealth(HealthCurrent, newHealthMax);
        }
        
        public void Delta(int delta)
        {
            SetHealth(HealthCurrent + delta);
        }
        
        public void Heal(int amount)
        {
            Delta(amount);
        }

        public void Damage(int amount)
        {
            Delta(-amount);
        }

        public void Revive()
        {
            IsDead = false;

            SetHealth(HealthCurrent > DeadlyHealth ? HealthCurrent : 1);
        }
    }
}