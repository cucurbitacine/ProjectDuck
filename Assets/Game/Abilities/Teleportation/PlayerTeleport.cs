using Game.Core;
using Game.LevelSystem;
using UnityEngine;

namespace Game.Abilities.Teleportation
{
    public class PlayerTeleport : MonoBehaviour
    {
        [SerializeField] private Transform destination;
        
        public void Teleport()
        {
            if (destination == null) destination = transform;

            var ability = LevelManager.Player.GetAbility();

            if (ability && ability is TeleportationAbility teleportationAbility)
            {
                teleportationAbility.Teleport(destination.position);
            }
        }
    }
}