using Game.Scripts.Interactions;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Abilities
{
    [DisallowMultipleComponent]
    public abstract class AbilityBase : MonoBehaviour, IDroppable
    {
        [field: SerializeField] public int AbilityId { get; private set; } = -1;

        [SerializeField] private GameObject dropEffectPrefab;
        
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
            if (dropEffectPrefab)
            {
                Instantiate(dropEffectPrefab, Player.GetBounds().center, Quaternion.identity);
            }
        }
    }
}