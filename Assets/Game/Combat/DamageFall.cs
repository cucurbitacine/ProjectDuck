using CucuTools.DamageSystem;
using Game.Player;
using UnityEngine;

namespace Game.Combat
{
    public class DamageFall : DamageSource
    {
        [field: Header("Fall")]
        [field: SerializeField] public bool Paused { get; set; }
        [SerializeField] private float heightMax = 5f;
        
        [Space]
        [SerializeField] private float maxSpeed = 5f;
        
        [Header("References")]
        [SerializeField] private PlayerController player;
 
        private void Fall(Vector2 relativeVelocity)
        {
            var projectedVelocity = Vector3.Project(relativeVelocity, player.GetMovement2D().Ground2D.Direction);

            var groundedSpeed = projectedVelocity.magnitude;

            maxSpeed = Mathf.Sqrt(2 * player.GetMovement2D().gravity.magnitude * heightMax);

            //Debug.Log($"{groundedSpeed} > {maxSpeed} ?");
            
            // TODO Handle somehow valid speed
            
            if (groundedSpeed > maxSpeed)
            {
                this.SendDamage(player.Health.DamageReceiver);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (Paused) return;

            if (player.GetMovement2D().Ground2D.groundCollider != other.collider) return;
            
            //Debug.Log($"{player.GetComponent<Rigidbody2D>().velocity} / {player.GetMovement2D().velocity} / {other.relativeVelocity}");
            
            Fall(other.relativeVelocity);
        }

        private void OnValidate()
        {
            var gravityScale = TryGetComponent<Rigidbody2D>(out var rgb) ? rgb.gravityScale : 1f;
            
            maxSpeed = Mathf.Sqrt(2 * Physics2D.gravity.magnitude * gravityScale * heightMax);
        }
    }
}