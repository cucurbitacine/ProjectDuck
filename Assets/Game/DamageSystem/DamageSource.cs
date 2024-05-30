using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DamageSystem
{
    [DisallowMultipleComponent]
    public abstract class DamageSource : MonoBehaviour
    {
        [SerializeField] private GameObject entity;
        
        private readonly List<IDamageModifier> modifiers = new List<IDamageModifier>();
        
        public event Action<Damage> OnDamageDelivered;
        public GameObject Entity => entity ? entity : (entity = gameObject);

        public abstract Damage CreateDamage(DamageReceiver receiver);

        public void SendDamage(DamageReceiver receiver, Damage damage)
        {
            ModifyDamage(damage);
            
            receiver.Receive(damage, HandleDamageDelivery);
        }
        
        public void SendDamage(DamageReceiver receiver)
        {
            var damage = CreateDamage(receiver);

            SendDamage(receiver, damage);
        }

        private void HandleDamageDelivery(Damage damage)
        {
            OnDamageDelivered?.Invoke(damage);
        }

        private void ModifyDamage(Damage damage)
        {
            foreach (var modifier in modifiers)
            {
                modifier.ModifyDamage(damage);
            }
        }
        
        private void Awake()
        {
            modifiers.AddRange(GetComponents<IDamageModifier>());
        }
    }
}