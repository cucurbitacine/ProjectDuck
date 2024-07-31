using Game.Player;
using Game.Utils;
using UnityEngine;

namespace Game.Abilities
{
    [DisallowMultipleComponent]
    public class PickupAbility : MonoBehaviour
    {
        [SerializeField] private bool destroyAfterPick = false;
        [SerializeField] private bool disableAfterPick = false;
        [SerializeField] private float enableTimeAfterDisabled = 0f;
        [Space]
        [SerializeField] private AbilityBase abilityPrefab;

        public AbilityBase GetAbility()
        {
            return abilityPrefab;
        }

        private void Enable()
        {
            gameObject.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGet<PlayerController>(out var player))
            {
                player.PickAbility(this);

                if (destroyAfterPick)
                {
                    Destroy(gameObject);
                }
                else if (disableAfterPick)
                {
                    gameObject.SetActive(false);

                    if (enableTimeAfterDisabled > 0f)
                    {
                        Invoke(nameof(Enable), enableTimeAfterDisabled);
                    }
                }
            }
        }
    }
}