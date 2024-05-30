using Game.Player;
using UnityEngine;

namespace Game.Abilities
{
    [DisallowMultipleComponent]
    public class Ability : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        
        public void SetPlayer(PlayerController newPlayer)
        {
            player = newPlayer;
            
            transform.SetParent(player.transform, false);
        }
    }
}