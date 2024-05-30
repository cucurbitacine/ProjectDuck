using System;
using System.Collections.Generic;
using Game.Utils;
using UnityEngine;

namespace Game.Movements
{
    [DisallowMultipleComponent]
    public class Ground2D : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField] private float width = 1f;
        [Min(0f)]
        [SerializeField] private float distance = 0.1f;
        [SerializeField] private LayerMask groundLayer = 1;

        [Space] 
        [Range(0f, 90f)]
        [SerializeField] private float maxSlopeAngle = 45f;
        
        [field: Space, SerializeField]
        public Vector2 Direction { get; set; } = Vector2.down;
        
        private readonly HashSet<Collider2D> ignore = new HashSet<Collider2D>();
        
        private GroundHit2D groundHit { get; set; }
        
        private Vector2 raycastOrigin => pointGroundCheck - Direction.normalized * raycastDistance * 0.5f;
        private float raycastWidth => width;
        private float raycastDistance => distance;
        private Vector2 raycastSize => new Vector2(raycastWidth, raycastDistance);
        private float raycastAngle => Vector2.SignedAngle(Vector2.up, Direction);
        
        private bool _wasGrounded;
        
        public event Action<bool> OnChanged;
        
        public bool isGrounded => onSurface && Mathf.Abs(groundSlopeAngle) <= maxSlopeAngle && !ignore.Contains(groundCollider);
        public bool onSurface => groundHit;
        public bool onSlope => !Mathf.Approximately(Mathf.Abs(groundSlopeAngle), 0f);
        public bool onPlatform => groundRigidbody && Platforms.TryGetValue(groundRigidbody, out var platform) && platform.active;
        
        public Vector2 pointGroundCheck => transform.position;
        public Vector2 groundPoint => groundHit.raycastHit ? groundHit.raycastHit.point : pointGroundCheck;
        public Vector2 groundNormal => groundHit.raycastHit ? groundHit.raycastHit.normal : -Direction;
        public Rigidbody2D groundRigidbody => groundHit.raycastHit.rigidbody;
        public Collider2D groundCollider => groundHit.raycastHit.collider;
        public Vector2 groundVelocity => groundRigidbody
            ? groundRigidbody.GetPointVelocity(groundPoint)
            : Vector2.zero;
        
        public float groundSlopeAngle => Vector2.SignedAngle(groundNormal, -Direction);
        
        private static readonly Dictionary<Rigidbody2D, Platform2D> Platforms = new Dictionary<Rigidbody2D, Platform2D>();
        
        public static bool AddPlatform(Rigidbody2D rigidbody2d, Platform2D platform2D)
        {
            return Platforms.TryAdd(rigidbody2d, platform2D);
        }
        
        public static bool RemovePlatform(Rigidbody2D rigidbody2d, out Platform2D platform2D)
        {
            return Platforms.Remove(rigidbody2d, out platform2D);
        }
        
        public void Ignore(Collider2D cld, bool value)
        {
            if (value)
            {
                if (isGrounded && groundCollider == cld)
                {
                    groundHit = default;
                    
                    HandleEvent();
                }
                
                if (ignore.Add(cld))
                {
                    //Debug.Log($"Start Ignore {cld.name}");
                }
            }
            else
            {
                if (ignore.Remove(cld))
                {
                    //Debug.Log($"Stop Ignore {cld.name}");
                }
            }
        }
        
        public void CheckGround()
        {
            groundHit = (GroundHit2D)Physics2D.BoxCast(raycastOrigin, raycastSize, raycastAngle, Direction, raycastDistance, groundLayer);

            if (groundHit && ignore.Contains(groundHit.raycastHit.collider))
            {
                groundHit = default;
            }

            HandleEvent();
        }

        private void HandleEvent()
        {
            if ((!_wasGrounded && isGrounded) || (_wasGrounded && !isGrounded))
            {
                OnChanged?.Invoke(isGrounded);
            }

            _wasGrounded = isGrounded;
        }
        
        private void OnDrawGizmos()
        {
            if (onSurface)
            {
                Gizmos.color = isGrounded ? Color.green : Color.yellow;
                Gizmos.DrawSphere(groundPoint, 0.1f);
                
                Gizmos.color = new Color(0.75f, 0.75f, 0.75f, 0.75f);
                Tools.DrawBox(raycastOrigin + Direction.normalized * groundHit.raycastHit.distance, raycastSize, raycastAngle);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(groundPoint, 0.1f);
                
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                Tools.DrawBox(raycastOrigin + Direction.normalized * raycastDistance, raycastSize, raycastAngle);
            }
        }

        
    }

    [Serializable]
    public struct GroundHit2D
    {
        public RaycastHit2D raycastHit;
        
        public static implicit operator bool(GroundHit2D hit) => hit.raycastHit;
        public static explicit operator GroundHit2D(RaycastHit2D hit) => new GroundHit2D() { raycastHit = hit };
    }
}