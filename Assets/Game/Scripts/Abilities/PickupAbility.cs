using Game.Scripts.Core;
using Game.Scripts.Interactions;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    [DisallowMultipleComponent]
    public class PickupAbility : PickupBase
    {
        [Header("Ability")]
        [SerializeField] private AbilityBase abilityPrefab;

        public int AbilityId => abilityPrefab ? abilityPrefab.AbilityId : -1;
        
        public GameObject GetAbilityPrefab()
        {
            return abilityPrefab.gameObject;
        }

        protected override bool TryPickup(Collider2D other)
        {
            if (other.TryGet<PlayerController>(out var player))
            {
                var abilityObject = player.GetAbility();

                if (abilityObject && abilityObject.TryGetComponent<AbilityBase>(out var activeAbility))
                {
                    if (activeAbility.AbilityId == AbilityId) return false;
                }
                
                activeAbility = Instantiate(abilityPrefab);
                activeAbility.SetPlayer(player);

                return true;
            }
            
            return false;
        }
    }
}