using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.DamageSystem
{
    [DisallowMultipleComponent]
    public class DamageReceiver : MonoBehaviour
    {
        [SerializeField] private GameObject entity;
        
        private readonly List<IDamageModifier> modifiers = new List<IDamageModifier>();
        
        public event Action<Damage> OnDamageReceived;
        
        public GameObject Entity => entity ? entity : (entity = gameObject);
        
        public void Receive(Damage damage, Action<Damage> callback = null)
        {
            ModifyDamage(damage);
            
            OnDamageReceived?.Invoke(damage);
            
            callback?.Invoke(damage);
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