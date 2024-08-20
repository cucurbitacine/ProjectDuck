using Game.Player;
using UnityEngine;

namespace Game.Abilities
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

            OnSetPlayer();
        }

        protected abstract void OnSetPlayer();

        public virtual void Drop()
        {
        }
    }
}