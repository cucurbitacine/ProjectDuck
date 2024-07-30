using Game.Player;
using UnityEngine;

namespace Game.Abilities
{
    [DisallowMultipleComponent]
    public abstract class AbilityBase : MonoBehaviour
    {
        public PlayerController Player { get; private set; }
        
        public void SetPlayer(PlayerController newPlayer)
        {
            Player = newPlayer;

            transform.SetParent(Player.transform, false);

            OnSetPlayer();
        }

        protected abstract void OnSetPlayer();
    }
}