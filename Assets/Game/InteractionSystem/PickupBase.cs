using UnityEngine;

namespace Game.InteractionSystem
{
    [DisallowMultipleComponent]
    public class PickupBase : ToggleBase
    {
        [Header("Pickup")]
        [SerializeField] private bool destroyAfterPick = false;
        [Min(0f)] [SerializeField] private float enableTimeAfterDisabled = 0f;

        public void Pickup(Collider2D other)
        {
            if (Paused) return;
            
            if (!TryPickup(other)) return;

            TurnOn(true);
            
            AfterPick();
        }
        
        [ContextMenu(nameof(AfterPick))]
        public void AfterPick()
        {
            if (destroyAfterPick)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);

                if (enableTimeAfterDisabled > 0f)
                {
                    Invoke(nameof(ResetPickup), enableTimeAfterDisabled);
                }
            }
        }
        
        [ContextMenu(nameof(ResetPickup))]
        public void ResetPickup()
        {
            gameObject.SetActive(true);

            TurnOn(false);
            
            OnResetPickup();
        }
        
        protected virtual bool TryPickup(Collider2D other)
        {
            return true;
        }
        
        protected virtual void OnResetPickup()
        {
        }
        
        protected virtual void Awake()
        {
            if (TryGetComponent<Collider2D>(out var cld2d))
            {
                cld2d.isTrigger = true;
            }
        }
        
        protected virtual void OnValidate()
        {
            if (TryGetComponent<Collider2D>(out var cld2d))
            {
                cld2d.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Pickup(other);
        }
    }
}