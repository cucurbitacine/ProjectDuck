using Game.InteractionSystem;
using Game.Player;
using Game.Utils;
using UnityEngine;

namespace Game.Abilities
{
    [DisallowMultipleComponent]
    public class PickupAbility : PickupBase
    {
        [Header("Ability")]
        [SerializeField] private AbilityBase abilityPrefab;

        public int AbilityId => abilityPrefab ? abilityPrefab.AbilityId : -1;
        
        public AbilityBase GetAbilityPrefab()
        {
            return abilityPrefab;
        }

        protected override bool TryPickup(Collider2D other)
        {
            return other.TryGet<PlayerController>(out var player) && player.PickAbility(this);
        }
    }
}