using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Abilities.Teleportation
{
    public class PlayerTeleport : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private Transform destination;
        
        public void Teleport()
        {
            if (destination == null) destination = transform;

            var abilityObject = player.GetAbility();

            if (abilityObject && abilityObject.TryGetComponent<AbilityBase>(out var ability) && ability is TeleportationAbility teleportationAbility)
            {
                teleportationAbility.Teleport(destination.position);
            }
        }
    }
}