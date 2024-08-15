using CucuTools.DamageSystem;
using Game.InteractionSystem;
using Game.Player;
using Game.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Combat
{
    public class HealPickup : ToggleBase
    {
        [Header("Pickup")]
        [Min(0)]
        [SerializeField] private int healAmount = 50;
        
        [Space]
        [SerializeField] private bool destroyAfterPick = false;
        [SerializeField] private bool disableAfterPick = false;
        [SerializeField] private float enableTimeAfterDisabled = 0f;

        [Space]
        [SerializeField] private UnityEvent onPicked = new UnityEvent();

        [Header("References")]
        [SerializeField] private DamageSource source;
        
        private void SetActive()
        {
            gameObject.SetActive(true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGet<PlayerController>(out var player))
            {
                TurnOn(true);

                var healingDamage = new Damage() { amount = -healAmount };
                source.SendDamage(healingDamage, player.Health);
                
                onPicked.Invoke();
                
                if (destroyAfterPick)
                {
                    Destroy(gameObject);
                }
                else if (disableAfterPick)
                {
                    gameObject.SetActive(false);

                    if (enableTimeAfterDisabled > 0f)
                    {
                        Invoke(nameof(SetActive), enableTimeAfterDisabled);
                    }
                }
            }
        }

        private void Awake()
        {
            if (TryGetComponent<Collider2D>(out var cld2d))
            {
                cld2d.isTrigger = true;
            }
        }
        
        private void OnValidate()
        {
            if (TryGetComponent<Collider2D>(out var cld2d))
            {
                cld2d.isTrigger = true;
            }
        }
    }
}