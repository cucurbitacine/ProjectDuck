using System;
using CucuTools.DamageSystem;
using Game.InteractionSystem;
using Game.Player;
using Game.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Combat
{
    public class HealPickup : PickupBase, IInteraction
    {
        [Header("Healing")]
        [Min(0)]
        [SerializeField] private int healAmount = 50;
        [Space]
        [SerializeField] private UnityEvent onPicked = new UnityEvent();

        [Header("References")]
        [SerializeField] private DamageSource source;

        public event Action<GameObject> OnInteracted;
        
        public void Interact(GameObject actor)
        {
            if (!actor.TryGetComponent<PlayerController>(out var player)) return;
            
            HealPlayer(player);
                
            AfterPick();
            
            OnInteracted?.Invoke(actor);
        }
        
        public void HealPlayer(PlayerController player)
        {
            var healingDamage = new Damage() { amount = -healAmount };
            
            source.SendDamage(healingDamage, player.Health);
                
            onPicked.Invoke();
        }

        protected override bool TryPickup(Collider2D other)
        {
            if (!other.TryGet<PlayerController>(out var player)) return false;
            
            HealPlayer(player);

            return true;
        }
    }
}