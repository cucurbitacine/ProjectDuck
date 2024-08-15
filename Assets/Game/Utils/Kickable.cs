using System;
using Game.InteractionSystem;
using Game.Movements;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Kickable : MonoBehaviour, IInteraction, IFocused
    {
        [SerializeField] private bool paused = false;
        
        private Rigidbody2D rigidbody2d; 
        
        private void Awake()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (paused) return;
            
            var source = other.collider.attachedRigidbody
                ? other.collider.attachedRigidbody.gameObject
                : other.collider.gameObject;
            
            if (source.TryGetComponent<Movement2D>(out var movement))
            {
                var contact = other.GetContact(0);
                
                var origin = contact.point;
                var scale = 1f - Mathf.Abs(Vector2.Dot(contact.normal, movement.groundUp));
                
                var direction = (contact.normal + movement.groundUp).normalized;
                
                var impulse = kickScale * scale * contact.normalImpulse * direction;
                rigidbody2d.AddForceAtPosition(impulse, origin, ForceMode2D.Impulse);
                
                Debug.DrawLine(origin, movement.groundUp * 0.1f + origin, Color.yellow, 1f);
                Debug.DrawLine(origin, scale * direction + origin, Color.blue, 1f);
            }
        }

        public event Action<GameObject> OnInteracted;
        
        [SerializeField] private float kickScale = 1f;
        [Space]
        [SerializeField] private float interactForce = 1f;
        [SerializeField] private float interactTorque = 1f;
        
        public void Interact(GameObject actor)
        {
            if (paused) return;
            
            rigidbody2d.AddForce(Vector2.up * interactForce, ForceMode2D.Impulse);
            rigidbody2d.AddTorque(Mathf.Sign(Random.value - 0.5f) * interactTorque, ForceMode2D.Impulse);
            
            OnInteracted?.Invoke(actor);
        }

        public bool Focused { get; private set; }
        public event Action<bool> OnFocusChanged;
        public void Focus(bool value)
        {
            Focused = value;
            
            OnFocusChanged?.Invoke(value);
        }
    }
}