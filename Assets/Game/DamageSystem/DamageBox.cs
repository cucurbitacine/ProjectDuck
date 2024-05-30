using UnityEngine;

namespace Game.DamageSystem
{
    public class DamageBox : MonoBehaviour
    {
        [SerializeField] private DamageSource damageSource;
        
        [Header("Settings")]
        [SerializeField] private bool isTrigger = false;
        [SerializeField] private bool directDamageGameObject = false;
        [SerializeField] private bool selfDamageAllowed = false;

        public void SetDamageSource(DamageSource source)
        {
            damageSource = source;
        }
        
        private void SendDamage(Collider2D cld2d)
        {
            if (!damageSource) return;
            
            var target = directDamageGameObject || !cld2d.attachedRigidbody
                ? cld2d.gameObject
                : cld2d.attachedRigidbody.gameObject;

            if (!target.TryGetComponent<DamageReceiver>(out var damageReceiver)) return;

            if (!selfDamageAllowed && damageSource.Entity == damageReceiver.Entity) return;
                
            damageSource.SendDamage(damageReceiver);
        }

        private void Initialize()
        {
            if (damageSource == null)
            {
                SetDamageSource(GetComponentInParent<DamageSource>());
            }
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!isTrigger)
            {
                SendDamage(other.collider);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isTrigger)
            {
                SendDamage(other);
            }
        }
    }
}