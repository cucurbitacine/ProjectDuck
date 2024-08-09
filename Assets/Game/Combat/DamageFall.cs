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
        
        [Header("References")]
        [SerializeField] private PlayerController player;
 
        private void Fall(Vector2 relativeVelocity)
        {
            var projectedVelocity = Vector3.Project(relativeVelocity, player.GetMovement2D().Ground2D.Direction);

            var groundedSpeed = projectedVelocity.magnitude;

            var maxSpeed = Mathf.Sqrt(2 * player.GetMovement2D().gravity.magnitude * heightMax);
                
            Debug.Log($"speed {groundedSpeed} < {maxSpeed}");
            
            if (groundedSpeed > maxSpeed)
            {
                this.SendDamage(player.Health.DamageReceiver);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (Paused) return;

            if (player.GetMovement2D().Ground2D.groundCollider != other.collider) return;
            
            Fall(other.relativeVelocity);
        }
    }
}