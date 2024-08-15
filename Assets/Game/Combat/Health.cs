using System;
using CucuTools.DamageSystem;
using UnityEngine;

namespace Game.Combat
{
    public class Health : DamageReceiver
    {
        public const int MinHealthMax = 1;
        public const int DeadlyHealth = 0;
        
        [field: Header("Health")]
        [field: SerializeField] public bool IsDead { get; private set; }
        [field: Space]
        [field: SerializeField, Min(DeadlyHealth)] public int HealthCurrent { get; private set; } = 90;
        [field: SerializeField, Min(MinHealthMax)] public int HealthMax { get; private set; } = 100;

        [Space]
        [SerializeField] private bool immortal = false;
        
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
                if (!immortal)
                {
                    IsDead = true;
                }
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

        public void Revive(int amount)
        {
            IsDead = false;

            SetHealth(amount);
        }
        
        [ContextMenu(nameof(Revive))]
        public void Revive()
        {
            Revive(HealthCurrent > DeadlyHealth ? HealthCurrent : 1);
        }

        [ContextMenu(nameof(Death))]
        public void Death()
        {
            Damage(HealthCurrent);
        }
        
        public override void HandleDamageEvent(DamageEvent damageEvent)
        {
            if (damageEvent.damage.amount > 0)
            {
                Damage(damageEvent.damage.amount);
            }
            else if (damageEvent.damage.amount < 0)
            {
                Heal(-damageEvent.damage.amount);
            }
        }
    }
}