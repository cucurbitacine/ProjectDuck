using Game.Movements;
using UnityEngine;

namespace Game.Utils
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Kickable : MonoBehaviour
    {
        private Rigidbody2D rigidbody2d; 
        
        private void Awake()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var source = other.collider.attachedRigidbody
                ? other.collider.attachedRigidbody.gameObject
                : other.collider.gameObject;
            
            if (source.TryGetComponent<Movement2D>(out var movement))
            {
                var contact = other.GetContact(0);
                
                var origin = contact.point;
                var scale = 1f - Mathf.Abs(Vector2.Dot(contact.normal, movement.groundUp));
                
                var direction = (contact.normal + movement.groundUp).normalized;
                
                var impulse = scale * contact.normalImpulse * direction;
                rigidbody2d.AddForceAtPosition(impulse, origin, ForceMode2D.Impulse);
                
                Debug.DrawLine(origin, movement.groundUp * 0.1f + origin, Color.yellow, 1f);
                Debug.DrawLine(origin, scale * direction + origin, Color.blue, 1f);
            }
        }
    }
}