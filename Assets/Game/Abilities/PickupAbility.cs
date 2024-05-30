using Game.Player;
using Game.Utils;
using UnityEngine;

namespace Game.Abilities
{
    [DisallowMultipleComponent]
    public class PickupAbility : MonoBehaviour
    {
        [SerializeField] private Ability abilityPrefab;

        public Ability GetAbility()
        {
            return abilityPrefab;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGet<PlayerController>(out var player))
            {
                player.PickAbility(this);
                
                Destroy(gameObject);
            }
        }
    }
}