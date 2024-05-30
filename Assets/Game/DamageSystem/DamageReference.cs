using UnityEngine;

namespace Game.DamageSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DamageReceiver))]
    public class DamageReference : MonoBehaviour
    {
        [SerializeField] private DamageReceiver target;
        
        private DamageReceiver receiver { get; set; }
        
        private void HandleDamageEvent(Damage damage)
        {
            if (target)
            {
                target.Receive(damage);
            }
        }

        private void Awake()
        {
            receiver = GetComponent<DamageReceiver>();
        }

        private void OnEnable()
        {
            if (receiver)
            {
                receiver.OnDamageReceived += HandleDamageEvent;
            }
        }

        private void OnDisable()
        {
            if (receiver)
            {
                receiver.OnDamageReceived -= HandleDamageEvent;
            }
        }
    }
}