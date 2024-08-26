using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    [DisallowMultipleComponent]
    public abstract class AbilityBase : MonoBehaviour
    {
        [field: SerializeField] public int AbilityId { get; private set; } = -1;
        
        public PlayerController Player { get; private set; }
        
        public void SetPlayer(PlayerController newPlayer)
        {
            Player = newPlayer;
            
            transform.SetParent(Player.transform, false);
            Player.SetAbility(gameObject);
            
            OnSetPlayer();
        }

        protected abstract void OnSetPlayer();

        public virtual void Drop()
        {
        }
    }
}